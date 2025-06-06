//base de datos
USE MVPEmpleadosDB;
GO

CREATE TABLE Empleados (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Cargo NVARCHAR(100),
    Salario DECIMAL(18, 2)
);
GO

-- Inserta algunos datos de ejemplo
INSERT INTO Empleados (Nombre, Apellido, Cargo, Salario) VALUES
('Ana', 'García', 'Desarrollador Senior', 60000.00),
('Carlos', 'López', 'Gerente de Proyectos', 75000.00),
('María', 'Fernández', 'Diseñador UI/UX', 55000.00);
GO

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
    }
}

//Model/EmpleadoRepository.cs
using System;
using System.Data; // Para DbType
using Microsoft.Data.SqlClient; // Usamos el proveedor moderno
using System.Collections.Generic; // Para List<Empleado> si se necesitara

namespace MVP_Empleados_Ejemplo.Model
{
    public class EmpleadoRepository
    {
        // CADENA DE CONEXIÓN: ¡Cámbiala según tu configuración de SQL Server!
        // Ejemplo para SQL Server Express local
        private readonly string _connectionString = "Server=localhost\\SQLEXPRESS;Database=MVPEmpleadosDB;Integrated Security=True;TrustServerCertificate=True;";
        // O si usas un servidor local sin instancia:
        // private readonly string _connectionString = "Server=.;Database=MVPEmpleadosDB;Integrated Security=True;TrustServerCertificate=True;";
        // Si usas autenticación SQL Server:
        // private readonly string _connectionString = "Server=tu_servidor;Database=MVPEmpleadosDB;User Id=tu_usuario;Password=tu_contraseña;TrustServerCertificate=True;";


        public Empleado ObtenerEmpleado(int id)
        {
            Empleado empleado = null;
            // Usamos 'using' para asegurar que la conexión y el comando se cierren y liberen recursos
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Comando SQL para seleccionar un empleado por ID
                string query = "SELECT Id, Nombre, Apellido, Cargo, Salario FROM Empleados WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Añadimos un parámetro para evitar la inyección SQL y para tipar el valor
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    connection.Open(); // Abrimos la conexión a la base de datos

                    using (SqlDataReader reader = command.ExecuteReader()) // Ejecutamos el comando y obtenemos un lector de datos
                    {
                        if (reader.Read()) // Si hay una fila (un empleado)
                        {
                            empleado = new Empleado(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Nombre")),
                                reader.GetString(reader.GetOrdinal("Apellido")),
                                reader.GetString(reader.GetOrdinal("Cargo")),
                                reader.GetDecimal(reader.GetOrdinal("Salario"))
                            );
                        }
                    }
                }
            }
            return empleado;
        }

        public void GuardarEmpleado(Empleado empleado)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // En este ejemplo, el método "Guardar" intenta actualizar un empleado existente.
                // En una aplicación real, tendrías un método "Insertar" para nuevos empleados
                // y "Actualizar" para los existentes, o un método "Upsert" que haga ambas cosas.
                string query = @"
                    UPDATE Empleados
                    SET Nombre = @Nombre, Apellido = @Apellido, Cargo = @Cargo, Salario = @Salario
                    WHERE Id = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Añadimos parámetros para todos los campos a actualizar
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = empleado.Id;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = empleado.Nombre;
                    command.Parameters.Add("@Apellido", SqlDbType.NVarChar, 100).Value = empleado.Apellido;
                    command.Parameters.Add("@Cargo", SqlDbType.NVarChar, 100).Value = empleado.Cargo;
                    command.Parameters.Add("@Salario", SqlDbType.Decimal).Value = empleado.Salario;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery(); // Ejecutamos la consulta de actualización
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"[Modelo]: Empleado ID {empleado.Id} actualizado en DB.");
                    }
                    else
                    {
                        Console.WriteLine($"[Modelo]: Empleado ID {empleado.Id} no encontrado para actualizar. (Considerar añadir lógica de INSERT para nuevos IDs).");
                    }
                }
            }
        }
    }
}

//view/IEmpladoView.cs
namespace MVP_Empleados_Ejemplo.View
{
    public interface IEmpleadoView
    {
        string EmpleadoNombre { get; set; }
        string EmpleadoApellido { get; set; }
        string EmpleadoCargo { get; set; }
        string EmpleadoSalario { get; set; }

        void MostrarMensaje(string mensaje);

        event System.EventHandler CargarEmpleadoClick;
        event System.EventHandler GuardarEmpleadoClick;
    }
}

//View/Form/Empleado.cs
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