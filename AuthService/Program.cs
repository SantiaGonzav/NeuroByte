using AuthService.Data;
using AuthService.Auth;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ================================
// CONTROLADORES
// ================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ================================
// SWAGGER + JWT
// ================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "Microservicio de autenticación para NeuroByte"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Usa: Bearer {tu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// ================================
// BASE DE DATOS
// ================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// ================================
// SERVICIOS
// ================================
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<EmailService>();

// ================================
// AUTENTICACIÓN JWT
// ================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            // ================================
            // CLAIMS ESTÁNDAR (CORRECTOS)
            // ================================
            NameClaimType = ClaimTypes.Name,               // "name"
            RoleClaimType = ClaimTypes.Role               // "role"
        };

        // Logs de errores JWT
        opt.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"❌ Error JWT: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine($"🔑 JWT OK → UserID: {ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value} | Rol: {ctx.Principal?.FindFirst(ClaimTypes.Role)?.Value}");
                return Task.CompletedTask;
            }
        };
    });

// ================================
// CORS
// ================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ================================
// SWAGGER
// ================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ================================
// MIDDLEWARE
// ================================
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// ================================
// SERVIR ARCHIVOS ESTÁTICOS (IMÁGENES PERFIL)
// ================================
var profilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
if (!Directory.Exists(profilePath))
    Directory.CreateDirectory(profilePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profilePath),
    RequestPath = "/images/profiles"
});

// ================================
app.MapControllers();
app.MapGet("/", () => "🚀 AuthService funcionando correctamente");

app.Run();
