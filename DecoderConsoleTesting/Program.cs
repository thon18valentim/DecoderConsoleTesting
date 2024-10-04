using Microsoft.Extensions.Caching.Memory;
using RJCP.IO.Ports;
using System.Text;

class Program
{
    private static StringBuilder stringBuilder = new StringBuilder(); // Acumulador de dados

    private static MemoryCache cache;
    private static MemoryCacheEntryOptions cacheOptions;

    private static int keyCount = 0;
    private static Queue<int> keysQueue;

    static void Main(string[] args)
    {
        keysQueue = new();

        cache = new MemoryCache(new MemoryCacheOptions()
        {
            SizeLimit = 1024
        });

        cacheOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetPriority(CacheItemPriority.High)
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10))
                .SetSlidingExpiration(TimeSpan.FromSeconds(5));

        // Configurações da porta serial
        string portName = "COM5";
        int baudRate = 9600;

        // Criação do SerialPortStream
        using var serialPort = new SerialPortStream(portName, baudRate);
        try
        {
            // Abrir a porta
            serialPort.Open();
            serialPort.Encoding = Encoding.UTF8;

            Console.WriteLine($"Conectado à {portName} com baudrate {baudRate}.");

            // Assinar o evento DataReceived com o tipo correto de evento
            serialPort.DataReceived += new EventHandler<RJCP.IO.Ports.SerialDataReceivedEventArgs>(DataReceivedHandler);

            // Manter o programa rodando enquanto dados estão sendo recebidos
            Console.WriteLine("Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        finally
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Porta fechada.");
            }
        }
    }

    // Método chamado sempre que novos dados são recebidos na porta serial
    private static void DataReceivedHandler(object sender, RJCP.IO.Ports.SerialDataReceivedEventArgs e)
    {
        var sp = (SerialPortStream)sender;
        var buffer = new byte[sp.BytesToRead];
        var bytesRead = sp.Read(buffer, 0, buffer.Length);
        var inData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        // Acumula os dados recebidos
        stringBuilder.Append(inData);

        // Verifica se a mensagem completa foi recebida (assumindo que termina com "\r\n")
        if (stringBuilder.ToString().Contains("\r\n"))
        {
            // Extrai a mensagem completa
            var data = stringBuilder.ToString().Trim();

            // Limpa o acumulador para a próxima leitura
            stringBuilder.Clear();

            // Processa e exibe a mensagem completa
            //Console.WriteLine($"Recebido: {fullMessage}");
            cache.Set(keyCount, data, cacheOptions);
            keysQueue.Enqueue(keyCount);
            keyCount++;

            try
            {
                if (cache.TryGetValue(keysQueue.Dequeue(), out string? requestData))
                {
                    Console.WriteLine(requestData);
                }
            }
            catch
            {
                // nothing
            }
        }
    }
}
