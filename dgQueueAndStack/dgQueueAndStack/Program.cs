using System;
using System.Collections.Generic;
using System.Linq;

namespace dgQueueAndStack
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<string> fila = new Queue<string>();

            fila.Enqueue("A");
            fila.Enqueue("B");
            fila.Enqueue("C");
            Console.WriteLine("Listando toda a fila:");
            foreach (var f in fila)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine("Saindo 1:");

            fila.Dequeue();

            Console.WriteLine("Listando toda a fila:");

            foreach (var f in fila)
            {
                Console.WriteLine(f);
            }

            Console.WriteLine("Entrando D");
            fila.Enqueue("D");

            Console.WriteLine("Listando toda a fila:");

            foreach (var f in fila)
            {
                Console.WriteLine(f);
            }

            Stack<int> pilha = new Stack<int>();
            pilha.Push(100);
            pilha.Push(200);
            pilha.Push(300);

            Console.WriteLine("Listando toda a Pilha:");
            foreach(var p in pilha)
            {
                Console.WriteLine(p);
            }

            Console.WriteLine("Tirando o ultimo que entrou:");
            pilha.Pop();
            Console.WriteLine("Listando toda a Pilha:");
            foreach (var p in pilha)
            {
                Console.WriteLine(p);
            }

            Console.WriteLine("Incluindo 400:");
            pilha.Push(400);
            Console.WriteLine("Listando toda a Pilha:");
            foreach (var p in pilha)
            {
                Console.WriteLine(p);
            }

        }
    }
}
