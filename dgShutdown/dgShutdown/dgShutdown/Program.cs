// See https://aka.ms/new-console-template for more information
using dgShutdown;
using System.ComponentModel;

internal class Program
{
    private static void Main(string[] args)
    {

        Win w = new Win();
        //w.Force();
        if (DateTime.Now.Hour > 22 || DateTime.Now.Hour < 6)
        {
            w.Desligar();
        }
        
        //w.FecharEDesligar();
        // w.Reiniciar();
    }
}