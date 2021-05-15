using System;
using System.IO;

namespace dgFileStreamBin
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"c:\tst\fileBin.txt";
            byte[] dataCol = { 1, 3, 5, 6, 104 };
            FileStream fileDest = new FileStream(filePath,FileMode.Create,FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(fileDest);
            foreach (var d in dataCol)
            {
                writer.Write(d);
            }
            fileDest.Close();
            writer.Close();
            Console.WriteLine("Gerado do arquivo {0}",filePath);

            //Abrir e ler
            FileStream fileRead = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(fileRead);
            Console.WriteLine();
            Console.WriteLine("Tamanho:{0} bytes", reader.BaseStream.Length);

            int position = 0;
            int byteReturned;
            int length = (int)reader.BaseStream.Length;
            byte[] dataRec = new byte[length];
            
            while ((byteReturned = reader.Read()) != -1)
            {
                dataRec[position] = (byte)byteReturned;
                position += sizeof(byte);
            }

            fileRead.Close();
            reader.Close();
            Console.WriteLine("Bytes lidos:");
            foreach (byte b in dataRec)
            {
                Console.Write(b + "-");
            }

        }
    }
}
