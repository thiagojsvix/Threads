using System.Collections.Concurrent;
using System.Diagnostics;

namespace SingleThread;

internal class Program
{
    static void Main(string[] args)
    {
        var medidor = new Stopwatch();

    Start:

        Console.WriteLine(
            """
            Para sair pressionar Enter, ou qualquer uma das opções abaixo de execução:
                1) Processamento Single Thread
                2) Processamento Multi Thread
                3) Processamento Paralelo com Single Thread
                4) Processamento Paralelo com Multi Thread
            """);
        Console.WriteLine();


        var result = char.ToUpperInvariant(Console.ReadKey().KeyChar);

        Console.Clear();

        while (result != '\r')
        {
            switch (result)
            {
                case '1':
                    Processa(() => Fruta().Wait(), () => Cor().Wait());

                    goto Start;
                case '2':
                    Processa(() => Task.WhenAll(Fruta(), Cor()).Wait());

                    goto Start;
                case '3':
                    Processa(Process0);

                    goto Start;

                case '4':
                    Processa(Process1);

                    goto Start;

                case '\r':
                    break;
                default:
                    Console.WriteLine("Opção não encontrada.");
                    Console.WriteLine("Digite 'S'ingle ou 'M'ulti");
                    goto Start;
            }
        }
    }

    private static void Processa(params Action[] actions)
    {
        var medidor = new Stopwatch();

        medidor.Start();

        foreach (var action in actions)
        {
            action();
        }

        medidor.Stop();

        Console.WriteLine();
        Console.WriteLine($"Tempo de execução: {medidor.ElapsedMilliseconds} ms");
        Console.WriteLine();
    }

    private static Task Fruta()
    {
        return Task.Run(() =>
        {
            List<string> frutas = ["Abaxi", "Banana", "Carambola", "Damasco", "Ervilha", "Fruta do Conde", "Goiaba"];
            foreach (var item in frutas)
            {
                Console.WriteLine($"Fruta: {item} -> ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(500);
            }
        });
    }

    private static Task Cor()
    {
        return Task.Run(() =>
        {
            List<string> Cores = ["Azul", "Branco", "Cinza", "Dourado", "Esmeralda", "Fúcsia", "Grená"];
            foreach (var item in Cores)
            {
                Console.WriteLine($"Core: {item} -> ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(500);
            }
        });
    }

    public static void Process0()
    {
        var enumerator = new ListEnumerator();

        Parallel.For(0, 7, (i) =>
        {
            enumerator.EnumerateColorsAsync(i).Wait();
            enumerator.EnumerateFruitsAsync(i).Wait();
        });

        enumerator.Result();
    }

    public static void Process1()
    {
        var enumerator = new ListEnumerator();

        Parallel.For(0, 7, (i) =>
        {
            Task.WhenAll(enumerator.EnumerateColorsAsync(i), enumerator.EnumerateFruitsAsync(i)).Wait();
        });

        enumerator.Result();
    }
}

public class ListEnumerator
{
    readonly ConcurrentBag<string> list = [];

    public async Task EnumerateFruitsAsync(int i)
    {
        List<string> frutas = ["Abaxi", "Banana", "Carambola", "Damasco", "Ervilha", "Fruta do Conde", "Goiaba"];

        // Processa a fruta
        //list.Add($"Thread {Thread.CurrentThread.ManagedThreadId} - Indice: {i} - Fruta {frutas[i]}.");
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Indice: {i} - Fruta {frutas[i]}.");

        // Simula um processamento mais longo
        await Task.Delay(500);
    }

    public async Task EnumerateColorsAsync(int i)
    {
        List<string> Cores = ["Azul", "Branco", "Cinza", "Dourado", "Esmeralda", "Fúcsia", "Grená"];

        // Processa a cor
        //list.Add($"Thread {Thread.CurrentThread.ManagedThreadId} - Indice: {i} - Cor {Cores[i]}.");
        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Indice: {i} - Cor {Cores[i]}.");

        // Simula um processamento mais longo
        await Task.Delay(500);
    }

    public void Result()
    {
        foreach (var item in list)
            Console.WriteLine(item);
    }
}
