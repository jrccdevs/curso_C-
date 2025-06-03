using System;

class Program
{
    static void Main(string[] args)
    {
        // Crear una cuenta
        Cuenta cuenta = new Cuenta
        {
            Numero = "ACC-1001",
            Saldo = 1000.00m
        };

        // Crear un cliente y asignarle la cuenta
        Cliente cliente = new Cliente
        {
            Nombre = "Juan Pérez",
            CuentaPrincipal = cuenta
        };

        // Mostrar datos
        cliente.MostrarDatos();
    }
}
