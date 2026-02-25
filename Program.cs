using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Api.Middlewares;
using PersonalExpenses.Api.Services;
using PersonalExpenses.Application.Dtos;
using PersonalExpenses.Application.Interfaces;
using PersonalExpenses.Application.Services;
using PersonalExpenses.Application.Validations;
using PersonalExpenses.Infrastructure.Data;
using PersonalExpenses.Infrastructure.Interfaces;
using PersonalExpenses.Infrastructure.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var builderConnection = new SqliteConnectionStringBuilder(connectionString);
string dbPath = builderConnection.DataSource;

string fullPath = Path.Combine(builder.Environment.ContentRootPath, dbPath);
// Garantir que o diretório existe
Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);


//Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddScoped<IValidator<CreateExpenseRequest>, ExpenseRequestValidator<CreateExpenseRequest>>();
builder.Services.AddScoped<IValidator<UpdateExpenseRequest>, ExpenseRequestValidator<UpdateExpenseRequest>>();

builder.Services.AddSingleton<ExceptionToProblemDetailsService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Aplicar migrações e criar banco de dados automaticamente
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

app.UseAuthorization();

app.MapControllers();

app.Run();
