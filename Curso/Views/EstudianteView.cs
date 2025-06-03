using Curso.Models;
using System;
using System.Collections.Generic;

namespace Curso.Views
{
    public class EstudianteView
    {
        public void MostrarLista(List<Estudiante> estudiantes)
        {
            Console.WriteLine("\n📚 Lista de Estudiantes:");

           
            foreach (var est in estudiantes)
            {
                Console.WriteLine($"ID: {est.Id}, Nombre: {est.Nombre}, Carrera: {est.Carrera}, Años: {est.Anio}");
            }
        }
    }
}
