public class Cuenta
{
    public string Numero { get; set; }
    public decimal Saldo { get; set; }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public void Mostrar()
    {
        Console.WriteLine($"Cuenta: {Numero}, Saldo: {Saldo} Bs");
    }
}
