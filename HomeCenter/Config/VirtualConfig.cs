using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class VirtualConfig
    {
        public List<VirtualSwitchConfig> Switches { get; } = new List<VirtualSwitchConfig>();

        public static VirtualConfig FromXml(XElement element)
        {
            var obj = new VirtualConfig();
            obj.Switches.AddRange(element.Elements("Switch").Select(element => VirtualSwitchConfig.FromXml(element)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Virtual", Switches.Select(@switch => @switch.ToXml()));
        }
    }

    public class VirtualSwitchConfig
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public void Check()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
        }

        public static VirtualSwitchConfig FromXml(XElement element)
        {
            var obj = new VirtualSwitchConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Description = (string)element.Attribute(nameof(obj.Description));
            obj.Status = (string)element.Attribute(nameof(obj.Status));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Switch",
                new XAttribute(nameof(Name), Name),
                Description != null ? new XAttribute(nameof(Description), Description) : null,
                Status != null ? new XAttribute(nameof(Status), Status) : null);
        }
    }
}
