using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Accounts;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<ServiceResponse<AuthenticateResponse>> RefreshToken(string? token, string? ipAddress);
        Task<ServiceResponse<string>> RevokeToken(string token, string ipAddress);
        Task<ServiceResponse<AccountResponse>> Register(RegisterRequest model);
        Task<ServiceResponse<string>> VerifyEmail(string token);
        Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordRequest model);
        Task<ServiceResponse<AccountModel>> ValidateResetToken(ValidateResetTokenRequest model);
        Task<ServiceResponse<AccountModel>> ResetPassword(ResetPasswordRequest model);
        Task<ServiceResponse<IEnumerable<AccountResponse>>> GetAll();
        Task<ServiceResponse<AccountResponse>> GetById(string id);
        Task<ServiceResponse<AccountResponse>> Create(CreateRequest model);
        Task<ServiceResponse<AccountResponse>> Update(string id, UpdateRequest model);
        Task<ServiceResponse<string>> Delete(string id);
    }
}
