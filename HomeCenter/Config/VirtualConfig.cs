using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    class VirtualConfig
    {
        public List<VirtualSwitchConfig> Switches { get; } = new List<VirtualSwitchConfig>();

        public static VirtualConfig FromXml(XElement element)
        {
            var obj = new VirtualConfig();
            obj.Switches.AddRange(element.Elements("Switch").Select(element2 => VirtualSwitchConfig.FromXml(element2)));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Virtual", Switches.Select(@switch => @switch.ToXml()));
        }
    }

    class VirtualSwitchConfig
    {
        public string Name { get; set; }

        public ConsoleKey Key { get; set; }

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
            obj.Name = (string)element.Attribute("Name");
            obj.Key = Enum.Parse<ConsoleKey>((string)element.Attribute("Key"));
            obj.Check();
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Switch",
                new XAttribute("Name", Name),
                Key != 0 ? new XAttribute("Key", Key) : null);
        }
    }
}
