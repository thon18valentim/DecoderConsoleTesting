using RJCP.IO.Ports;
using System;
using System.Text;

class Program
{
    private static StringBuilder stringBuilder = new StringBuilder(); // Acumulador de dados

    static void Main(string[] args)
    {
        // Configurações da porta serial
        string portName = "COM5";
        int baudRate = 9600;

        // Criação do SerialPortStream
        using (var serialPort = new SerialPortStream(portName, baudRate))
        {
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
    }

    // Método chamado sempre que novos dados são recebidos na porta serial
    private static void DataReceivedHandler(object sender, RJCP.IO.Ports.SerialDataReceivedEventArgs e)
    {
        SerialPortStream sp = (SerialPortStream)sender;
        byte[] buffer = new byte[sp.BytesToRead];
        int bytesRead = sp.Read(buffer, 0, buffer.Length);
        string inData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        // Acumula os dados recebidos
        stringBuilder.Append(inData);

        // Verifica se a mensagem completa foi recebida (assumindo que termina com "\r\n")
        if (stringBuilder.ToString().Contains("\r\n"))
        {
            // Extrai a mensagem completa
            string fullMessage = stringBuilder.ToString().Trim();

            // Limpa o acumulador para a próxima leitura
            stringBuilder.Clear();

            // Processa e exibe a mensagem completa
            Console.WriteLine($"Recebido: {fullMessage}");
        }
    }
}
