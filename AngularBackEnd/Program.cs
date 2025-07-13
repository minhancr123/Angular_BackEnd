using JeeBeginner.Reponsitories.AccountManagement;
using JeeBeginner.Reponsitories.Authorization;
using JeeBeginner.Reponsitories.CustomerManagement;
using JeeBeginner.Reponsitories.PartnerManagement;
using JeeBeginner.Services;
using JeeBeginner.Services.AccountManagement;
using JeeBeginner.Services.Authorization;
using JeeBeginner.Services.CustomerManagement;
using JeeBeginner.Services.PartnerManagement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

// CORS
builder.Services.AddCors(o => o.AddPolicy("AllowOrigin", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Controller + JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

// JWT Authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
    };
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {your JWT}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddOptions();

// Repositories & Services
builder.Services.AddTransient<IAccountManagementRepository, AccountManagementRepository>();
builder.Services.AddTransient<IPartnerManagementRepository, PartnerManagementRepository>();
builder.Services.AddTransient<IAuthorizationRepository, AuthorizationRepository>();
builder.Services.AddTransient<ICustomerManagementRepository, CustomerManagementRepository>();

builder.Services.AddTransient<IPartnerManagementService, PartnerManagementService>();
builder.Services.AddTransient<IAccountManagementService, AccountManagementService>();
builder.Services.AddTransient<ICustomAuthorizationService, CustomAuthorizationService>();
builder.Services.AddTransient<ICustomerManagementService, CustomerManagementService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); // 👈 Phải có dòng này
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JeeBeginner v1"));
}

app.UseRouting();
app.UseCors("AllowOrigin");

app.UseAuthentication(); // 👈 Phải có nếu dùng JWT
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();
app.Run();
