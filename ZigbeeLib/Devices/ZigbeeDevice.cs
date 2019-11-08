using Newtonsoft.Json.Linq;

namespace ZigbeeLib.Devices
{
    public abstract class ZigbeeDevice
    {
        protected ZigbeeDevice(string sid)
        {
            Sid = sid;
        }

        public string Sid { get; }

        public abstract void ParseData(JObject data);
    }
}
