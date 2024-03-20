using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Accounts;
using Org.BouncyCastle.Cms;
using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [SwaggerOperation(Summary = "Authenticates a new user account on the system")]
        [SwaggerResponse(200, "User authenticated.", typeof(ServiceResponse<AuthenticateResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await _accountService.Authenticate(model, IpAddress());
            SetTokenCookie(response.Data?.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [SwaggerOperation(Summary = "Use a refresh token to get a new JWT token")]
        [SwaggerResponse(200, "Access token aquired.", typeof(ServiceResponse<AuthenticateResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _accountService.RefreshToken(refreshToken, IpAddress());
            SetTokenCookie(response.Data?.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        [SwaggerOperation(Summary = "Revoke a refresh token")]
        [SwaggerResponse(200, "Refresh token revoked.", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            // users can revoke their own tokens and admins can revoke any tokens
            if (Account == null || (!Account.OwnsToken(token) && Account.Role != Role.Admin))
                return Unauthorized(new { message = "Unauthorized" });

           var response = await _accountService.RevokeToken(token, IpAddress());
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers a new user account on the system.")]
        [SwaggerResponse(200, "User and associated account created. User must confirm email before being able to log in.", typeof(ServiceResponse<AccountModel>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            string? origin = Request.Headers.Origin;

            if (string.IsNullOrEmpty(origin))
                return BadRequest(new ServiceResponse<string> { Data = null,  Message = "Origin header is required", Success = false});

            var response = await _accountService.Register(model, origin);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("verify-email")]
        [SwaggerOperation(Summary = "Verifys users email address.")]
        [SwaggerResponse(200, "Email address verified", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
        {
            var response = await _accountService.VerifyEmail(model.Token);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Request password reset.")]
        [SwaggerResponse(200, "Request valid, reset email sent.", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            string? origin = Request.Headers.Origin;

            if (string.IsNullOrEmpty(origin))
                return BadRequest(new ServiceResponse<string> { Data = null, Message = "Origin header is required", Success = false });

            var response = await _accountService.ForgotPassword(model, origin);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("validate-reset-token")]
        [SwaggerOperation(Summary = "Validate reset token.")]
        [SwaggerResponse(200, "Token valid", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
        {
            var response = await _accountService.ValidateResetToken(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Reset user password.")]
        [SwaggerResponse(200, "Password reset", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            var response = await _accountService.ResetPassword(model);
            return Ok(response);
        }

        [Authorize(Role.Admin)]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all accounts.")]
        [SwaggerResponse(200, "List of all accounts.", typeof(ServiceResponse<AccountResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> GetAll()
        {
            if (Account == null)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            // users can get their own account and admins can get any account
            if (Account.Role != Role.Admin)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            var response = await _accountService.GetAll();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get account by id.")]
        [SwaggerResponse(200, "Account found.", typeof(ServiceResponse<AccountResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> GetById(int id)
        {
            if(Account == null)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            // users can get their own account and admins can get any account
            if (id != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            var account = await _accountService.GetById(id);
            return Ok(account);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new account.")]
        [SwaggerResponse(200, "Account created.", typeof(ServiceResponse<AccountResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> Create(CreateRequest model)
        {
            if (Account == null)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            // users can get their own account and admins can get any account
            if (Account.Role != Role.Admin)
                return Unauthorized(new ServiceResponse<string> { Data = null, Message = "Unauthorized", Success = false });

            var account = await _accountService.Create(model);
            return Ok(account);
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Update an account.")]
        [SwaggerResponse(200, "Account updated.", typeof(ServiceResponse<AccountResponse>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> Update(int id, UpdateRequest model)
        {
            if(Account == null)
                return Unauthorized(new { message = "Unauthorized" });
            // users can update their own account and admins can update any account
            if (id != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            // only admins can update role
            if (Account.Role != Role.Admin)
                model.Role = null;

            var account = await _accountService.Update(id, model);
            return Ok(account);
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete an account.")]
        [SwaggerResponse(200, "Account deleted.", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            if (Account == null)
                return Unauthorized(new { message = "Unauthorized" });
            // users can delete their own account and admins can delete any account
            if (id != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var response = await _accountService.Delete(id);
            return Ok(response);
        }

        // helper methods

        private void SetTokenCookie(string? token)
        {
            if (token == null) return;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
