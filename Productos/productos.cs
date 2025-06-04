// base de datos
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100),
    Precio DECIMAL(10,2),
    Stock INT
);
 

 //models/products
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

//view/viewProductos.cs
using Models;

public class ProductoView
{
    public Producto PedirProducto()
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();

        Console.Write("Precio: ");
        decimal precio = decimal.Parse(Console.ReadLine());

        Console.Write("Stock: ");
        int stock = int.Parse(Console.ReadLine());

        return new Producto { Nombre = nombre, Precio = precio, Stock = stock };
    }

    public void MostrarProductos(List<Producto> productos)
    {
        if (productos.Count == 0)
        {
            Console.WriteLine("No hay productos.");
            return;
        }

        foreach (var p in productos)
        {
            Console.WriteLine($"ID: {p.Id}, Nombre: {p.Nombre}, Precio: {p.Precio:C}, Stock: {p.Stock}");
        }
    }

    public void MostrarProducto(Producto producto)
    {
        if (producto == null)
        {
            Console.WriteLine("Producto no encontrado.");
        }
        else
        {
            Console.WriteLine($"ID: {producto.Id}, Nombre: {producto.Nombre}, Precio: {producto.Precio:C}, Stock: {producto.Stock}");
        }
    }
}


//controller/productosControllee
using Models;
using System.Data.SqlClient;

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


//program.cs
using Models;

class Program
{
    static void Main()
    {
        var controller = new ProductoController();
        var view = new ProductoView();
        int opcion;

        do
        {
            Console.WriteLine("\n=== MENÚ PRODUCTOS ===");
            Console.WriteLine("1. Agregar producto");
            Console.WriteLine("2. Ver todos los productos");
            Console.WriteLine("3. Buscar producto por ID");
            Console.WriteLine("4. Eliminar producto");
            Console.WriteLine("0. Salir");
            Console.Write("Opción: ");
            int.TryParse(Console.ReadLine(), out opcion);

            switch (opcion)
            {
                case 1:
                    var nuevo = view.PedirProducto();
                    controller.AgregarProducto(nuevo);
                    Console.WriteLine("Producto agregado.");
                    break;

                case 2:
                    var productos = controller.ObtenerTodos();
                    view.MostrarProductos(productos);
                    break;

                case 3:
                    Console.Write("Ingrese ID: ");
                    int idBuscar = int.Parse(Console.ReadLine());
                    var producto = controller.ObtenerPorId(idBuscar);
                    view.MostrarProducto(producto);
                    break;

                case 4:
                    Console.Write("Ingrese ID a eliminar: ");
                    int idEliminar = int.Parse(Console.ReadLine());
                    controller.EliminarProducto(idEliminar);
                    Console.WriteLine("Producto eliminado.");
                    break;

                case 0:
                    Console.WriteLine("Saliendo...");
                    break;

                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

        } while (opcion != 0);
    }
}
