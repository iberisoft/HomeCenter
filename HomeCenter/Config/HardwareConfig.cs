using System.Xml.Linq;

namespace HomeCenter.Config
{
    class HardwareConfig
    {
        public MiHomeConfig MiHome { get; set; }

        public HttpConfig Http { get; set; }

        public VirtualConfig Virtual { get; set; }

        public static HardwareConfig FromXml(XElement element)
        {
            var obj = new HardwareConfig();
            obj.MiHome = MiHomeConfig.FromXml(element.Element("MiHome"));
            obj.Http = HttpConfig.FromXml(element.Element("Http"));
            obj.Virtual = VirtualConfig.FromXml(element.Element("Virtual"));
            return obj;
        }

        public XElement ToXml()
        {
            return new XElement("Configuration",
                MiHome.ToXml(),
                Http.ToXml(),
                Virtual.ToXml());
        }
    }
}
