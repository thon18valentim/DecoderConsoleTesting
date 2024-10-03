using DecoderConsoleTesting;
using System.IO.Ports;

Console.WriteLine("Inicializando teste...");

var appSettings = SettingsLoader.LoadSettings();
var serialPort = new SerialPort(appSettings.SerialPort, appSettings.BaudRate, Parity.None, 8, StopBits.One)
{
    ReadTimeout = 1000,
    Encoding = System.Text.Encoding.UTF8
};

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