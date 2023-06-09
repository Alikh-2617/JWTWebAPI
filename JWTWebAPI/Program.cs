using JWTWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

 // put option to the sweggare for token 
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Oauth", new OpenApiSecurityScheme
    {
        Description = "Bearer schema (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authentication",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();  // pakage : swashbuckle.aspnetcore.filters
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // for ValidateIssuer = false, ValidateAudience = false, we can use them later !! with SSL host url
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSetting:TokenKey").Value)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddScoped <IAuthService,AuthService>(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();  // use the Aythentication ! 

app.UseAuthorization();

app.MapControllers();

app.Run();
