using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCenter.Mqtt
{
    public abstract class MqttDevice
    {
        protected MqttDevice(string sid)
        {
            Sid = sid;
        }

        public string Sid { get; }

        public abstract string[] Topics { get; }

        public virtual bool SupportsTopic(string topic) => Topics.Contains(topic);

        public abstract Task Initialize();

        public abstract void ParseData(string data);

        public Func<string, string, Task> SendMessage { get; set; }

        public static MqttDevice Create(string typeName, string sid)
        {
            var type = Type.GetType(typeof(MqttDevice).Namespace + "." + typeName);
            return type != null ? Activator.CreateInstance(type, sid) as MqttDevice : null;
        }
    }
}
