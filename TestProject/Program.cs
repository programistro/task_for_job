using FluentMigrator.Runner;
using FluentValidation;
using Npgsql;
using TestProject.Migrations;
using TestProject.Repositories;
using TestProject.Services;
using TestProject.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.AddFluentMigratorCore();
builder.Services.ConfigureRunner(runner => runner
    .AddPostgres()
    .WithGlobalConnectionString(connectionString)
    .WithMigrationsIn(typeof(CreateElementsTable).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<InjectionPayloadDtoValidator>();
builder.Services.AddControllers();
builder.Services.AddScoped<IParseService, ParseService>();
builder.Services.AddSingleton<IEmailExtractor, EmailExtractor>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IElementRepository, ElementRepository>();

// Lowercase every generated route, so [Route("[controller]")] becomes /elements, /parser.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply pending migrations on startup.
using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    migrator.MigrateUp();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();