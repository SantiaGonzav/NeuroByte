using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RequestService.Data;
using RequestService.HttpClients;
using RequestService.HttpClients.Auth;
using RequestService.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// -----------------------------------------
// Mostrar información detallada de errores JWT (solo dev)
// -----------------------------------------
IdentityModelEventSource.ShowPII = true;

// -----------------------------------------
// 🔥 Mantener claims estándar de Microsoft
// -----------------------------------------
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Restaurar mapeos NECESARIOS
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Sub] = ClaimTypes.NameIdentifier;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.UniqueName] = ClaimTypes.Name;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[JwtRegisteredClaimNames.Email] = ClaimTypes.Email;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 1. Config JWT
// ===============================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) ||
    string.IsNullOrEmpty(jwtIssuer) ||
    string.IsNullOrEmpty(jwtAudience))
{
    throw new Exception("❌ Configuración JWT incompleta en appsettings.json");
}

// ===============================
// 2. Base de datos
// ===============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===============================
// 3. Autenticación JWT
// ===============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            // Claims internos
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"❌ JWT ERROR: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },

            OnTokenValidated = ctx =>
            {
                var id = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = ctx.Principal?.FindFirst(ClaimTypes.Name)?.Value;
                var role = ctx.Principal?.FindFirst(ClaimTypes.Role)?.Value;

                Console.WriteLine($"🔑 TOKEN VÁLIDO → ID:{id} | User:{username} | Rol:{role}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ===============================
// 4. HttpClients
// ===============================
builder.Services.AddHttpClient<IEquipClient, EquipClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:EquipServiceUrl"]);
});

builder.Services.AddHttpClient<IAuthClient, AuthClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:AuthServiceUrl"]);
});

// ===============================
// 5. Servicios internos
// ===============================
builder.Services.AddScoped<IEmailService, EmailService>();

// ===============================
// 6. Swagger + JWT
// ===============================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RequestService API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
            },
            Array.Empty<string>()
        }
    });
});

// ===============================
// 7. CORS
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

// ===============================
// 8. APP
// ===============================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
