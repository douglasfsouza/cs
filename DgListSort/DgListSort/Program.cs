// See https://aka.ms/new-console-template for more information

List<string> fruits = new List<string>();
fruits.Add("lemon");
fruits.Add("apple");
fruits.Add("banana");
Console.WriteLine("Before sort:");
fruits.ForEach(x => Console.WriteLine(x));

fruits.Sort((x,y)=> x.CompareTo(y));
Console.WriteLine("After sort:");
fruits.ForEach(x => Console.WriteLine(x));
