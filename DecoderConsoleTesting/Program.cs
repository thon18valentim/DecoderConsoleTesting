using DecoderConsoleTesting;
using System.IO.Ports;

Console.WriteLine("Inicializando teste...");

var appSettings = SettingsLoader.LoadSettings();
var serialPort = new SerialPort(appSettings.SerialPort, appSettings.BaudRate, Parity.None, 8, StopBits.One);

Console.WriteLine("Configurações OK...");

try
{
    serialPort.Open();

    var records = 0;
    while (records < appSettings.Records)
    {
        if (serialPort.BytesToRead > 0)
        {
            Console.WriteLine(serialPort.ReadLine());
            records++;
        }
        else
        {
            Console.WriteLine("[ Nenhum byte recebido ]");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    Console.ReadLine();
}
finally
{
    serialPort.Close();
    Console.WriteLine("Teste finalizado...");
}