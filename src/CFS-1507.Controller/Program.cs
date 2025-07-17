using CFS_1507.Application;
using CFS_1507.Injection;
using CFS_1507.Infrastructure;
using CFS_1507.Infrastructure.Persistence;
// using CFS_1507.Infrastructure.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CFS_1507.Controller.Middlewares;
using CFS_1507.Controller.Endpoint;
using Microsoft.Extensions.FileProviders;
using CFS_1507.Domain.Common;
using System.Security.Claims;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.WebHost.UseUrls("http://0.0.0.0:5555");
// ------- DI --------
builder.Services.AddInjection(builder.Configuration);
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy(ERole.ADMIN.ToString(), policy =>
        policy.RequireRole(ERole.ADMIN.ToString()));

    options.AddPolicy(ERole.USER.ToString(), policy =>
        policy.RequireRole(ERole.USER.ToString()));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = Environment.GetEnvironmentVariable("Issuer"),
        ValidAudience = Environment.GetEnvironmentVariable("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!)),
        RoleClaimType = ClaimTypes.Role

    };
});

// builder.Services.AddAuthorization();
builder.Services.AddControllers();
// -------------------

// ------- logger ------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// --------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// ------ map endpoints -------
new AuthEndpoint().MapEndpoints(app);
new ProductEndpoint().MapEndpoints(app);
// ----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -------- test db connection --------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        dbContext.Database.OpenConnection();
        logger.LogInformation("Connected to Neon successfully.");
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        logger.LogError($"Database connection failed: {ex.Message}");
    }
}
// -----------------------------------

// --------- upload file ----------
var uploadPath = "/data/Uploads";
if (Directory.Exists(uploadPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider("/data/Uploads"),
        RequestPath = "/images"
    });
}
else
{
    Console.WriteLine("Warning: Upload path does not exist, static files for uploads will not be served.");
}

// --------------------------------


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
// ----- Middleware ----
app.UseMiddleware<TokenRevalidator>();
// ---------------------
app.UseAuthorization();
app.Run();

