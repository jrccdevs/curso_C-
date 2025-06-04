//Model/Tarea.cs
namespace TareasApp.Model
{
    public class Tarea
    {
        public int Id { get; private set; }
        public string Descripcion { get; set; }
        public bool Completada { get; set; }

        public Tarea(int id, string descripcion)
        {
            Id = id;
            Descripcion = descripcion;
            Completada = false;
        }
    }

    public class GestorTareas
    {
        private List<Tarea> _tareas;
        private int _siguienteId;

        public GestorTareas()
        {
            _tareas = new List<Tarea>();
            _siguienteId = 1; // El primer ID será 1
        }

        public void AgregarTarea(string descripcion)
        {
            _tareas.Add(new Tarea(_siguienteId++, descripcion));
        }

        public Tarea ObtenerTareaPorId(int id)
        {
            return _tareas.FirstOrDefault(t => t.Id == id);
        }

        public List<Tarea> ObtenerTodasLasTareas()
        {
            // Devolvemos una copia para evitar modificaciones directas externas
            return new List<Tarea>(_tareas);
        }

        public bool MarcarComoCompletada(int id)
        {
            Tarea tarea = ObtenerTareaPorId(id);
            if (tarea != null)
            {
                tarea.Completada = true;
                return true;
            }
            return false;
        }

        public bool EliminarTarea(int id)
        {
            Tarea tareaAEliminar = ObtenerTareaPorId(id);
            if (tareaAEliminar != null)
            {
                _tareas.Remove(tareaAEliminar);
                return true;
            }
            return false;
        }
    }
}

//Vista/VistaConsola.cs
using TareasApp.Model;

namespace TareasApp.View
{
    public class VistaConsola
    {
        public void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("--- GESTIÓN DE TAREAS ---");
            Console.WriteLine("1. Agregar Tarea");
            Console.WriteLine("2. Listar Tareas");
            Console.WriteLine("3. Marcar Tarea como Completada");
            Console.WriteLine("4. Eliminar Tarea");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
        }

        public string ObtenerDescripcionTarea()
        {
            Console.Write("Ingrese la descripción de la tarea: ");
            return Console.ReadLine();
        }

        public int ObtenerIdTarea()
        {
            int id;
            Console.Write("Ingrese el ID de la tarea: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Entrada inválida. Por favor, ingrese un número.");
                Console.Write("Ingrese el ID de la tarea: ");
            }
            return id;
        }

        public void MostrarTareas(List<Tarea> tareas)
        {
            Console.Clear();
            if (tareas == null || !tareas.Any())
            {
                Console.WriteLine("No hay tareas para mostrar.");
                return;
            }

            Console.WriteLine("--- LISTA DE TAREAS ---");
            Console.WriteLine("ID\tEstado\tDescripción");
            Console.WriteLine("----------------------------------");
            foreach (var tarea in tareas)
            {
                string estado = tarea.Completada ? "Completada" : "Pendiente ";
                Console.WriteLine($"{tarea.Id}\t{estado}\t{tarea.Descripcion}");
            }
            Console.WriteLine("----------------------------------");
            Pausar();
        }

        public void MostrarMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
        }

        public void Pausar()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}

//Controlador/controladorTarea.cs
using TareasApp.Model;
using TareasApp.View;

namespace TareasApp.Controller
{
    public class ControladorTareas
    {
        private GestorTareas _gestorTareas;
        private VistaConsola _vista;

        public ControladorTareas(GestorTareas gestorTareas, VistaConsola vista)
        {
            _gestorTareas = gestorTareas;
            _vista = vista;
        }

        public void Iniciar()
        {
            bool salir = false;
            while (!salir)
            {
                _vista.MostrarMenu();
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        AgregarTarea();
                        break;
                    case "2":
                        ListarTareas();
                        break;
                    case "3":
                        MarcarComoCompletada();
                        break;
                    case "4":
                        EliminarTarea();
                        break;
                    case "5":
                        salir = true;
                        _vista.MostrarMensaje("Saliendo de la aplicación...");
                        break;
                    default:
                        _vista.MostrarMensaje("Opción no válida. Intente de nuevo.");
                        _vista.Pausar();
                        break;
                }
            }
        }

        private void AgregarTarea()
        {
            string descripcion = _vista.ObtenerDescripcionTarea();
            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                _gestorTareas.AgregarTarea(descripcion);
                _vista.MostrarMensaje("Tarea agregada con éxito.");
            }
            else
            {
                _vista.MostrarMensaje("La descripción de la tarea no puede estar vacía.");
            }
            _vista.Pausar();
        }

        private void ListarTareas()
        {
            List<Tarea> tareas = _gestorTareas.ObtenerTodasLasTareas();
            _vista.MostrarTareas(tareas);
        }

        private void MarcarComoCompletada()
        {
            int id = _vista.ObtenerIdTarea();
            if (_gestorTareas.MarcarComoCompletada(id))
            {
                _vista.MostrarMensaje($"Tarea con ID {id} marcada como completada.");
            }
            else
            {
                _vista.MostrarMensaje($"No se encontró una tarea con ID {id}.");
            }
            _vista.Pausar();
        }

        private void EliminarTarea()
        {
            int id = _vista.ObtenerIdTarea();
            if (_gestorTareas.EliminarTarea(id))
            {
                _vista.MostrarMensaje($"Tarea con ID {id} eliminada.");
            }
            else
            {
                _vista.MostrarMensaje($"No se encontró una tarea con ID {id}.");
            }
            _vista.Pausar();
        }
    }
}

//Program.cs

using TareasApp.Model;
using TareasApp.View;
using TareasApp.Controller;

namespace TareasApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instanciar el Modelo
            GestorTareas gestorTareas = new GestorTareas();

            // Instanciar la Vista
            VistaConsola vista = new VistaConsola();

            // Instanciar el Controlador, pasándole el Modelo y la Vista
            ControladorTareas controlador = new ControladorTareas(gestorTareas, vista);

            // Iniciar la aplicación a través del Controlador
            controlador.Iniciar();
        }
    }
}