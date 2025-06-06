//Model/Empleado.cs
using System;

namespace MVP_Empleados_Ejemplo.Model
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cargo { get; set; }
        public decimal Salario { get; set; }

        public Empleado(int id, string nombre, string apellido, string cargo, decimal salario)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Cargo = cargo;
            Salario = salario;
        }

        // Lógica de negocio simple, por ejemplo, aplicar un aumento de salario
        public void AplicarAumento(decimal porcentaje)
        {
            if (porcentaje > 0)
            {
                Salario += Salario * (porcentaje / 100);
            }
        }
    }

    // Un "repositorio" o "servicio" que simula la interacción con la base de datos
    public class EmpleadoRepository
    {
        // Simulación de una base de datos en memoria con un empleado de ejemplo
        private Empleado _empleadoActual;

        public EmpleadoRepository()
        {
            _empleadoActual = new Empleado(1, "Ana", "García", "Desarrollador Senior", 60000.00m);
        }

        public Empleado ObtenerEmpleado(int id)
        {
            // En una aplicación real, esto iría a la base de datos
            if (id == _empleadoActual.Id)
            {
                return _empleadoActual;
            }
            return null; // O lanzar una excepción si no se encuentra
        }

        public void GuardarEmpleado(Empleado empleado)
        {
            // En una aplicación real, esto guardaría en la base de datos
            if (empleado.Id == _empleadoActual.Id)
            {
                _empleadoActual.Nombre = empleado.Nombre;
                _empleadoActual.Apellido = empleado.Apellido;
                _empleadoActual.Cargo = empleado.Cargo;
                _empleadoActual.Salario = empleado.Salario;
                Console.WriteLine($"[Modelo]: Empleado ID {empleado.Id} actualizado: {empleado.Nombre} {empleado.Apellido}, Cargo: {empleado.Cargo}, Salario: {empleado.Salario:C}.");
            }
            else
            {
                // En un escenario real, manejar la creación de nuevos empleados
                Console.WriteLine("[Modelo]: No se puede guardar un empleado con un ID diferente en este ejemplo simple.");
            }
        }
    }
}

//View/IEmpleadoView.cs
namespace MVP_Empleados_Ejemplo.View
{
    public interface IEmpleadoView
    {
        // Propiedades para obtener y establecer los datos de la UI
        string EmpleadoNombre { get; set; }
        string EmpleadoApellido { get; set; }
        string EmpleadoCargo { get; set; }
        string EmpleadoSalario { get; set; } // Lo manejamos como string para la UI

        // Métodos para mostrar mensajes al usuario
        void MostrarMensaje(string mensaje);

        // Eventos que la Vista expone al Presentador cuando hay interacciones del usuario
        event System.EventHandler CargarEmpleadoClick;
        event System.EventHandler GuardarEmpleadoClick;
    }
}

//View/FormEmpleado.cs
using System;
using System.Windows.Forms;
using MVP_Empleados_Ejemplo.View;
using MVP_Empleados_Ejemplo.Presenter; // Necesario para instanciar el Presentador

namespace MVP_Empleados_Ejemplo
{
    public partial class FormEmpleado : Form, IEmpleadoView
    {
        private EmpleadoPresenter _presenter;

        public FormEmpleado()
        {
            InitializeComponent();
            // El Presentador necesita una referencia a esta Vista
            _presenter = new EmpleadoPresenter(this);
            // Suscribir al evento Load del formulario para cargar datos iniciales
            this.Load += FormEmpleado_Load;
        }

        // --- Implementación de las propiedades de la interfaz IEmpleadoView ---
        public string EmpleadoNombre
        {
            get { return txtNombre.Text; }
            set { txtNombre.Text = value; }
        }

        public string EmpleadoApellido
        {
            get { return txtApellido.Text; }
            set { txtApellido.Text = value; }
        }

        public string EmpleadoCargo
        {
            get { return txtCargo.Text; }
            set { txtCargo.Text = value; }
        }

        public string EmpleadoSalario
        {
            get { return txtSalario.Text; }
            set { txtSalario.Text = value; }
        }

        public void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
        }

