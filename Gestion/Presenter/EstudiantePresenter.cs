using System;
using MVP_Estudiante_Ejemplo.Model;
using MVP_Estudiante_Ejemplo.View;

namespace MVP_Estudiante_Ejemplo.Presenter
{
    public class EstudiantePresenter
    {
        private IEstudianteView _view;
        private EstudianteRepository _model; // El Presentador tiene una referencia al Modelo

        // El constructor recibe la Vista (a través de su interfaz)
        public EstudiantePresenter(IEstudianteView view)
        {
            _view = view;
            _model = new EstudianteRepository(); // Instanciamos nuestro "repositorio" del Modelo

            // Suscribimos los eventos de la Vista a nuestros métodos del Presentador
            _view.CargarEstudianteClick += OnCargarEstudianteClick;
            _view.GuardarEstudianteClick += OnGuardarEstudianteClick;
        }

        // Manejador del evento CargarEstudianteClick de la Vista
        private void OnCargarEstudianteClick(object sender, EventArgs e)
        {
            // El Presentador le pide al Modelo los datos
            Estudiante estudiante = _model.ObtenerEstudiante(1); // Siempre obtenemos el ID 1 en este ejemplo

            if (estudiante != null)
            {
                // El Presentador actualiza la Vista con los datos del Modelo
                _view.NombreEstudiante = estudiante.Nombre;
                _view.ApellidoEstudiante = estudiante.Apellido;
                _view.EdadEstudiante = estudiante.Edad.ToString();
                _view.MostrarMensaje("Datos del estudiante cargados.");
            }
            else
            {
                _view.MostrarMensaje("No se pudo cargar el estudiante.");
            }
        }

        // Manejador del evento GuardarEstudianteClick de la Vista
        private void OnGuardarEstudianteClick(object sender, EventArgs e)
        {
            // El Presentador obtiene los datos de la Vista
            string nombre = _view.NombreEstudiante;
            string apellido = _view.ApellidoEstudiante;
            
            // Validaciones básicas de la lógica de presentación
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                _view.MostrarMensaje("Nombre y apellido no pueden estar vacíos.");
                return;
            }

            int edad;
            if (!int.TryParse(_view.EdadEstudiante, out edad) || edad < 0)
            {
                _view.MostrarMensaje("Edad inválida. Debe ser un número positivo.");
                return;
            }

            // Creamos un nuevo objeto Estudiante con los datos de la Vista
            Estudiante estudiante = new Estudiante(1, nombre, apellido, edad); // Siempre ID 1 para este ejemplo

            // El Presentador le dice al Modelo que guarde los datos
            _model.GuardarEstudiante(estudiante);

            // El Presentador actualiza la Vista con un mensaje de éxito
            _view.MostrarMensaje("Estudiante guardado exitosamente!");
        }
    }
}