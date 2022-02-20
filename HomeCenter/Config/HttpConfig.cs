using System;
using System.Collections.Generic;

namespace HomeCenter.Config
{
    public class HttpConfig : IValidator
    {
        public List<HttpDeviceConfig> Devices { get; set; } = new();

        public void Validate()
        {
            foreach (var deviceConfig in Devices)
            {
                deviceConfig.Validate();
            }
        }
    }

    public class HttpDeviceConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Host { get; set; }

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
            if (Type == null)
            {
                throw new InvalidOperationException($"{nameof(Type)} is null");
            }
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
        }
    }
}
