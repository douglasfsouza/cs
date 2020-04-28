using System;

namespace dgSerializeBin
{
    class Program
    {
        static void Main(string[] args)
        {
            //Serializar
            //DateTime dt = DateTime.Parse("05-01-1977");
            DateTime dt = Convert.ToDateTime("05/01/1977", System.Globalization.CultureInfo.InvariantCulture);

            TestSerialize t = new TestSerialize("Douglas", "m", dt);
            CustomSerializeBin < TestSerialize > b = new CustomSerializeBin<TestSerialize>();
            byte[] rs = b.SerializetoBytes(t);
            for (int i = 0; i < rs.Length; i++)
            {
                Console.WriteLine(rs[i]);
            }            
            Console.Read();

            //Desserializar
            CustomSerializeBin<TestSerialize> bd = new CustomSerializeBin<TestSerialize>();
            TestSerialize td = bd.DeserializeFromBytes(rs);
            string strdt = td.DataNas.ToString("MM/dd/yyyy");

            Console.WriteLine($"Nome:{td.Nome} - Sexo:{td.Sexo} - DataNas:{strdt}");
            Console.Read();

        }
        
    }

    [Serializable]
    internal class TestSerialize
    {
        public string Nome { get; set; }
        public string Sexo { get; set; }
        public DateTime DataNas { get; set; }
        public TestSerialize()
        {
        }
        public TestSerialize(string nome, string sexo, DateTime dataNas)
        {
            Nome = nome;
            Sexo = sexo;
            DataNas = dataNas;
        }
    }
}
