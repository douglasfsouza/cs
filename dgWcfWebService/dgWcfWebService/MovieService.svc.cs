using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace dgWcfWebService
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da classe "Service1" no arquivo de código, svc e configuração ao mesmo tempo.
    // OBSERVAÇÃO: Para iniciar o cliente de teste do WCF para testar esse serviço, selecione Service1.svc ou Service1.svc.cs no Gerenciador de Soluções e inicie a depuração.
    public class Service1 : IMovieService
    {
        public Movie GetMovie(int id)
        {
            return new Movie()
            {
                Id = 1,
                MovieName = "Topgun",
                Protagonist = "Tom Cruise"
            };
        }
        
        public bool UpdateMovie(Movie movie)
        {
            if (movie.Id == 0)
            {
                throw new Exception("Invalid ID");
            }
            if (string.IsNullOrEmpty(movie.MovieName))
            {
                throw new Exception("Invalid movie name");
            }

            if (string.IsNullOrEmpty(movie.Protagonist))
            {
                throw new Exception("Invalid protagonist");
            }
            return true;

        }
        public bool Async(Movie movie)
        {
            if (movie.Id == 0)
            {
                throw new Exception("Invalid ID");
            }
            if (string.IsNullOrEmpty(movie.MovieName))
            {
                throw new Exception("Invalid movie name");
            }

            if (string.IsNullOrEmpty(movie.Protagonist))
            {
                throw new Exception("Invalid protagonist");
            }
            return true;

        }

    }
}
