using System.Reflection;
using System.Text;
using CompanyAuth.Application.Interfaces.Repositories;
using CompanyAuth.Application.Interfaces.Services;
using CompanyAuth.Infrastructure.Persistence;
using CompanyAuth.Infrastructure.Persistence.Seed;
using CompanyAuth.Infrastructure.Repositories;
using CompanyAuth.Infrastructure.Security;
using CompanyAuth.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// MVC Controllers
// ===========================
builder.Services.AddControllers();

// ===========================
// DbContext (PostgreSQL)
// ===========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===========================
// JWT Settings
// ===========================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// ===========================
// Repositorios
// ===========================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();

// ===========================
// Servicios de aplicaci�n
// ===========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// ===========================
// Swagger + JWT
// ===========================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Info b�sica
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CompanyAuth API",
        Version = "v1",
        Description = "API para autenticaci�n, roles, permisos y autorizaci�n"
    });

    // XML comments (opcional, pero �til)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Security Scheme (Bearer)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Escribe: Bearer {tu_token_jwt}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// ===========================
// JWT Authentication
// ===========================
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("La secci�n 'Jwt' no est� configurada en appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // opcional, para que caduque exacto
    };
});

// ===========================
// Authorization Policies
// ===========================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewUsers", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var permissions = context.User.FindAll("permission").Select(c => c.Value);
            return permissions.Contains("ViewUsers") || context.User.IsInRole("Admin");
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===========================
// Middleware pipeline
// ===========================
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CompanyAuth API v1");
    });
// }

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ===========================
// Seed inicial (Roles, Permisos, Admin, etc.)
// ===========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await InitialDataSeeder.SeedAsync(context);
}

// ===========================
// Map Controllers
// ===========================
app.MapControllers();

app.Run();