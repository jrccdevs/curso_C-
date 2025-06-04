// aplicacion de tienda de productos con modelo vista controlador

//models
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

//(Similares para Cliente.cs y Pedido.cs)
namespace Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
    }
}

// models/Pedido
using System;
using System.Collections.Generic;

namespace Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public Cliente Cliente { get; set; }
        public List<Producto> Productos { get; set; }
        public DateTime Fecha { get; set; }
    }
}

//Vista
//View/ProductView.cs
using Models;
using System;
using System.Collections.Generic;

namespace Views
{
    public class ProductoView
    {
        public Producto PedirProducto()
        {
            Console.Write("Nombre del producto: ");
            string nombre = Console.ReadLine();

            Console.Write("Precio: ");
            decimal.TryParse(Console.ReadLine(), out decimal precio);

            Console.Write("Stock: ");
            int.TryParse(Console.ReadLine(), out int stock);

            return new Producto { Nombre = nombre, Precio = precio, Stock = stock };
        }

        public void MostrarProductos(List<Producto> productos)
        {
            foreach (var p in productos)
                Console.WriteLine($"ID: {p.Id} - {p.Nombre} - ${p.Precio} - Stock: {p.Stock}");
        }
    }
}


//view/Cliente
using Models;
using System;
using System.Collections.Generic;

namespace Views
{
    public class ClienteView
    {
        public Cliente PedirCliente()
        {
            Console.Write("Nombre completo: ");
            string nombre = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            return new Cliente { NombreCompleto = nombre, Email = email };
        }

        public void MostrarClientes(List<Cliente> clientes)
        {
            foreach (var c in clientes)
                Console.WriteLine($"ID: {c.Id} - {c.NombreCompleto} - {c.Email}");
        }
    }
}

//View/Pedido.cs
using Models;
using System;
using System.Collections.Generic;

namespace Views
{
    public class PedidoView
    {
        public void MostrarPedidos(List<Pedido> pedidos)
        {
            foreach (var p in pedidos)
            {
                Console.WriteLine($"\nPedido ID: {p.Id} - Cliente: {p.Cliente.NombreCompleto} - Fecha: {p.Fecha}");
                Console.WriteLine("Productos:");
                foreach (var prod in p.Productos)
                    Console.WriteLine($"   - {prod.Nombre} (${prod.Precio})");
            }
        }

        public void MostrarMensaje(string mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}

//Controllers/Producto.cs
using Models;
using System.Collections.Generic;

namespace Controllers
{
    public class ProductoController
    {
        private List<Producto> productos = new List<Producto>();
        private int idActual = 1;

        public void AgregarProducto(Producto p)
        {
            p.Id = idActual++;
            productos.Add(p);
        }

        public List<Producto> ObtenerTodos()
        {
            return productos;
        }

        public Producto ObtenerPorId(int id)
        {
            return productos.Find(p => p.Id == id);
        }
    }
}

//Controllers/Cliente.cs
using Models;
using System.Collections.Generic;

namespace Controllers
{
    public class ClienteController
    {
        private List<Cliente> clientes = new List<Cliente>();
        private int idActual = 1;

        public void AgregarCliente(Cliente c)
        {
            c.Id = idActual++;
            clientes.Add(c);
        }

        public List<Cliente> ObtenerTodos()
        {
            return clientes;
        }

        public Cliente ObtenerPorId(int id)
        {
            return clientes.Find(c => c.Id == id);
        }
    }
}

//Controllers/Pedido
using Models;
using System;
using System.Collections.Generic;

namespace Controllers
{
    public class PedidoController
    {
        private List<Pedido> pedidos = new List<Pedido>();
        private int idActual = 1;

        public void CrearPedido(Cliente cliente, List<Producto> productos)
        {
            Pedido p = new Pedido
            {
                Id = idActual++,
                Cliente = cliente,
                Productos = productos,
                Fecha = DateTime.Now
            };
            pedidos.Add(p);
        }

        public List<Pedido> ObtenerTodos()
        {
            return pedidos;
        }
    }
}


//Programs.cs
using Controllers;
using Views;
using Models;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        var productoCtrl = new ProductoController();
        var clienteCtrl = new ClienteController();
        var pedidoCtrl = new PedidoController();

        var productoView = new ProductoView();
        var clienteView = new ClienteView();
        var pedidoView = new PedidoView();

        int opcion;
        do
        {
            Console.WriteLine("\n==== MENÚ ====");
            Console.WriteLine("1. Agregar Producto");
            Console.WriteLine("2. Listar Productos");
            Console.WriteLine("3. Agregar Cliente");
            Console.WriteLine("4. Listar Clientes");
            Console.WriteLine("5. Crear Pedido");
            Console.WriteLine("6. Ver Pedidos");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            int.TryParse(Console.ReadLine(), out opcion);

            switch (opcion)
            {
                case 1:
                    var producto = productoView.PedirProducto();
                    productoCtrl.AgregarProducto(producto);
                    break;
                case 2:
                    productoView.MostrarProductos(productoCtrl.ObtenerTodos());
                    break;
                case 3:
                    var cliente = clienteView.PedirCliente();
                    clienteCtrl.AgregarCliente(cliente);
                    break;
                case 4:
                    clienteView.MostrarClientes(clienteCtrl.ObtenerTodos());
                    break;
                case 5:
                    clienteView.MostrarClientes(clienteCtrl.ObtenerTodos());
                    Console.Write("ID del cliente: ");
                    int.TryParse(Console.ReadLine(), out int idCliente);
                    var clientePedido = clienteCtrl.ObtenerPorId(idCliente);

                    productoView.MostrarProductos(productoCtrl.ObtenerTodos());
                    Console.Write("IDs de productos separados por coma: ");
                    string[] ids = Console.ReadLine().Split(',');
                    var productosPedido = new List<Producto>();

                    foreach (string id in ids)
                    {
                        if (int.TryParse(id.Trim(), out int pid))
                        {
                            var prod = productoCtrl.ObtenerPorId(pid);
                            if (prod != null)
                                productosPedido.Add(prod);
                        }
                    }

                    if (clientePedido != null && productosPedido.Count > 0)
                    {
                        pedidoCtrl.CrearPedido(clientePedido, productosPedido);
                        pedidoView.MostrarMensaje("Pedido registrado correctamente.");
                    }
                    else
                    {
                        pedidoView.MostrarMensaje("Error al crear el pedido.");
                    }
                    break;
                case 6:
                    pedidoView.MostrarPedidos(pedidoCtrl.ObtenerTodos());
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
