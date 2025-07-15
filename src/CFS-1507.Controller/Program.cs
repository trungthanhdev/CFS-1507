using CFS_1507.Application;
using CFS_1507.Injection;
using CFS_1507.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ------- DI --------
// builder.Services.AddInfrastructure();
// builder.Services.InjectApplication();
builder.Services.AddInjection(builder.Configuration);
// -------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

