using AppManageTasks.Data;
using AppManageTasks.Module;
using AppManageTasks.Services.BackgroundServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(MapperProfile.Profiles);

// Module
AppManageTasksModule.ConfigureService(builder.Services, builder.Configuration);

// Background Service
builder.Services.AddHostedService<OverdueTaskBackgroundService>();
builder.Services.AddScoped<OverdueTaskBackgroundService>();

// Controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// MemoryCache
builder.Services.AddMemoryCache();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AppManageTasks API",
        Version = "v1",
        Description = "API для управления задачами"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppManageTasks API v1");
    c.RoutePrefix = "swagger";
});

if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();