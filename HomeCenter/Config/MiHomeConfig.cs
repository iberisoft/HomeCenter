using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class MiHomeConfig
    {
        public List<MiHomeGatewayConfig> Gateways { get; } = new List<MiHomeGatewayConfig>();

        public static MiHomeConfig FromXml(XElement element)
        {
            var obj = new MiHomeConfig();
            obj.Gateways.AddRange(element.Elements("Gateway").Select(element2 => MiHomeGatewayConfig.FromXml(element2)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("MiHome", Gateways.Select(gateway => gateway.ToXml()));
        }
    }

    public class MiHomeGatewayConfig
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string Password { get; set; }

        public List<MiHomeDeviceConfig> Devices { get; } = new List<MiHomeDeviceConfig>();

        public void Check()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
        }

        public static MiHomeGatewayConfig FromXml(XElement element)
        {
            var obj = new MiHomeGatewayConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Id = (string)element.Attribute(nameof(obj.Id));
            obj.Password = (string)element.Attribute(nameof(obj.Password));
            obj.Devices.AddRange(element.Elements("Device").Select(element2 => MiHomeDeviceConfig.FromXml(element2)));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Gateway",
                new XAttribute(nameof(Name), Name),
                Id != null ? new XAttribute(nameof(Id), Id) : null,
                Password != null ? new XAttribute(nameof(Password), Password) : null,
                Devices.Select(device => device.ToXml()));
        }
    }

    public class MiHomeDeviceConfig
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

        public static MiHomeDeviceConfig FromXml(XElement element)
        {
            var obj = new MiHomeDeviceConfig();
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
