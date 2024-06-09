// See https://aka.ms/new-console-template for more information
public class DgDelegate4
{
    public delegate void ContraVariance(StreamWriter  tw);
    public static void StreamMethod(TextWriter tw)
    {
        return;
    }     

    public static void Main()
    {
        ContraVariance c = StreamMethod;
       
        Console.WriteLine("Exemplo de ContraVariance Delegate");
    }

}
