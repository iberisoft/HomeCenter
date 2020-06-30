using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

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

        public Func<JObject, Task> SendMessage { get; set; }
    }
}
