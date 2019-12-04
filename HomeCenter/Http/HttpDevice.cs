using Serilog;
using System;
using System.Net;

namespace HomeCenter.Http
{
    public abstract class HttpDevice
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
                lock (m_WebClient)
                {
                    return m_WebClient.DownloadString(command);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred when connecting to {Address}", BaseAddress);
                return null;
            }
        }

        public static HttpDevice Create(string typeName)
        {
            var type = Type.GetType(typeof(HttpDevice).Namespace + "." + typeName);
            return type != null ? Activator.CreateInstance(type) as HttpDevice : null;
        }
    }
}
