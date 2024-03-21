using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Authorization.Interfaces;
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
        private readonly ILogService _logService;
        private readonly IAuthIdentityService _authIdentityService;

        public AccountService(
            DataContext context,
            IJwtUtils jwtUtils,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            ILogService logService,
            IAuthIdentityService identityService)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _logService = logService;
            _authIdentityService = identityService;
        }

        /// <summary>
        /// Authenticates a user with the provided credentials.
        /// </summary>
        /// <param name="model">The authentication request model containing user credentials.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with authentication details.</returns>
        public async Task<ServiceResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == model.Email);

                // validate
                if (account == null || !BC.Verify(model.Password, account.PasswordHash))
                    throw new AppException("Email or password is incorrect");

                if (!account.IsVerified)
                    throw new AppException("Account email not confirmed");

                // Authentication successful, so generate JWT and refresh tokens
                var jwtToken = _jwtUtils.GenerateJwtToken(account);
                var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
                account.RefreshTokens.Add(refreshToken);

                // Remove old refresh tokens from account
                RemoveOldRefreshTokens(account);

                // Save changes to the database
                _context.Update(account);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<AuthenticateResponse>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = refreshToken.Token;
                await _logService.ActivityLog("User authenticated successfully", "Email", account.Email);
                return new ServiceResponse<AuthenticateResponse> { Success = true, Message = "User Authenticated Successfully.", Data = response };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "IP address", ipAddress);
                return new ServiceResponse<AuthenticateResponse> { Success = false, Message = error.Message, Data = null };
            }

        }

        /// <summary>
        /// Refreshes an authentication token for the provided refresh token.
        /// </summary>
        /// <param name="token">The refresh token used for authentication.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with refreshed authentication details.</returns>
        public async Task<ServiceResponse<AuthenticateResponse>> RefreshToken(string? token, string? ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new AppException("Token is required");

                if (string.IsNullOrEmpty(ipAddress))
                    throw new AppException("IP Address is required");

                var account = await GetAccountByRefreshToken(token);
                var refreshToken = account?.RefreshTokens?.Single(x => x.Token == token) ?? throw new AppException("Invalid token");

                if (refreshToken.IsRevoked)
                {
                    // Revoke all descendant tokens in case this token has been compromised
                    RevokeDescendantRefreshTokens(refreshToken, account, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }

                if (!refreshToken.IsActive)
                    throw new AppException("Invalid token");

                // Replace old refresh token with a new one (rotate token)
                var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
                account.RefreshTokens.Add(newRefreshToken);

                // Remove old refresh tokens from account
                RemoveOldRefreshTokens(account);

                // Save changes to the database
                _context.Update(account);
                await _context.SaveChangesAsync();

                // Generate new JWT
                var jwtToken = _jwtUtils.GenerateJwtToken(account);

                // Return data in authenticate response object
                var response = _mapper.Map<AuthenticateResponse>(account);
                response.JwtToken = jwtToken;
                response.RefreshToken = newRefreshToken.Token;
                return new ServiceResponse<AuthenticateResponse> { Success = true, Message = "Token Refreshed.", Data = response };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "IP address", ipAddress);
                return new ServiceResponse<AuthenticateResponse> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        /// <param name="token">The refresh token to be revoked.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the revocation.</returns>
        public async Task<ServiceResponse<string>> RevokeToken(string token, string ipAddress)
        {
            try
            {
                var account = await GetAccountByRefreshToken(token);

                var refreshToken = account.RefreshTokens.SingleOrDefault(x => x.Token == token);

                if (refreshToken == null || refreshToken.IsRevoked)
                    throw new AppException("Token not found");

                if (!refreshToken.IsActive)
                    throw new AppException("Invalid token");

                // Revoke token and save
                RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
                _context.Update(account);
                await _context.SaveChangesAsync();

                return new ServiceResponse<string> { Data = null, Message = "Refresh token revoked.", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "IP address", ipAddress);
                return new ServiceResponse<string> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Registers a new account.
        /// </summary>
        /// <param name="model">The registration request model containing user details.</param>
        /// <param name="origin">The origin URL for verification email (optional).</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with the registered account details.</returns>
        public async Task<ServiceResponse<AccountModel>> Register(RegisterRequest model, string origin)
        {
            try
            {
                if (model == null)
                    throw new AppException("Model is null");

                // Validate
                if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                {
                    // Send already registered error in email to prevent account enumeration
                    _emailService.SendAlreadyRegisteredEmail(model.Email, origin);
                    return new ServiceResponse<AccountModel> { Data = null, Message = "Email address already registered.", Success = false };
                }

                // Map model to new account object
                var account = _mapper.Map<Account>(model);

                // First registered account is an admin
                var isFirstAccount = !await _context.Accounts.AnyAsync();
                account.Role = isFirstAccount ? Role.Admin : Role.User;
                account.Created = DateTime.UtcNow;
                account.VerificationToken = await GenerateVerificationToken();

                // Hash password
                account.PasswordHash = BC.HashPassword(model.Password);

                // Save account
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                // Send email
                _emailService.SendVerificationEmail(account, origin);
                await _logService.ActivityLog("User registered successfully", "Email", account.Email);
                return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(account), Message = "Registration successful, please check your email for verification instructions.", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Email", model.Email);
                return new ServiceResponse<AccountModel> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Verifies the email address associated with the provided verification token.
        /// </summary>
        /// <param name="token">The verification token sent to the user's email address.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the verification.</returns>
        public async Task<ServiceResponse<string>> VerifyEmail(string token)
        {
            try
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
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, null, null);
                return new ServiceResponse<string> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Initiates the process for password reset by sending a reset link to the user's email address.
        /// </summary>
        /// <param name="model">The request model containing the email address of the user.</param>
        /// <param name="origin">The origin URL for password reset link (optional).</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the password reset initiation.</returns>
        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            try
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x => x.Email == model.Email);

                // Always return an OK response to prevent email enumeration
                if (account == null) return new ServiceResponse<string> { Data = null, Message = "No account associated with that email", Success = false };

                // Create a reset token that expires after 1 day
                account.ResetToken = await GenerateResetToken();
                account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                // Send email
                _emailService.SendPasswordResetEmail(account, origin);
                return new ServiceResponse<string> { Data = null, Message = "Please check your email for password reset instructions", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Email", model.Email);
                return new ServiceResponse<string> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Validates the reset token provided by the user for password reset.
        /// </summary>
        /// <param name="model">The request model containing the reset token to validate.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the token validation.</returns>
        public async Task<ServiceResponse<AccountModel>> ValidateResetToken(ValidateResetTokenRequest model)
        {
            try
            {
                var response = await GetAccountByResetToken(model.Token);
                return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(response), Message = "Token is valid", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, null, null);
                return new ServiceResponse<AccountModel> { Success = false, Message = error.Message, Data = null };
            }

        }

        /// <summary>
        /// Resets the password for the user associated with the provided reset token.
        /// </summary>
        /// <param name="model">The request model containing the reset token and the new password.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the password reset.</returns>
        public async Task<ServiceResponse<AccountModel>> ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                var account = await GetAccountByResetToken(model.Token);

                // Update password and remove reset token
                account.PasswordHash = BC.HashPassword(model.Password);
                account.PasswordReset = DateTime.UtcNow;
                account.ResetToken = null;
                account.ResetTokenExpires = null;

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
                return new ServiceResponse<AccountModel> { Data = _mapper.Map<AccountModel>(account), Message = "Password reset successful, you can now login", Success = true };

            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, null, null);
                return new ServiceResponse<AccountModel> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Retrieves all user accounts.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the service response with the list of user accounts.</returns>
        public async Task<ServiceResponse<IEnumerable<AccountResponse>>> GetAll()
        {
            try
            {
                var accounts = await _context.Accounts.ToListAsync();
                return new ServiceResponse<IEnumerable<AccountResponse>> { Data = _mapper.Map<IList<AccountResponse>>(accounts), Message = "Accounts retrieved successfully", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
                return new ServiceResponse<IEnumerable<AccountResponse>> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Retrieves a user account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user account.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with the retrieved user account.</returns>
        public async Task<ServiceResponse<AccountResponse>> GetById(int id)
        {
            try
            {
                var account = await GetAccount(id);
                return new ServiceResponse<AccountResponse> { Data = _mapper.Map<AccountResponse>(account), Message = "Account retrieved successfully", Success = true };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
                return new ServiceResponse<AccountResponse> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        /// <param name="model">The request model containing the details of the account to be created.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with the created user account.</returns>
        public async Task<ServiceResponse<AccountResponse>> Create(CreateRequest model)
        {
            try
            {             // Validate
                if (await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                    throw new AppException($"Email '{model.Email}' is already registered");

                // Map model to new account object
                var account = _mapper.Map<Account>(model);
                account.Created = DateTime.UtcNow;
                account.Verified = DateTime.UtcNow;

                // Hash password
                account.PasswordHash = BC.HashPassword(model.Password);

                // Save account
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                return new ServiceResponse<AccountResponse> { Success = true, Message = "Account created successfully.", Data = _mapper.Map<AccountResponse>(account) };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
                return new ServiceResponse<AccountResponse> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Updates an existing user account.
        /// </summary>
        /// <param name="id">The unique identifier of the user account to update.</param>
        /// <param name="model">The request model containing the updated details of the account.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response with the updated user account.</returns>
        public async Task<ServiceResponse<AccountResponse>> Update(int id, UpdateRequest model)
        {
            try
            {
                var account = await GetAccount(id);

                // Validate
                if (account.Email != model.Email && await _context.Accounts.AnyAsync(x => x.Email == model.Email))
                    throw new AppException($"Email '{model.Email}' is already registered");

                // Hash password if it was entered
                if (!string.IsNullOrEmpty(model.Password))
                    account.PasswordHash = BC.HashPassword(model.Password);

                // Copy model to account and save
                _mapper.Map(model, account);
                account.Updated = DateTime.UtcNow;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();

                return new ServiceResponse<AccountResponse> { Success = true, Message = "Account updated successfully.", Data = _mapper.Map<AccountResponse>(account) };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
                return new ServiceResponse<AccountResponse> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Deletes a user account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user account to delete.</param>
        /// <returns>A task representing the asynchronous operation, containing the service response indicating the result of the deletion.</returns>
        public async Task<ServiceResponse<string>> Delete(int id)
        {
            try
            {
                var account = await GetAccount(id);
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return new ServiceResponse<string> { Success = true, Message = "Account deleted successfully." };
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
                return new ServiceResponse<string> { Success = false, Message = error.Message, Data = null };
            }
        }

        /// <summary>
        /// Retrieves a user account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user account to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved user account.</returns>
        private async Task<Account> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account ?? throw new KeyNotFoundException("Account not found");
        }

        /// <summary>
        /// Retrieves a user account associated with the provided refresh token.
        /// </summary>
        /// <param name="token">The refresh token for which to retrieve the associated user account.</param>
        /// <returns>A task representing the asynchronous operation, containing the user account associated with the refresh token.</returns>
        private async Task<Account> GetAccountByRefreshToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            return account ?? throw new AppException("Invalid token");
        }

        /// <summary>
        /// Retrieves a user account associated with the provided reset token.
        /// </summary>
        /// <param name="token">The reset token for which to retrieve the associated user account.</param>
        /// <returns>A task representing the asynchronous operation, containing the user account associated with the reset token.</returns>
        private async Task<Account> GetAccountByResetToken(string token)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(x =>
                x.ResetToken == token && x.ResetTokenExpires > DateTime.UtcNow);
            return account ?? throw new AppException("Invalid token");
        }

        /// <summary>
        /// Generates a reset token for password reset.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the generated reset token.</returns>
        private async Task<string> GenerateResetToken()
        {
            // Token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // Ensure token is unique by checking against database
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.ResetToken == token);
            if (!tokenIsUnique)
                return await GenerateResetToken();

            return token;
        }

        /// <summary>
        /// Generates a verification token for email verification.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the generated verification token.</returns>
        private async Task<string> GenerateVerificationToken()
        {
            // Token is a cryptographically strong random sequence of values
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            // Ensure token is unique by checking against database
            var tokenIsUnique = !await _context.Accounts.AnyAsync(x => x.VerificationToken == token);
            if (!tokenIsUnique)
                return await GenerateVerificationToken();

            return token;
        }

        /// <summary>
        /// Rotates a refresh token by generating a new refresh token and revoking the old one.
        /// </summary>
        /// <param name="refreshToken">The refresh token to rotate.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <returns>The new refresh token.</returns>
        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        /// <summary>
        /// Removes old refresh tokens associated with the specified account.
        /// </summary>
        /// <param name="account">The account whose refresh tokens are to be checked and removed.</param>
        private void RemoveOldRefreshTokens(Account account)
        {
            account?.RefreshTokens?.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        /// <summary>
        /// Revokes all descendant refresh tokens of the specified refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token whose descendants are to be revoked.</param>
        /// <param name="account">The account associated with the refresh token.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <param name="reason">The reason for revocation.</param>
        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, Account? account, string ipAddress, string reason)
        {
            // Recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = account?.RefreshTokens?.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken != null && childToken.IsActive)
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else if (childToken != null && !childToken.IsActive)
                    RevokeDescendantRefreshTokens(childToken, account, ipAddress, reason);
            }
        }

        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        /// <param name="token">The refresh token to revoke.</param>
        /// <param name="ipAddress">The IP address of the requesting client.</param>
        /// <param name="reason">The reason for revocation (optional).</param>
        /// <param name="replacedByToken">The token that replaces the revoked token (optional).</param>
        private static void RevokeRefreshToken(RefreshToken token, string? ipAddress, string? reason = null, string? replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

    }
}
