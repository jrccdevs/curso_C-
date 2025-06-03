namespace Curso.Models
{
    public class Estudiante
    {
        // Propiedades
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public int Anio;

        // Constructor
        public Estudiante(int id, string nombre, string carrera, int anio)
        {
            Id = id;
            Nombre = nombre;
            Carrera = carrera;
            Anio = anio;
        }
    }
}
