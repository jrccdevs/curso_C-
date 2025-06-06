using System;
using System.Windows.Forms;
using MVP_Estudiante_Ejemplo.View;
using MVP_Estudiante_Ejemplo.Presenter; // Necesario para instanciar el Presentador

namespace MVP_Estudiante_Ejemplo
{
    public partial class FormEstudiante : Form, IEstudianteView
    {
        private EstudiantePresenter _presenter;

        public FormEstudiante()
        {
            InitializeComponent();
            _presenter = new EstudiantePresenter(this); // El Presentador necesita una referencia a esta Vista
        }

        // Implementación de las propiedades de la interfaz IEstudianteView
        public string NombreEstudiante
        {
            get { return txtNombre.Text; }
            set { txtNombre.Text = value; }
        }

        public string ApellidoEstudiante
        {
            get { return txtApellido.Text; }
            set { txtApellido.Text = value; }
        }

        public string EdadEstudiante
        {
            get { return txtEdad.Text; }
            set { txtEdad.Text = value; }
        }

        public void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = mensaje;
        }

        // Implementación de los eventos de la interfaz IEstudianteView
        // Estos eventos serán suscritos por el Presentador
        public event EventHandler CargarEstudianteClick;
        public event EventHandler GuardarEstudianteClick;

        // Métodos de los manejadores de eventos del formulario (click de botones)
        // Estos invocan los eventos de la interfaz para que el Presentador los escuche
        private void btnCargar_Click(object sender, EventArgs e)
        {
            CargarEstudianteClick?.Invoke(this, EventArgs.Empty);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            GuardarEstudianteClick?.Invoke(this, EventArgs.Empty);
        }

        // Opcional: Llama a cargar al iniciar el formulario
        private void FormEstudiante_Load(object sender, EventArgs e)
        {
            // Cargar el estudiante inicial al cargar el formulario
            CargarEstudianteClick?.Invoke(this, EventArgs.Empty);
        }
    }
}