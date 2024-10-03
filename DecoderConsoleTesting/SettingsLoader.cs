using Newtonsoft.Json;

namespace DecoderConsoleTesting
{
    internal class SettingsLoader
    {
        public static AppSettings LoadSettings()
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(directory, "settings.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("App Settings NOT found.");

            var jsonString = File.ReadAllText(filePath);
            var appSettings = JsonConvert.DeserializeObject<AppSettings>(jsonString)
                ?? throw new NullReferenceException("Settings NOT found");

            return appSettings;
        }
    }
}
