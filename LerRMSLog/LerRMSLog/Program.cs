// See https://aka.ms/new-console-template for more information

bool updateOn = false;
double valor = 0;
string log = "C:\\log\\jad.txt";
StreamReader reader;
StreamWriter writer;

reader = File.OpenText(log);
writer = File.CreateText("C:\\log\\updates.txt");

while (!reader.EndOfStream)
{
    string line = reader.ReadLine();
    if (updateOn)
    {
        Console.WriteLine(line);
        valor += Convert.ToDouble(line.Split('=')[1].Replace(".",","));
        updateOn = false;
        writer.WriteLine(line);
    }
    if (line.Contains("UPDATE AA1RTITU"))
    {
        updateOn = true;                
    }    
}
writer.Close();
reader.Close();
Console.WriteLine($"Total: {valor}");

