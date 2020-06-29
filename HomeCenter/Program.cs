using HomeCenter.Config;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HomeCenter
{
    static class Program
    {
        static readonly string m_ConfigFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        static readonly Automation m_Automation = new Automation();

        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Checking hardware...");
            var config = LoadHardwareConfig(Path.Combine(m_ConfigFolderPath, "Hardware.xml"));
            if (await m_Automation.FindDevicesAsync(config))
            {
                SaveHardwareConfig(Path.Combine(m_ConfigFolderPath, "Hardware.xml"), config);
                Log.Information("Hardware configuration updated");
            }
            await Task.Delay(1000);
            foreach (var info in m_Automation.GetDeviceInfo())
            {
                Log.Information("{Name} - {Device}", info.Name, info.Device);
            }

            var automationConfig = LoadAutomationConfig(Path.Combine(m_ConfigFolderPath, "Automation.xml"));
            m_Automation.Start(automationConfig);

            Console.ReadKey(true);

            Log.Information("Closing...");
            await m_Automation.CloseDevicesAsync();
        }

        private static HardwareConfig LoadHardwareConfig(string filePath)
        {
            var document = XDocument.Load(filePath);
            return HardwareConfig.FromXml(document.Root);
        }

        private static AutomationConfig LoadAutomationConfig(string filePath)
        {
            var document = XDocument.Load(filePath);
            return AutomationConfig.FromXml(document.Root);
        }

        private static void SaveHardwareConfig(string filePath, HardwareConfig config)
        {
            var document = new XDocument(config.ToXml());
            document.Save(filePath);
        }
    }
}
