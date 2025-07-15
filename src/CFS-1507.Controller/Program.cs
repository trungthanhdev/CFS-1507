using CFS_1507.Application;
using CFS_1507.Injection;
using CFS_1507.Infrastructure;
using CFS_1507.Infrastructure.Persistence;
// using CFS_1507.Infrastructure.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// ------- DI --------
builder.Services.AddInjection(builder.Configuration);
// -------------------

// ------- logger ------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// --------------------


var app = builder.Build();
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


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// app.ApplyMigration();
app.Run();

