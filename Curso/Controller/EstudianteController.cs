using Curso.Models;
using Curso.Views;
using System.Collections.Generic;

namespace Curso.Controller
{
    public class EstudianteController
    {
        private List<Estudiante> listaEstudiantes = new();//creamos una list vacio donde agregaremos los estudiantes
        private EstudianteView vista = new EstudianteView();// creamos una instacia con el objeto EstudianteView

//creamos un metodo para agregar estudiantes
        public void AgregarEstudiante(int id, string nombre, string carrera, int anio)
        {
            // creamos una instancia nuevo con el objet estudiante
            Estudiante nuevo = new Estudiante(id, nombre, carrera, anio);
            listaEstudiantes.Add(nuevo);
        }

        public void MostrarEstudiantes()
        {
            vista.MostrarLista(listaEstudiantes);
        }
    }
}
