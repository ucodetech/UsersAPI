using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsersAPI.Data;

namespace UsersAPI.Helper;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserApiDbContext _db;
    
    public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, UserApiDbContext db) : base(options, logger, encoder, clock)
    {
        _db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("No headers found!");
        }

        var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        if (headerValue != null)
        {
            var bytes = Convert.FromBase64String(headerValue.Parameter);
            string credentials = Encoding.UTF8.GetString(bytes);
            string[] array = credentials.Split(":");
            string username = array[0];
            string password = array[1];
             var user = await _db.Users.FirstOrDefaultAsync(u=>u.Username==username && u.Password==password);
             if (user != null )
             {
                 var claim = new[] { new Claim(ClaimTypes.Name, user.Username) };
                 var identity = new ClaimsIdentity(claim, Scheme.Name);
                 var principal = new ClaimsPrincipal(identity);
                 var ticket = new AuthenticationTicket(principal, Scheme.Name);
                 return AuthenticateResult.Success(ticket);
             }
             else
             {
                 return AuthenticateResult.Fail("UnAuthorized");
             }
        }
        else
        {
            return AuthenticateResult.Fail("No Header Found");
        }
    }
}