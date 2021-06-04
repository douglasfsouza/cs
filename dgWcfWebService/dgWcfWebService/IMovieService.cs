using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace dgWcfWebService
{
    // OBSERVAÇÃO: Você pode usar o comando "Renomear" no menu "Refatorar" para alterar o nome da interface "IService1" no arquivo de código e configuração ao mesmo tempo.
    [ServiceContract]
    public interface IMovieService
    {

        [OperationContract]
        Movie GetMovie(int id);

        [OperationContract]
        bool UpdateMovie(Movie movie);

        // TODO: Adicione suas operações de serviço aqui
    }


    // Use um contrato de dados como ilustrado no exemplo abaixo para adicionar tipos compostos a operações de serviço.
    [DataContract]

    public class Movie
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string MovieName { get; set; }
        [DataMember]
        public string Protagonist { get; set; }

    }
    
}
