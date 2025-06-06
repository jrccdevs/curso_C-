/*. Configuración del Proyecto
Necesitarás dos proyectos separados: uno para el backend (C#) y otro para el frontend (Flutter).

Backend (ASP.NET Core MVC)
Crear el Proyecto: Abre Visual Studio 2022.
Crea un nuevo proyecto.
Busca "Aplicación web ASP.NET Core" o "ASP.NET Core Web API". (Aunque dice "Web API", también podemos usar el patrón MVC para la API).
Selecciona "API" como plantilla.
Nombre del proyecto: EmpleadosApiBackend
Framework: Elige .NET 8.0 (o la versión más reciente que tengas).
Desmarca "Usar controladores (para crear API)". No, en este caso vamos a usar un controlador manual.
Desmarca "Habilitar OpenAPI support".
Marcar "Usar controlador".
Marcar "Habilitar Docker"
Marcar "Usar https"
Marcar "Habilitar autenticación"
Frontend (Flutter)
Crear el Proyecto: Abre tu terminal o línea de comandos.
Navega a la ubicación donde quieres crear el proyecto.
Ejecuta: flutter create empleados_frontend
Navega a la carpeta del proyecto: cd empleados_frontend
2. Backend: ASP.NET Core MVC (C#)
Vamos a simular una base de datos en memoria para mantener el enfoque en la arquitectura MVC y la API.

2.1. El Modelo (M de MVC)
Define la estructura de tus datos.

Models/Empleado.cs*/
namespace EmpleadosApiBackend.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } // Nullable para flexibilidad
        public string? Apellido { get; set; }
        public string? Cargo { get; set; }
        public decimal Salario { get; set; }
    }
}

/*2.2. Simulación de Datos (Repositorio)
Esto reemplaza la conexión real a la base de datos por simplicidad. En una aplicación real, usarías Entity Framework Core con SQL Server, PostgreSQL, etc.

Data/EmpleadoRepository.cs*/
using EmpleadosApiBackend.Models;
using System.Collections.Generic;
using System.Linq;

namespace EmpleadosApiBackend.Data
{
    public class EmpleadoRepository
    {
        private static List<Empleado> _empleados = new List<Empleado>
        {
            new Empleado { Id = 1, Nombre = "Ana", Apellido = "García", Cargo = "Desarrollador Senior", Salario = 60000.00m },
            new Empleado { Id = 2, Nombre = "Carlos", Apellido = "López", Cargo = "Gerente de Proyectos", Salario = 75000.00m },
            new Empleado { Id = 3, Nombre = "María", Apellido = "Fernández", Cargo = "Diseñador UI/UX", Salario = 55000.00m }
        };
        private static int _nextId = 4; // Para nuevos empleados

        public IEnumerable<Empleado> GetAll()
        {
            return _empleados;
        }

        public Empleado? GetById(int id)
        {
            return _empleados.FirstOrDefault(e => e.Id == id);
        }

        public Empleado Add(Empleado newEmpleado)
        {
            newEmpleado.Id = _nextId++;
            _empleados.Add(newEmpleado);
            return newEmpleado;
        }

        public bool Update(Empleado updatedEmpleado)
        {
            var existingEmpleado = _empleados.FirstOrDefault(e => e.Id == updatedEmpleado.Id);
            if (existingEmpleado == null)
            {
                return false;
            }

            existingEmpleado.Nombre = updatedEmpleado.Nombre;
            existingEmpleado.Apellido = updatedEmpleado.Apellido;
            existingEmpleado.Cargo = updatedEmpleado.Cargo;
            existingEmpleado.Salario = updatedEmpleado.Salario;
            return true;
        }

        public bool Delete(int id)
        {
            var empleadoToRemove = _empleados.FirstOrDefault(e => e.Id == id);
            if (empleadoToRemove == null)
            {
                return false;
            }
            _empleados.Remove(empleadoToRemove);
            return true;
        }
    }
}

