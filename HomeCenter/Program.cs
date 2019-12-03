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

        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Checking hardware...");
            var config = LoadHardwareConfig(Path.Combine(m_ConfigFolderPath, "Hardware.xml"));
            if (await Automation.FindDevices(config))
            {
                SaveHardwareConfig(Path.Combine(m_ConfigFolderPath, "Hardware.xml"), config);
                Log.Information("Hardware configuration updated");
            }
            foreach (var name in Automation.DeviceNames)
            {
                Log.Information("{Name} - {Device}", name, Automation.GetDevice(name));
            }

            var automationConfig = LoadAutomationConfig(Path.Combine(m_ConfigFolderPath, "Automation.xml"));
            Automation.Start(automationConfig);

            Thread.Sleep(Timeout.Infinite);
            await Automation.CloseDevices();
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
