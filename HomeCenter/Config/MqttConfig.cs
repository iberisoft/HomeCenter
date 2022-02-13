using System;
using System.Collections.Generic;

namespace HomeCenter.Config
{
    public class MqttConfig : IValidator
    {
        public List<ZigbeeSnifferConfig> ZigbeeSniffers { get; set; } = new List<ZigbeeSnifferConfig>();

        public List<MqttBrokertConfig> Brokers { get; set; } = new List<MqttBrokertConfig>();

        public void Validate()
        {
            foreach (var snifferConfig in ZigbeeSniffers)
            {
                snifferConfig.Validate();
            }
            foreach (var brokerConfig in Brokers)
            {
                brokerConfig.Validate();
            }
        }
    }

    public class ZigbeeSnifferConfig : IValidator
    {
        public string Host { get; set; }

        public int? Port { get; set; }

        public List<ZigbeeDeviceConfig> Devices { get; set; } = new List<ZigbeeDeviceConfig>();

        public void Validate()
        {
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
            foreach (var deviceConfig in Devices)
            {
                deviceConfig.Validate();
            }
        }
    }

    public class ZigbeeDeviceConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
            if (Id == null)
            {
                throw new InvalidOperationException($"{nameof(Id)} is null");
            }
        }
    }

    public class MqttBrokertConfig : IValidator
    {
        public string Host { get; set; }

        public int? Port { get; set; }

        public List<MqttDeviceConfig> Devices { get; set; } = new List<MqttDeviceConfig>();

        public void Validate()
        {
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
            foreach (var deviceConfig in Devices)
            {
                deviceConfig.Validate();
            }
        }
    }

    public class MqttDeviceConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

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
            if (Id == null)
            {
                throw new InvalidOperationException($"{nameof(Id)} is null");
            }
        }
    }
}
