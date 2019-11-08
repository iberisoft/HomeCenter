using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    class ZigbeeConfig
    {
        public List<ZigbeeSnifferConfig> Sniffers { get; } = new List<ZigbeeSnifferConfig>();

        public static ZigbeeConfig FromXml(XElement element)
        {
            var obj = new ZigbeeConfig();
            obj.Sniffers.AddRange(element.Elements("Sniffer").Select(element2 => ZigbeeSnifferConfig.FromXml(element2)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Zigbee", Sniffers.Select(sniffer => sniffer.ToXml()));
        }
    }

    class ZigbeeSnifferConfig
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
            obj.Devices.AddRange(element.Elements("Device").Select(element2 => ZigbeeDeviceConfig.FromXml(element2)));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Sniffer",
                new XAttribute(nameof(Host), Host),
                Port != null ? new XAttribute(nameof(Port), Port) : null,
                Devices.Select(device => device.ToXml()));
        }
    }

    class ZigbeeDeviceConfig
    {
        public string Name { get; set; }

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
            obj.Id = (string)element.Attribute(nameof(obj.Id));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Device",
                new XAttribute(nameof(Name), Name),
                new XAttribute(nameof(Id), Id));
        }
    }
}
