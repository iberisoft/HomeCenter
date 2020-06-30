using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class MqttConfig
    {
        public List<ZigbeeSnifferConfig> ZigbeeSniffers { get; } = new List<ZigbeeSnifferConfig>();

        public List<MqttBrokertConfig> Brokers { get; } = new List<MqttBrokertConfig>();

        public static MqttConfig FromXml(XElement element)
        {
            var obj = new MqttConfig();
            obj.ZigbeeSniffers.AddRange(element.Elements("ZigbeeSniffer").Select(element => ZigbeeSnifferConfig.FromXml(element)));
            obj.Brokers.AddRange(element.Elements("Broker").Select(element => MqttBrokertConfig.FromXml(element)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Mqtt",
                ZigbeeSniffers.Select(sniffer => sniffer.ToXml()),
                Brokers.Select(broker => broker.ToXml()));
        }
    }

    public class ZigbeeSnifferConfig
    {
        public string Host { get; set; }

        public int? Port { get; set; }

        public List<ZigbeeDeviceConfig> Devices { get; } = new List<ZigbeeDeviceConfig>();

        public void Check()
        {
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
        }

        public static ZigbeeSnifferConfig FromXml(XElement element)
        {
            var obj = new ZigbeeSnifferConfig();
            obj.Host = (string)element.Attribute(nameof(obj.Host));
            obj.Port = (int?)element.Attribute(nameof(obj.Port));
            obj.Devices.AddRange(element.Elements("Device").Select(element => ZigbeeDeviceConfig.FromXml(element)));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("ZigbeeSniffer",
                new XAttribute(nameof(Host), Host),
                Port != null ? new XAttribute(nameof(Port), Port) : null,
                Devices.Select(device => device.ToXml()));
        }
    }

    public class ZigbeeDeviceConfig
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public void Check()
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

        public static ZigbeeDeviceConfig FromXml(XElement element)
        {
            var obj = new ZigbeeDeviceConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Description = (string)element.Attribute(nameof(obj.Description));
            obj.Id = (string)element.Attribute(nameof(obj.Id));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Device",
                new XAttribute(nameof(Name), Name),
                Description != null ? new XAttribute(nameof(Description), Description) : null,
                new XAttribute(nameof(Id), Id));
        }
    }

    public class MqttBrokertConfig
    {
        public string Host { get; set; }

        public int? Port { get; set; }

        public List<MqttDeviceConfig> Devices { get; } = new List<MqttDeviceConfig>();

        public void Check()
        {
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
        }

        public static MqttBrokertConfig FromXml(XElement element)
        {
            var obj = new MqttBrokertConfig();
            obj.Host = (string)element.Attribute(nameof(obj.Host));
            obj.Port = (int?)element.Attribute(nameof(obj.Port));
            obj.Devices.AddRange(element.Elements("Device").Select(element => MqttDeviceConfig.FromXml(element)));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Broker",
                new XAttribute(nameof(Host), Host),
                Port != null ? new XAttribute(nameof(Port), Port) : null,
                Devices.Select(device => device.ToXml()));
        }
    }

    public class MqttDeviceConfig
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

        public void Check()
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

        public static MqttDeviceConfig FromXml(XElement element)
        {
            var obj = new MqttDeviceConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Description = (string)element.Attribute(nameof(obj.Description));
            obj.Type = (string)element.Attribute(nameof(obj.Type));
            obj.Id = (string)element.Attribute(nameof(obj.Id));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Device",
                new XAttribute(nameof(Name), Name),
                Description != null ? new XAttribute(nameof(Description), Description) : null,
                new XAttribute(nameof(Type), Type),
                new XAttribute(nameof(Id), Id));
        }
    }
}
