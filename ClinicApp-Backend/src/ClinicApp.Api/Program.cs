using ClinicApp.Application.Services;
using ClinicApp.Application.Services.Implementations;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Security;
using ClinicApp.Infrastructure.Persistence;
using ClinicApp.Infrastructure.Persistence.Repositories;
using ClinicApp.Infrastructure.Security;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ===== Configurar Logging com Serilog =====
var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs");
if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(logsPath);
}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(Path.Combine(logsPath, "clinic-app-.txt"), 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ===== Configurar Services =====

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinic App API",
        Version = "v1",
        Description = "API para Gestão de Agendamentos de Clínica",
        Contact = new OpenApiContact
        {
            Name = "Clinic App",
            Email = "contato@clinicapp.com"
        }
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// ===== Autenticação JWT =====
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey não configurado");
var issuer = jwtSettings["Issuer"] ?? "ClinicApp";
var audience = jwtSettings["Audience"] ?? "ClinicAppUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ===== Injeção de Dependências =====

// Infrastructure
builder.Services.AddScoped<IDbConnectionFactory, SqlServerConnectionFactory>();

// Repositórios
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IHealthProfessionalRepository, HealthProfessionalRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Serviços da Aplicação
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IHealthProfessionalService, HealthProfessionalService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IInsuranceProviderService, InsuranceProviderService>();

// Segurança
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IJwtTokenService>(sp => new JwtTokenServiceAdapter(secretKey, issuer, audience));
builder.Services.AddScoped<ClinicApp.Domain.Security.IPasswordHasher, PasswordHasher>();

var app = builder.Build();

// ===== Inicializar Banco de Dados =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using (var connection = connectionFactory.CreateConnection())
        {
            connection.Open();
            Log.Information("✅ Conexão com banco de dados estabelecida com sucesso");
        }
    }
    catch (Exception ex)
    {
        Log.Warning("⚠️ Erro ao conectar ao banco: {Message}. A aplicação continuará, mas o banco pode não estar disponível.", ex.Message);
    }
}

// ===== Middleware =====

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

// Middleware de tratamento global de erros
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Ocorreu um erro não tratado na requisição {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        var message = "Ocorreu um erro interno no servidor.";
        var statusCode = 500;

        // Tratar erros específicos de banco de dados (SQL Server)
        if (ex.Message.Contains("UNIQUE KEY") || ex.Message.Contains("duplicate key"))
        {
            statusCode = 400;
            if (ex.Message.Contains("Patients") && ex.Message.Contains("Cpf"))
                message = "Já existe um paciente cadastrado com este CPF.";
            else if (ex.Message.Contains("Patients") && ex.Message.Contains("Email"))
                message = "Já existe um paciente cadastrado com este E-mail.";
            else if (ex.Message.Contains("Users") && ex.Message.Contains("Username"))
                message = "Este nome de usuário já está sendo utilizado.";
            else
                message = "Já existe um registro com estes dados no sistema.";
        }
        else if (ex is ClinicApp.Domain.Exceptions.BusinessRuleException || ex is ClinicApp.Domain.Exceptions.InvalidDataException)
        {
            statusCode = 400;
            message = ex.Message;
        }
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { 
            Success = false, 
            Message = message 
        });
    }
});

// Habilitar Swagger em todos os ambientes quando estiver rodando no Docker ou Desenvolvimento
// Isso permite ver a documentação mesmo em "Produção" local (Docker)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic App API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz (/)
});

if (!app.Environment.IsDevelopment())
{
    // Apenas habilitar Redirecionamento HTTPS se não estiver rodando em container (opcional)
    // No Docker, o SSL geralmente é tratado por um Proxy Reverso (Nginx) ou não é necessário localmente
    // app.UseHttpsRedirection(); 
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check simples
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

try
{
    Log.Information("Iniciando aplicação ClinicApp...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação terminou de forma inesperada");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}
