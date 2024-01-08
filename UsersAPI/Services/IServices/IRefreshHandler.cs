namespace UsersAPI.Services;

public interface IRefreshHandler
{
    Task<string> GenerateToken(string username);
}