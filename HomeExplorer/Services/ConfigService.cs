using HomeCenter.Config;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.IO;
using System.Xml.Linq;

namespace HomeExplorer.Services
{
    class ConfigService
    {
        readonly IWebHostEnvironment m_Environment;

        public ConfigService(IWebHostEnvironment environment)
        {
            m_Environment = environment;
        }

        private string ConfigFilePath(string fileName) => Path.Combine(m_Environment.WebRootPath, fileName);

        public string LoadConfig(string fileName) => File.Exists(ConfigFilePath(fileName)) ? File.ReadAllText(ConfigFilePath(fileName)) : "";

        public void SaveConfig(string fileName, string config) => File.WriteAllText(ConfigFilePath(fileName), config);

        public bool IsModified { get; set; }

        public HardwareConfig LoadHardwareConfig(string fileName) => LoadConfig(fileName, element => HardwareConfig.FromXml(element));

        public HomeConfig LoadHomeConfig(string fileName) => LoadConfig(fileName, element => HomeConfig.FromXml(element));

        public AutomationConfig LoadAutomationConfig(string fileName) => LoadConfig(fileName, element => AutomationConfig.FromXml(element));

        public void SaveHardwareConfig(string fileName, HardwareConfig config) => SaveConfig(fileName, config, config => config.ToXml());

        private T LoadConfig<T>(string fileName, Func<XElement, T> fromXml)
            where T : new()
        {
            if (File.Exists(ConfigFilePath(fileName)))
            {
                try
                {
                    var document = XDocument.Load(ConfigFilePath(fileName));
                    return fromXml(document.Root);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred when loading file {FileName}", fileName);
                }
            }
            else
            {
                Log.Warning("File {FileName} not found", fileName);
            }
            return new T();
        }

        private void SaveConfig<T>(string fileName, T config, Func<T, XElement> toXml)
        {
            try
            {
                var document = new XDocument(toXml(config));
                document.Save(ConfigFilePath(fileName));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred when saving file {FileName}", fileName);
            }
        }
    }
}
