using System.Text;
using System.Threading.RateLimiting;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using UsersAPI.Data;
using UsersAPI.Helper;
using UsersAPI.Modal;
using UsersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<UserApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserApiConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRefreshHandler, RefreshHandler>();
builder.Services.AddControllers();

var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);
//specified cors policy
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
    {
        build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }
));
// default cors policy
builder.Services.AddCors(p => p.AddDefaultPolicy( build =>
    {
        build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }
));
//Rate limiter
builder.Services.AddRateLimiter(f => f.AddFixedWindowLimiter(policyName: "WindowFixedLimiter", options =>
{
    //configuration options
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}));
//
// builder.Services.AddAuthentication("BasicAuthentication")
//     .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("BasicAuthentication", null);

var authkey = builder.Configuration.GetValue<string>("JWT_Setting:SecretKey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

//file logger using Serilog log
string? logPath = builder.Configuration.GetSection("Logging:LogPath").Value;
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logPath)
    .CreateLogger();
var jwtSetting = builder.Configuration.GetSection("JWT_Setting");
builder.Services.Configure<JWTSettings>(jwtSetting);


builder.Logging.AddSerilog(logger);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// enable rate limiter
app.UseRateLimiter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // to use a specified cors policy add the policy name ("name")
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();