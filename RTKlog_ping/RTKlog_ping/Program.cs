using RTKlog_ping.DataContext.cs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RTKlog_ping.Models;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.SqlServer;
using Serilog;
using RTKlog_ping.Utils;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQL Server
//builder.Services.AddDbContext<DataContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHttpLogging(options =>
{
    options.CombineLogs = true;
});

//C:\Users\INNOVADMIN\Desktop

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<RequestLoggingMiddleware>();
app.Run();