        // --- Implementación de los eventos de la interfaz IEmpleadoView ---
        // Estos eventos serán suscritos por el Presentador
        public event EventHandler CargarEmpleadoClick;
        public event EventHandler GuardarEmpleadoClick;

        // --- Métodos de los manejadores de eventos del formulario (click de botones) ---
        // Estos invocan los eventos de la interfaz para que el Presentador los escuche
        private void FormEmpleado_Load(object sender, EventArgs e)
        {
            // Cargar el empleado inicial al cargar el formulario
            CargarEmpleadoClick?.Invoke(this, EventArgs.Empty);
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            CargarEmpleadoClick?.Invoke(this, EventArgs.Empty);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarEmpleadoClick?.Invoke(this, EventArgs.Empty);
        }
    }
}

//Presenter/EmpleadoPresenter.cs
using System;
using MVP_Empleados_Ejemplo.Model;
using MVP_Empleados_Ejemplo.View;

namespace MVP_Empleados_Ejemplo.Presenter
{
    public class EmpleadoPresenter
    {
        private IEmpleadoView _view;
        private EmpleadoRepository _model; // El Presentador tiene una referencia al Modelo

        // El constructor recibe la Vista (a través de su interfaz)
        public EmpleadoPresenter(IEmpleadoView view)
        {
            _view = view;
            _model = new EmpleadoRepository(); // Instanciamos nuestro "repositorio" del Modelo

            // Suscribimos los eventos de la Vista a nuestros métodos del Presentador
            _view.CargarEmpleadoClick += OnCargarEmpleadoClick;
            _view.GuardarEmpleadoClick += OnGuardarEmpleadoClick;
        }

        // Manejador del evento CargarEmpleadoClick de la Vista
        private void OnCargarEmpleadoClick(object sender, EventArgs e)
        {
            // El Presentador le pide al Modelo los datos (siempre ID 1 en este ejemplo)
            Empleado empleado = _model.ObtenerEmpleado(1);

            if (empleado != null)
            {
                // El Presentador actualiza la Vista con los datos del Modelo
                _view.EmpleadoNombre = empleado.Nombre;
                _view.EmpleadoApellido = empleado.Apellido;
                _view.EmpleadoCargo = empleado.Cargo;
                _view.EmpleadoSalario = empleado.Salario.ToString("F2"); // Formato a 2 decimales
                _view.MostrarMensaje("Datos del empleado cargados.");
            }
            else
            {
                _view.MostrarMensaje("No se pudo cargar el empleado.");
            }
        }

        // Manejador del evento GuardarEmpleadoClick de la Vista
        private void OnGuardarEmpleadoClick(object sender, EventArgs e)
        {
            // El Presentador obtiene los datos de la Vista
            string nombre = _view.EmpleadoNombre.Trim();
            string apellido = _view.EmpleadoApellido.Trim();
            string cargo = _view.EmpleadoCargo.Trim();
            decimal salario;

            // Validaciones básicas de la lógica de presentación
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(cargo))
            {
                _view.MostrarMensaje("Nombre, Apellido y Cargo no pueden estar vacíos.");
                return;
            }

            if (!decimal.TryParse(_view.EmpleadoSalario, out salario) || salario < 0)
            {
                _view.MostrarMensaje("Salario inválido. Debe ser un número positivo.");
                return;
            }

            // Creamos un nuevo objeto Empleado con los datos de la Vista (siempre ID 1 para este ejemplo)
            Empleado empleado = new Empleado(1, nombre, apellido, cargo, salario);

            // El Presentador le dice al Modelo que guarde los datos
            _model.GuardarEmpleado(empleado);

            // El Presentador actualiza la Vista con un mensaje de éxito
            _view.MostrarMensaje("Empleado guardado exitosamente!");
        }
    }
}

//Program.cs
using System;
using System.Windows.Forms; // Necesario para Application.Run() y Form

namespace MVP_Empleados_Ejemplo
{
    internal static class Program
    {
        /// <summary>
        ///  Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Crea y ejecuta la instancia de tu formulario principal (la Vista)
            Application.Run(new FormEmpleado());
        }
    }
}