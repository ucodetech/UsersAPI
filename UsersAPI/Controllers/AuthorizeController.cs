using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UsersAPI.Data;
using UsersAPI.Modal;
using UsersAPI.Models;
using UsersAPI.Services;

namespace UsersAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeController : Controller
{
    private readonly UserApiDbContext _db;
    private readonly JWTSettings _jwtSettings;
    private readonly IRefreshHandler _refreshHandler;
    public AuthorizeController(UserApiDbContext db, IOptions<JWTSettings> options, IRefreshHandler refreshHandler)
    {
        _db = db;
        _jwtSettings = options.Value;
        _refreshHandler = refreshHandler;
    }

    [HttpPost("GenerateToken")]
    public async Task<ActionResult> GenerateToken([FromBody] UserCred userCred)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u=>u.Username==userCred.Username && u.Password==userCred.Password); 
        if (user != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.Secretkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var finalToken = tokenHandler.WriteToken(token);
            return Ok( new TokenResponse
            {
                Token = finalToken,
                RefreshToken = await _refreshHandler.GenerateToken(userCred.Username)
            });

        }
        else
        {
            return Unauthorized();
        }
       
    }   
    
    [HttpPost("GenerateRefreshToken")]
    public async Task<ActionResult> GenerateToken([FromBody] TokenResponse tokenResponse)
    {
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(item=>item.refreshToken == tokenResponse.RefreshToken); 
        if (refreshToken != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.Secretkey);
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(tokenResponse.Token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                ValidateIssuer = false,
                ValidateAudience = false, 
            }, out securityToken);
            var _token = securityToken as JwtSecurityToken;
            if (_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                string? username = principal.Identity?.Name;
                var existingData =  await _db.RefreshTokens.FirstOrDefaultAsync(u => u.userId == username && u.refreshToken==tokenResponse.RefreshToken);
                if (existingData != null)
                {
                    var newToken = new JwtSecurityToken(
                              claims:principal.Claims.ToArray(),
                              expires:DateTime.Now.AddSeconds(30),
                              signingCredentials: new SigningCredentials( new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secretkey)), SecurityAlgorithms.HmacSha256)
                        );
                    var finalToken = tokenHandler.WriteToken(newToken);
                    return Ok( new TokenResponse
                    {
                        Token = finalToken,
                        RefreshToken = await _refreshHandler.GenerateToken(username)
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
           
        }
        else
        {
            return Unauthorized();
        }
       
    }
}