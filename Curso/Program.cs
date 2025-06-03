using Curso.Controller;
using System;

namespace Curso
{
    class Program
    {
        static void Main(string[] args)
        {
            EstudianteController controlador = new();

            Console.WriteLine("🎓 Sistema de Gestión de Estudiantes - MVC\n");

            controlador.AgregarEstudiante(1, "Carlos Pérez", "Ingeniería", 27);
            controlador.AgregarEstudiante(2, "María Gómez", "Medicina", 30);

            controlador.MostrarEstudiantes();

            Console.WriteLine("\n✅ Fin del programa.");
        }
    }
}
