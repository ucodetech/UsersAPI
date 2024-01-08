using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Data;
using UsersAPI.Models;

namespace UsersAPI.Services;

public class RefreshHandler : IRefreshHandler
{
    private readonly UserApiDbContext _db;
    public RefreshHandler(UserApiDbContext db)
    {
        _db = db;
    }
    
    public async Task<string> GenerateToken(string username)
    {
        var randomNumber = new byte[32];
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);
            var ExistingToken = await _db.RefreshTokens.FirstOrDefaultAsync(t=>t.userId==username);
            if (ExistingToken != null)
            {
                ExistingToken.refreshToken = refreshToken;
               
            }
            else
            {
                await _db.RefreshTokens.AddAsync(new RefreshToken
                {
                    userId = username,
                    tokenId = new Random().Next().ToString(),
                    refreshToken = refreshToken
                });
            }
            await _db.SaveChangesAsync();
            return refreshToken;
        }
    }
}