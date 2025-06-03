// ESTRUCTURA MVC COMPLETA CON SQL SERVER PARA GESTIÓN DE ESTUDIANTES

// 1. MODELO - Models/Estudiante.cs
using System;

namespace Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Carrera { get; set; }
    }
}

// 2. VISTA - Views/EstudianteView.cs
using System;
using System.Collections.Generic;
using Models;

namespace Views
{
    public class EstudianteView
    {
        public Estudiante PedirEstudiante()
        {
            Console.Write("Nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("Edad: ");
            int.TryParse(Console.ReadLine(), out int edad);

            Console.Write("Carrera: ");
            string carrera = Console.ReadLine();

            return new Estudiante { Nombre = nombre, Edad = edad, Carrera = carrera };
        }

        public void MostrarEstudiantes(List<Estudiante> estudiantes)
        {
            if (estudiantes.Count == 0)
            {
                Console.WriteLine("No hay estudiantes registrados.");
                return;
            }
            foreach (var est in estudiantes)
            {
                Console.WriteLine($"ID: {est.Id} - {est.Nombre} - Edad: {est.Edad} - Carrera: {est.Carrera}");
            }
        }

        public void MostrarEstudiante(Estudiante est)
        {
            if (est == null)
            {
                Console.WriteLine("Estudiante no encontrado.");
            }
            else
            {
                Console.WriteLine($"ID: {est.Id} - {est.Nombre} - Edad: {est.Edad} - Carrera: {est.Carrera}");
            }
        }
    }
}

// 3. CONTROLADOR - Controllers/EstudianteController.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Models;

namespace Controllers
{
    public class EstudianteController
    {
        private string connectionString = "Server=TU_SERVIDOR;Database=TuBaseDeDatos;Trusted_Connection=True;";

        public void AgregarEstudiante(Estudiante est)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Estudiantes (Nombre, Edad, Carrera) VALUES (@Nombre, @Edad, @Carrera)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nombre", est.Nombre);
                cmd.Parameters.AddWithValue("@Edad", est.Edad);
                cmd.Parameters.AddWithValue("@Carrera", est.Carrera);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Estudiante> ObtenerTodos()
        {
            List<Estudiante> lista = new List<Estudiante>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Estudiantes";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Estudiante
                    {
                        Id = (int)reader["Id"],
                        Nombre = reader["Nombre"].ToString(),
                        Edad = (int)reader["Edad"],
                        Carrera = reader["Carrera"].ToString()
                    });
                }
            }
            return lista;
        }

        public Estudiante ObtenerPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Estudiantes WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Estudiante
                    {
                        Id = (int)reader["Id"],
                        Nombre = reader["Nombre"].ToString(),
                        Edad = (int)reader["Edad"],
                        Carrera = reader["Carrera"].ToString()
                    };
                }
            }
            return null;
        }

        public void EliminarEstudiante(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Estudiantes WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

// 4. PROGRAMA PRINCIPAL - Program.cs
using System;
using Controllers;
using Views;

class Program
{
    static void Main(string[] args)
    {
        var ctrl = new EstudianteController();
        var vista = new EstudianteView();
        int opcion;
        do
        {
            Console.WriteLine("\n=== MENÚ ESTUDIANTES ===");
            Console.WriteLine("1. Agregar");
            Console.WriteLine("2. Ver Todos");
            Console.WriteLine("3. Buscar por ID");
            Console.WriteLine("4. Eliminar por ID");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            int.TryParse(Console.ReadLine(), out opcion);

            switch (opcion)
            {
                case 1:
                    var est = vista.PedirEstudiante();
                    ctrl.AgregarEstudiante(est);
                    Console.WriteLine("✅ Agregado");
                    break;
                case 2:
                    var todos = ctrl.ObtenerTodos();
                    vista.MostrarEstudiantes(todos);
                    break;
                case 3:
                    Console.Write("ID: ");
                    int.TryParse(Console.ReadLine(), out int idB);
                    var e = ctrl.ObtenerPorId(idB);
                    vista.MostrarEstudiante(e);
                    break;
                case 4:
                    Console.Write("ID a eliminar: ");
                    int.TryParse(Console.ReadLine(), out int idE);
                    ctrl.EliminarEstudiante(idE);
                    Console.WriteLine("✅ Eliminado");
                    break;
                case 0:
                    Console.WriteLine("Saliendo...");
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        } while (opcion != 0);
    }
}

// NOTA IMPORTANTE:
// Debes crear una base de datos con esta tabla:
// CREATE TABLE Estudiantes (
//     Id INT IDENTITY(1,1) PRIMARY KEY,
//     Nombre NVARCHAR(100),
//     Edad INT,
//     Carrera NVARCHAR(100)
// );
