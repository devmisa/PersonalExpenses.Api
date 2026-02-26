using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PersonalExpenses.Api.Middlewares;
using PersonalExpenses.Api.Services;
using PersonalExpenses.Application.Context;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Application.Services;
using PersonalExpenses.Application.Validations;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Infrastructure.Data;
using PersonalExpenses.Infrastructure.Extensions;
using PersonalExpenses.Infrastructure.Interfaces;
using PersonalExpenses.Infrastructure.Repositories;
using PersonalExpenses.Security.Interfaces;
using PersonalExpenses.Security.Passwords;
using PersonalExpenses.Security.Services;
using PersonalExpenses.Security.Settings;
using System.Text;
using System.Threading.RateLimiting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

JwtSettings? jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var builderConnection = new SqliteConnectionStringBuilder(connectionString);
string dbPath = builderConnection.DataSource;

string fullPath = Path.Combine(builder.Environment.ContentRootPath, dbPath);
Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);


//Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.ConfigureSqliteOptions(connectionString));

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<CreateExpenseRequest>, ExpenseRequestValidator<CreateExpenseRequest>>();
builder.Services.AddScoped<IValidator<UpdateExpenseRequest>, ExpenseRequestValidator<UpdateExpenseRequest>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddSingleton<ExceptionToProblemDetailsService>();
builder.Services.AddSingleton(jwtSettings);

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        _ = policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000") // Frontend URLs //TODO: editar
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

//Configurar Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    _ = options.AddFixedWindowLimiter("login", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(15);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    _ = options.AddFixedWindowLimiter("general", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience
    };
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT"
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});


WebApplication app = builder.Build();

try
{
    using IServiceScope scope = app.Services.CreateScope();
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Erro ao aplicar migrações do banco de dados");
    throw;
}



// Configure the HTTP request pipeline.
// Registrar middleware de tratamento global de exceções
_ = app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapControllers();

app.Run();
