using AuthService.Data;
using AuthService.Auth;
using AuthService.Services; // 👈 Asegúrate de tener esta carpeta
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controladores y endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger con soporte JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "Microservicio de autenticación para el sistema de inventario hospitalario (NeuroByte)"
    });

    // 🔒 Config para el botón Authorize (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce tu token JWT aquí (sin 'Bearer ' al inicio)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Conexión a PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

// ✅ Servicio JWT
builder.Services.AddSingleton<JwtService>();

// ✅ Servicio de envío de correos
builder.Services.AddScoped<EmailService>(); // 👈 nuevo

// ✅ Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
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
            )
        };
    });

// ✅ CORS para permitir peticiones del frontend (React en localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // 👈 puerto del front
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 👇 Orden correcto del pipeline
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "🚀 AuthService API funcionando correctamente con PostgreSQL + React frontend");

app.Run();
