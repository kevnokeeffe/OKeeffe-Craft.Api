namespace OKeeffeCraft.Authorization.Interfaces
{
    public interface IAuthIdentityService
    {
        bool HasRole(string role);
        string? GetAccountId();
    }
}
