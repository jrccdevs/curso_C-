using System;

namespace MVP_Estudiante_Ejemplo.Model
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Edad { get; set; }

        public Estudiante(int id, string nombre, string apellido, int edad)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Edad = edad;
        }

        // Lógica de negocio simple, por ejemplo, validación
        public bool EsMayorDeEdad()
        {
            return Edad >= 18;
        }
    }

    // Un "repositorio" o "servicio" que simula la interacción con la base de datos
    public class EstudianteRepository
    {
        // Simulación de una base de datos en memoria
        private Estudiante _estudianteActual;

        public EstudianteRepository()
        {
            // Inicializamos con un estudiante de ejemplo
            _estudianteActual = new Estudiante(1, "Juan", "Perez", 20);
        }

        public Estudiante ObtenerEstudiante(int id)
        {
            // En una aplicación real, esto iría a la base de datos
            if (id == _estudianteActual.Id)
            {
                return _estudianteActual;
            }
            return null; // O lanzar una excepción si no se encuentra
        }

        public void GuardarEstudiante(Estudiante estudiante)
        {
            // En una aplicación real, esto guardaría en la base de datos
            if (estudiante.Id == _estudianteActual.Id)
            {
                _estudianteActual.Nombre = estudiante.Nombre;
                _estudianteActual.Apellido = estudiante.Apellido;
                _estudianteActual.Edad = estudiante.Edad;
                Console.WriteLine($"Estudiante ID {estudiante.Id} actualizado: {estudiante.Nombre} {estudiante.Apellido}, {estudiante.Edad} años.");
            }
            else
            {
                // En un escenario real, manejar la creación de nuevos estudiantes
                Console.WriteLine("No se puede guardar un estudiante con un ID diferente en este ejemplo simple.");
            }
        }
    }
}