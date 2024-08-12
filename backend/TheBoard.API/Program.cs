using Microsoft.AspNetCore.Antiforgery;
using TheBoard.API;
using TheBoard.API.Middlewares;
using TheBoard.Application;
using TheBoard.Infrastructure;

// Load enviroment variables
DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
// Add services to the container.

services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

services.AddJwtAuthentication();
services.AddAntiforgery(options => { options.HeaderName = "x-xsrf-token"; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAccessToken();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();