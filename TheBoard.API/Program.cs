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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.Use(async (context, next) =>
//{
//    var token = context.Request.Cookies[GetEnvironmentVariable("JWT_ACCESS_COOKIE_NAME")];
//    if (!string.IsNullOrEmpty(token))
//        context.Request.Headers.Add("Authorization", "Bearer " + token);

//    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
//    context.Response.Headers.Add("X-Xss-Protection", "1");
//    context.Response.Headers.Add("X-Frame-Options", "DENY");

//    await next();
//});

app.UseAccessToken();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
