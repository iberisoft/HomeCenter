using System;
using System.Net;

namespace HomeCenter.Http
{
    public abstract class Device
    {
        string m_Host;
        readonly WebClient m_WebClient = new WebClient();

        public string Host
        {
            get => m_Host;
            set
            {
                m_Host = value;
                m_WebClient.BaseAddress = BaseAddress;
            }
        }

        protected virtual string BaseAddress => $"http://{Host}/";

        protected virtual string SendCommand(string command)
        {
            try
            {
                return m_WebClient.DownloadString(command);
            }
            catch
            {
                return null;
            }
        }

        public static Device Create(string typeName)
        {
            var type = Type.GetType(typeof(Device).Namespace + "." + typeName);
            return type != null ? Activator.CreateInstance(type) as Device : null;
        }
    }
}
