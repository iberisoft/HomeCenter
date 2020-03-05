using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class HardwareConfig
    {
        public MiHomeConfig MiHome { get; set; }

        public ZigbeeConfig Zigbee { get; set; }

        public HttpConfig Http { get; set; }

        public VirtualConfig Virtual { get; set; }

        public static HardwareConfig FromXml(XElement element)
        {
            var obj = new HardwareConfig();
            if (element.Element("MiHome") != null)
            {
                obj.MiHome = MiHomeConfig.FromXml(element.Element("MiHome"));
            }
            if (element.Element("Zigbee") != null)
            {
                obj.Zigbee = ZigbeeConfig.FromXml(element.Element("Zigbee"));
            }
            if (element.Element("Http") != null)
            {
                obj.Http = HttpConfig.FromXml(element.Element("Http"));
            }
            if (element.Element("Virtual") != null)
            {
                obj.Virtual = VirtualConfig.FromXml(element.Element("Virtual"));
            }
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Configuration",
                MiHome?.ToXml(),
                Zigbee?.ToXml(),
                Http?.ToXml(),
                Virtual?.ToXml());
        }
    }
}