/*2.3. El Controlador (C de MVC)
Este será un Controlador de API que manejará las solicitudes HTTP del frontend y devolverá respuestas JSON.

Controllers/EmpleadosController.cs*/
using EmpleadosApiBackend.Data;
using EmpleadosApiBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EmpleadosApiBackend.Controllers
{
    // Indica que este controlador es una API web
    [ApiController]
    // Define la ruta base para este controlador (ej. /api/empleados)
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadoRepository _repository;

        // Inyección de dependencias para el repositorio
        public EmpleadosController(EmpleadoRepository repository)
        {
            _repository = repository;
        }

        // GET: api/empleados
        [HttpGet]
        public ActionResult<IEnumerable<Empleado>> GetEmpleados()
        {
            return Ok(_repository.GetAll());
        }

        // GET: api/empleados/{id}
        [HttpGet("{id}")]
        public ActionResult<Empleado> GetEmpleado(int id)
        {
            var empleado = _repository.GetById(id);
            if (empleado == null)
            {
                return NotFound(); // Código 404 si no se encuentra el empleado
            }
            return Ok(empleado); // Código 200 y el empleado
        }

        // POST: api/empleados
        [HttpPost]
        public ActionResult<Empleado> PostEmpleado([FromBody] Empleado empleado)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(empleado.Nombre) || string.IsNullOrWhiteSpace(empleado.Apellido))
            {
                return BadRequest("Nombre y Apellido son requeridos.");
            }
            if (empleado.Salario < 0)
            {
                return BadRequest("El salario no puede ser negativo.");
            }

            _repository.Add(empleado);
            // Devuelve el empleado creado con su nuevo ID y una URL para acceder a él
            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
        }

        // PUT: api/empleados/{id}
        [HttpPut("{id}")]
        public IActionResult PutEmpleado(int id, [FromBody] Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");
            }

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(empleado.Nombre) || string.IsNullOrWhiteSpace(empleado.Apellido))
            {
                return BadRequest("Nombre y Apellido son requeridos.");
            }
            if (empleado.Salario < 0)
            {
                return BadRequest("El salario no puede ser negativo.");
            }

            if (!_repository.Update(empleado))
            {
                return NotFound(); // No se encontró el empleado para actualizar
            }
            return NoContent(); // Código 204, éxito sin contenido de retorno
        }

        // DELETE: api/empleados/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEmpleado(int id)
        {
            if (!_repository.Delete(id))
            {
                return NotFound(); // No se encontró el empleado para eliminar
            }
            return NoContent(); // Código 204, éxito sin contenido de retorno
        }
    }
}

/*2.4. Configuración (Program.cs)
Necesitas registrar el EmpleadoRepository para la inyección de dependencias.

Program.cs (Asegúrate de que WebApplication.CreateBuilder(args) ya está ahí)*/
using EmpleadosApiBackend.Data; // Importa el namespace de tu repositorio

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Agrega soporte para controladores
// Aprende más sobre la configuración de Swagger/OpenAPI en https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar el EmpleadoRepository como un servicio Singleton (se crea una sola instancia para toda la aplicación)
builder.Services.AddSingleton<EmpleadoRepository>();

// Configuración de CORS para permitir que Flutter acceda a la API
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin() // Permite cualquier origen (para desarrollo)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirige HTTP a HTTPS
app.UseCors(); // Usa la política CORS definida

app.UseAuthorization(); // Para autorización si la usaras

app.MapControllers(); // Mapea los controladores a las rutas HTTP

app.Run();


/*2.5. Ejecutar el Backend
Presiona F5 en Visual Studio. Esto iniciará el servidor web Kestrel.
Deberías ver una ventana de Swagger/OpenAPI en tu navegador (si dejaste el soporte habilitado), donde puedes probar tus endpoints de API.
Nota importante: La API se ejecutará en una URL como https://localhost:70XX (donde XX es un número de puerto asignado). Necesitarás esta URL para el frontend de Flutter. Por lo general, cuando ejecutas desde Visual Studio, también te muestra la URL.
*/
