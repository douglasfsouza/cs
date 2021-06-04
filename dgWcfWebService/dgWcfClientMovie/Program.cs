using System;
using System.Threading.Tasks;
using ServiceReference2;
using dgWcfWebService;


namespace dgWcfClientMovie
{
    class Program
    {
        static void Main(string[] args)
        {

            MovieServiceClient client = new MovieServiceClient();

            // Use a variável 'client' para chamar as operações no serviço.

            // Sempre feche o cliente.
            //client.GetMovieAsync(2);
            //Movie m = new Movie();
            //client.UpdateMovieAsync(m);
            clsTst t = new clsTst();
            t.Tst(client);
             
            
            //Console.WriteLine(client.)

            //client.Close();

           // Console.WriteLine("Hello World!");
        }
        
    }
    public class clsTst
    {
        async public Task<string> Tst(MovieServiceClient client)
        {
            Movie m = new Movie();
            m = await client.GetMovieAsync(1);
            Console.WriteLine(m.Protagonist);
            //await client.UpdateMovieAsync(m);
            return null;
        }

    }
    
    
}
