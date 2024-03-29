﻿using HomeCenter.Config;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HomeExplorer.Services
{
    class ConfigService
    {
        readonly IWebHostEnvironment m_Environment;

        public ConfigService(IWebHostEnvironment environment)
        {
            m_Environment = environment;
        }

        private string ConfigFilePath(string fileName) => Path.Combine(m_Environment.WebRootPath, "config", fileName);

        public IEnumerable<string> GetConfigFiles(string filePattern)
        {
            var configFolderPath = ConfigFilePath("");
            foreach (var filePath in Directory.EnumerateFiles(configFolderPath, filePattern, SearchOption.AllDirectories).OrderBy(filePath => filePath))
            {
                yield return filePath[(configFolderPath.Length + 1)..];
            }
        }

        public string LoadConfig(string fileName) => File.Exists(ConfigFilePath(fileName)) ? File.ReadAllText(ConfigFilePath(fileName)) : "";

        public void SaveConfig(string fileName, string config) => File.WriteAllText(ConfigFilePath(fileName), config);

        public bool IsModified { get; set; }

        public T LoadConfig<T>(string fileName)
            where T : IValidator, new()
        {
            if (File.Exists(ConfigFilePath(fileName)))
            {
                try
                {
                    return ConfigFile.Load<T>(ConfigFilePath(fileName));
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
            return new();
        }

        public void SaveConfig<T>(string fileName, T config)
        {
            try
            {
                ConfigFile.Save(ConfigFilePath(fileName), config);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred when saving file {FileName}", fileName);
            }
        }

        public IEnumerable<T> LoadConfigFolder<T>(string folderName, string filePattern)
            where T : IValidator, new()
        {
            if (Directory.Exists(ConfigFilePath(folderName)))
            {
                foreach (var filePath in Directory.EnumerateFiles(ConfigFilePath(folderName), filePattern, SearchOption.AllDirectories))
                {
                    yield return ConfigFile.Load<T>(filePath);
                }
            }
        }
    }
}
