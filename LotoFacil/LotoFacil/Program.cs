// See https://aka.ms/new-console-template for more information
Console.WriteLine("Loto facil");

for(int n1 = 1; n1 < 26; n1++)
{
	for (int n2 = 1; n2 < 26; n2++)
	{
		if (n2 > n1)
		{
			Console.WriteLine($"{n1}-{n2}");
		}

	}


}
