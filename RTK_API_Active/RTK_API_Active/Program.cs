using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RTK_API_Active.DBcontex;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Obtener cadena de conexión desde appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "cadena de conexion no encontrada";

// 🔹 Verificar manualmente la conexión antes de iniciar la aplicación
try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Conexión exitosa a la base de datos.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($" Error de conexión: {ex.Message}");
    Environment.Exit(1); // Salir de la aplicación si la conexión falla
}

// 🔹 Configurar DbContext con la cadena de conexión
builder.Services.AddDbContext<EquipmentControlContext>(options =>
    options.UseSqlServer(connectionString));

// 🔹 Agregar servicios de controladores
builder.Services.AddControllers();

// 🔹 Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Activity",
        Version = "v1",
        Description = "API para gestionar datos de equipos",
        
    });
});

var app = builder.Build();

// 🔹 Habilitar Swagger solo en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.RoutePrefix = string.Empty; // Permite abrir Swagger UI en la raíz
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();

