using System;
using System.IO;
using System.Text;

namespace dgArquivosMemoryStream
{
    class Program
    {
        static void Main(string[] args)
        {
            UnicodeEncoding uniEncoding = new UnicodeEncoding();

            MemoryStream m = GerarArquivo();

            // Set the position to the beginning of the stream.
            m.Seek(0, SeekOrigin.Begin);

            byte[] byteArray = new byte[m.Length];

            int count = m.Read(byteArray, 0, 20);

            while(count < m.Length)
            {
                byteArray[count++] = Convert.ToByte(m.ReadByte());
            }

            // Decode the byte array into a char array
            // and write it to the console.
            
            char[] charArray = new char[uniEncoding.GetCharCount(
                byteArray, 0, count)];
            uniEncoding.GetDecoder().GetChars(
                byteArray, 0, count, charArray, 0);

            Console.WriteLine("Leitura com MemoryStream");
            Console.WriteLine(charArray);            
            
            m.Close();

            MemoryStream GerarArquivo()
            {
                byte[] firstString = uniEncoding.GetBytes(
            "Hello, Invalid file path characters are: ");
                byte[] secondString = uniEncoding.GetBytes(
                    Path.GetInvalidPathChars());

                MemoryStream m = new MemoryStream(100);
                m.Write(firstString, 0, firstString.Length);
                int count = 0;
                while (count < secondString.Length)
                {
                    m.WriteByte(secondString[count++]);
                }                   
                return m;
            }
        }
    }
}
