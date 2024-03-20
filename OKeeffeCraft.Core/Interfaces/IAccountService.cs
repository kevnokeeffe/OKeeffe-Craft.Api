using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Accounts;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponse<AuthenticateResponse>> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<ServiceResponse<AuthenticateResponse>> RefreshToken(string? token, string? ipAddress);
        Task<ServiceResponse<string>> RevokeToken(string token, string ipAddress);
        Task<ServiceResponse<AccountModel>> Register(RegisterRequest model, string origin);
        Task<ServiceResponse<string>> VerifyEmail(string token);
        Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<ServiceResponse<AccountModel>> ValidateResetToken(ValidateResetTokenRequest model);
        Task<ServiceResponse<AccountModel>> ResetPassword(ResetPasswordRequest model);
        Task<ServiceResponse<IEnumerable<AccountResponse>>> GetAll();
        Task<ServiceResponse<AccountResponse>> GetById(int id);
        Task<ServiceResponse<AccountResponse>> Create(CreateRequest model);
        Task<ServiceResponse<AccountResponse>> Update(int id, UpdateRequest model);
        Task<ServiceResponse<string>> Delete(int id);
    }
}
