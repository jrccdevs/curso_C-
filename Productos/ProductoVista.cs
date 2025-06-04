//Models
//    Producto.cs
//Controllers
//    ProductoController.cs
//Forms
//    MainForm.cs
//Program.cs

//models/producto.cs
namespace Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}

//controllers/producto/controller.cs
using Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Controllers
{
    public class ProductoController
    {
        private string connectionString = "Data Source=TU_SERVIDOR;Initial Catalog=TU_BASE;Integrated Security=True;";

        public void AgregarProducto(Producto producto)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Productos (Nombre, Precio, Stock) VALUES (@Nombre, @Precio, @Stock)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                cmd.Parameters.AddWithValue("@Stock", producto.Stock);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Productos";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Producto p = new Producto
                    {
                        Id = (int)reader["Id"],
                        Nombre = reader["Nombre"].ToString(),
                        Precio = (decimal)reader["Precio"],
                        Stock = (int)reader["Stock"]
                    };
                    lista.Add(p);
                }
            }

            return lista;
        }

        public Producto ObtenerPorId(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Productos WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Producto
                    {
                        Id = (int)reader["Id"],
                        Nombre = reader["Nombre"].ToString(),
                        Precio = (decimal)reader["Precio"],
                        Stock = (int)reader["Stock"]
                    };
                }
            }

            return null;
        }

        public void EliminarProducto(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Productos WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

//Forms/MainForms.cs
using Controllers;
using Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Forms
{
    public partial class MainForm : Form
    {
        ProductoController controller = new ProductoController();

        public MainForm()
        {
            InitializeComponent();
            CargarProductos();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Producto nuevo = new Producto
            {
                Nombre = txtNombre.Text,
                Precio = decimal.Parse(txtPrecio.Text),
                Stock = int.Parse(txtStock.Text)
            };

            controller.AgregarProducto(nuevo);
            MessageBox.Show("Producto agregado.");
            CargarProductos();
        }

        private void CargarProductos()
        {
            List<Producto> productos = controller.ObtenerTodos();
            dgvProductos.DataSource = productos;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtBuscarId.Text);
            Producto prod = controller.ObtenerPorId(id);
            if (prod != null)
            {
                MessageBox.Show($"Nombre: {prod.Nombre}, Precio: {prod.Precio}, Stock: {prod.Stock}");
            }
            else
            {
                MessageBox.Show("Producto no encontrado.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtEliminarId.Text);
            controller.EliminarProducto(id);
            MessageBox.Show("Producto eliminado.");
            CargarProductos();
        }
    }
}


//Program.cs
using System;
using System.Windows.Forms;
using Forms;

namespace TuProyecto
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
