using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de los servicios
builder.Services.AddControllers(); // Necesario para usar los controladores

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pokémon API", Version = "v1" });
});

var app = builder.Build();

// Habilitar la documentación Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilita la generación del archivo Swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokémon API v1");
        c.RoutePrefix = string.Empty;  // Configura Swagger UI para que se cargue en la raíz (http://localhost:5001)
    });
}

// Configuración de las rutas de los controladores
app.MapControllers();

// Ejecutar la aplicación
app.Run();
