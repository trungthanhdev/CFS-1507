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

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// ------- DI --------
builder.Services.AddInjection(builder.Configuration);
builder.Services.AddAuthorization(options =>
{

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
                    (Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!))
    };
});

builder.Services.AddAuthorization();
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
app.Run("http://0.0.0.0:5555");
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
// var imagePath = "/data/Uploads";

// if (!Directory.Exists(imagePath))
// {
//     Directory.CreateDirectory(imagePath); // đảm bảo tồn tại khi chạy lần đầu
// }
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("/data/Uploads"),
    RequestPath = "/images"
});
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
// ------ map endpoints -------
new AuthEndpoint().MapEndpoints(app);
new ProductEndpoint().MapEndpoints(app);
// ----------------------------
app.Run();

