using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class HttpConfig
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

    public class HttpDeviceConfig
    {
        public string Name { get; set; }

        public string Description { get; set; }

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
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Description = (string)element.Attribute(nameof(obj.Description));
            obj.Type = (string)element.Attribute(nameof(obj.Type));
            obj.Host = (string)element.Attribute(nameof(obj.Host));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Device",
                new XAttribute(nameof(Name), Name),
                Description != null ? new XAttribute(nameof(Description), Description) : null,
                new XAttribute(nameof(Type), Type),
                new XAttribute(nameof(Host), Host));
        }
    }
}
