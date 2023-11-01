using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RoleBasedAuthWithJWT.Helpers;
using RoleBasedAuthWithJWT.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var services = builder.Services;

// configure strongly typed settings objects
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
services.Configure<AppSettings>(appSettingsSection);

// configure jwt authentication
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// configure DI for application services
services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware gets called when response status code is between 400 - 599
app.UseStatusCodePages(async statusCodeContext =>
{
    if (statusCodeContext.HttpContext.Response.StatusCode == 401)
    {
        await statusCodeContext.HttpContext.Response.WriteAsync("UnAuthorized !!!");
    }

    if (statusCodeContext.HttpContext.Response.StatusCode == 403)
    {
        await statusCodeContext.HttpContext.Response.WriteAsync("Forbidden !!!");
    }
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
