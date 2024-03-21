using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Database;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Helpers;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Accounts;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace OKeeffeCraft.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;

        public AccountService(
            DataContext context,
            IJwtUtils jwtUtils,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email);

            // validate
            if (account == null || !BC.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            if (!account.IsVerified)
                throw new AppException("Account email not confirmed");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(account);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;

            return new ServiceResponse<AuthenticateResponse> { Success = true, Message = "User Authenticated Successfully.", Data = response };
        }

        public async Task<ServiceResponse<AuthenticateResponse>> RefreshToken(string? token, string? ipAddress)
        {
            if (string.IsNullOrEmpty(token))
                throw new AppException("Token is required");

            if (string.IsNullOrEmpty(ipAddress))
                throw new AppException("IP Address is required");

            var account = await GetAccountByRefreshToken(token);
            var refreshToken = account?.RefreshTokens?.Single(x => x.Token == token) ?? throw new AppException("Invalid token");

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Update(account);
                await _context.SaveChangesAsync();
            }

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            account.RefreshTokens.Add(newRefreshToken);


            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            await _context.SaveChangesAsync();

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(account);

            // return data in authenticate response object
            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return new ServiceResponse<AuthenticateResponse> { Success = true, Message = "Token Refreshed.", Data = response };
        }

        public async Task<ServiceResponse<string>> RevokeToken(string token, string ipAddress)
        {
            var account = await GetAccountByRefreshToken(token);

            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken == null || refreshToken.IsRevoked)
                throw new AppException("Token not found");

            if (!refreshToken.IsActive)
                throw new AppException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _context.Update(account);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = null, Message = "Refresh token revoked.", Success = true };
        }

        public async Task<ServiceResponse<AccountModel>> Register(RegisterRequest model, string origin)
        {
            // validate
            if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                SendAlreadyRegisteredEmail(model.Email, origin);
                return new ServiceResponse<AccountModel> { Data = null, Message = "Email address already registered.", Success = false };
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            // first registered account is an admin
            var isFirstAccount = !await _context.Accounts.AnyAsync();
            account.Role = isFirstAccount ? Role.Admin : Role.User;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = await GenerateVerificationToken();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // send email
            SendVerificationEmail(account, origin);
            return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(account), Message = "Registration successful, please check your email for verification instructions.", Success = true };
        }

        public async Task<ServiceResponse<string>> VerifyEmail(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.VerificationToken == token) ?? throw new AppException("Verification failed");

            if (account.IsVerified)
                return new ServiceResponse<string> { Data = null, Message = "Email already verified", Success = false };

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Data = null, Message = "Verification successful, you can now login", Success = true };
        }

        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return new ServiceResponse<string> { Data = null, Message = "No account associated with that email", Success = false };

            // create reset token that expires after 1 day
            account.ResetToken = await GenerateResetToken();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            // send email
            SendPasswordResetEmail(account, origin);
            return new ServiceResponse<string> { Data = null, Message = "Please check your email for password reset instructions", Success = true };
        }

        public async Task<ServiceResponse<AccountModel>> ValidateResetToken(ValidateResetTokenRequest model)
        {
            var response = await GetAccountByResetToken(model.Token);
            return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(response), Message = "Token is valid", Success = true };
        }

        public async Task<ServiceResponse<AccountModel>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await GetAccountByResetToken(model.Token);

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(account), Message = "Password reset successful, you can now login", Success = true };
        }

        public async Task<ServiceResponse<IEnumerable<AccountResponse>>> GetAll()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return new ServiceResponse<IEnumerable<AccountResponse>> { Data = _mapper.Map<IList<AccountResponse>>(accounts), Message = "Accounts retrieved successfully", Success = true };
        }

        public async Task<ServiceResponse<AccountResponse>> GetById(int id)
        {
            var account = await GetAccount(id);
            return new ServiceResponse<AccountResponse> { Data = _mapper.Map<AccountResponse>(account), Message = "Account retrieved successfully", Success = true };
        }

        public async Task<ServiceResponse<AccountResponse>> Create(CreateRequest model)
        {
            // validate
            if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new ServiceResponse<AccountResponse> { Success = true, Message = "Account created successfully.", Data = _mapper.Map<AccountResponse>(account) };
        }

        public async Task<ServiceResponse<AccountResponse>> Update(int id, UpdateRequest model)
        {
            var account = await GetAccount(id);

            // validate
            if (account.Email != model.Email && await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return new ServiceResponse<AccountResponse> { Success = true, Message = "Account updated successfully.", Data = _mapper.Map<AccountResponse>(account) };
        }

        public async Task<ServiceResponse<string>> Delete(int id)
        {
            var account = await GetAccount(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return new ServiceResponse<string> { Success = true, Message = "Account deleted successfully." };
        }

        // helper methods

        private async Task<Account> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account ?? throw new KeyNotFoundException("Account not found");
        }

        private async Task<Account> GetAccountByRefreshToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            return account ?? throw new AppException("Invalid token");
        }

        private async Task<Account> GetAccountByResetToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
            return account ?? throw new AppException("Invalid token");
        }

        private async Task<string> GenerateResetToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.ResetToken == token);
            if (!tokenIsUnique)
                return await GenerateResetToken();

            return token;
        }

        private async Task<string> GenerateVerificationToken()
        {
            // token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // ensure token is unique by checking against db
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.VerificationToken == token);
            if (!tokenIsUnique)
                return await GenerateVerificationToken();

            return token;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account?.RefreshTokens?.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account? account, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = account?.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken != null && childToken.IsActive)
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else if (childToken != null && !childToken.IsActive)
                    RevokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
            }
        }

        private static void RevokeRefreshToken(RefreshToken token, string? ipAddress, string? reason = null, string? replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        private void SendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                // origin exists if request sent from browser single page app (e.g. Angular or React)
                // so send link to verify via single page app
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                // origin missing if request sent directly to api (e.g. from Postman)
                // so send instructions to verify directly with api
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}"
            );
        }

        private void SendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                        {message}"
            );
        }
    }
}
