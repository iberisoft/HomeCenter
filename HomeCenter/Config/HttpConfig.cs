using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    class HttpConfig
    {
        public List<HttpDeviceConfig> Devices { get; } = new List<HttpDeviceConfig>();

        public static HttpConfig FromXml(XElement element)
        {
            var obj = new HttpConfig();
            obj.Devices.AddRange(element.Elements("Device").Select(element2 => HttpDeviceConfig.FromXml(element2)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Http", Devices.Select(device => device.ToXml()));
        }
    }

    class HttpDeviceConfig
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Host { get; set; }

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
            if (Host == null)
            {
                throw new InvalidOperationException($"{nameof(Host)} is null");
            }
        }

        public static HttpDeviceConfig FromXml(XElement element)
        {
            var obj = new HttpDeviceConfig();
            obj.Name = (string)element.Attribute("Name");
            obj.Type = (string)element.Attribute("Type");
            obj.Host = (string)element.Attribute("Host");
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Device",
                new XAttribute("Name", Name),
                new XAttribute("Type", Type),
                new XAttribute("Host", Host));
        }
    }
}
