// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks.Dataflow;

Console.WriteLine("Testando a biblioteca DataFlow!");
Console.WriteLine("Para melhorar o desempenho com paralelismo!");
Console.WriteLine();

//TransformBlock Transforma uma entrada em uma saida para entrada no action

//1 parametro de entrada + 1 parametro de saida + uma funcao

var transformBlock = new TransformBlock<string, string>(Compactar);

//ActionBlock recebe um parametro e executa uma cao
var actionBlock = new ActionBlock<string>(Criptografar,dataflowBlockOptions: new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount});

//Fazer o link entre eles
transformBlock.LinkTo(actionBlock, linkOptions: new DataflowLinkOptions { PropagateCompletion = true});

//alimentar o transformBlock
transformBlock.Post("texto1");
transformBlock.Post("texto2");

//Finalizar
transformBlock.Complete();

//Esperar a finalizacao
await transformBlock.Completion.ContinueWith(t =>
{
    if (t.IsFaulted)
    {
        throw t.Exception;
    }
});


string Compactar(string texto)
{
    string result = $"{texto} compactado";
    Console.WriteLine($"Compactando {texto}...");
    return result;
}

void Criptografar(string texto)
{
    Console.WriteLine($"Criptografando {texto}");
}