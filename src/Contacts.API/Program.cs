using Contacts.API.Middleware;
using Contacts.API.OpenApi;
using Contacts.Application.Interfaces;
using Contacts.Application.UseCases;
using Contacts.Domain.Abstractions;
using Contacts.Infrastructure.Persistence;
using Contacts.Infrastructure.Repositories;
using Contacts.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // allowIntegerValues: false rejeita valores numéricos para enums, por exemplo sex: 0.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
    });

// Suprime o filtro automático do ModelState para que o formato de erro
// seja sempre controlado pelo middleware da solução.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Contacts API", Version = "v1" });
    options.SupportNonNullableReferenceTypes();
    options.SchemaFilter<ContactSchemaFilter>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<Contacts.Application.Validators.ContactRequestValidator>();

builder.Services.AddScoped<CreateContactUseCase>();
builder.Services.AddScoped<ListActiveContactsUseCase>();
builder.Services.AddScoped<GetActiveContactByIdUseCase>();
builder.Services.AddScoped<UpdateContactUseCase>();
builder.Services.AddScoped<DeactivateContactUseCase>();
builder.Services.AddScoped<ActivateContactUseCase>();
builder.Services.AddScoped<DeleteContactUseCase>();

var app = builder.Build();

await ApplyMigrationsAsync(app);

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

static async Task ApplyMigrationsAsync(WebApplication app)
{
    const int maxAttempts = 10;
    var delay = TimeSpan.FromSeconds(5);

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (app.Environment.IsEnvironment("Testing"))
    {
        logger.LogInformation("Skipping automatic migrations in Testing environment.");
        return;
    }

    if (!dbContext.Database.IsRelational())
    {
        logger.LogInformation("Skipping automatic migrations because the configured provider is not relational.");
        return;
    }

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            logger.LogWarning(ex, "Failed to apply migrations on attempt {Attempt}/{MaxAttempts}. Retrying in {DelaySeconds} seconds.", attempt, maxAttempts, delay.TotalSeconds);
            await Task.Delay(delay);
        }
    }

    await dbContext.Database.MigrateAsync();
}

/// <summary>
/// Tipo parcial usado pelo host da aplicação e pelos testes de integração.
/// </summary>
public partial class Program { }
