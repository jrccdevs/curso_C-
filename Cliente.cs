public class Cliente
{
    public string Nombre { get; set; }
    public Cuenta CuentaPrincipal { get; set; }

    public void MostrarDatos()
    {
        Console.WriteLine($"Cliente: {Nombre}");
        CuentaPrincipal.Mostrar();
    }
}
