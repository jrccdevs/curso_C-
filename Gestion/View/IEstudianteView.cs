namespace MVP_Estudiante_Ejemplo.View
{
    public interface IEstudianteView
    {
        // Propiedades para obtener y establecer los datos de la UI
        string NombreEstudiante { get; set; }
        string ApellidoEstudiante { get; set; }
        string EdadEstudiante { get; set; } // Lo manejamos como string para la UI

        // Métodos para mostrar mensajes al usuario
        void MostrarMensaje(string mensaje);

        // Eventos que la Vista expone al Presentador cuando hay interacciones del usuario
        event System.EventHandler CargarEstudianteClick;
        event System.EventHandler GuardarEstudianteClick;
    }
}