// See https://aka.ms/new-console-template for more information
using DgsGofVisitor;

Console.WriteLine("Hello, Implementando o padrão GOF Visitor!");
Carro car = new Carro("Corolla", 70000);

Console.WriteLine($"Carro: {car.Nome}");
Console.WriteLine($"Preço: {car.Preco}");

IVisitor visitorDesconto = new VisitorDesconto();
visitorDesconto.Accept(car);

IVisitor visitorAumento = new VisitorAumento();
visitorAumento.Accept(car);

