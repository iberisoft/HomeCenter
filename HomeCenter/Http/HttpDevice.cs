using Serilog;
using System;
using System.Net.Http;

namespace HomeCenter.Http
{
    public abstract class HttpDevice
    {
        string m_Host;
        readonly HttpClient m_HttpClient = new();

        public string Host
        {
            get => m_Host;
            set
            {
                m_Host = value;
                m_HttpClient.BaseAddress = new(BaseAddress);
            }
        }

        protected virtual string BaseAddress => $"http://{Host}/";

        protected virtual string SendCommand(string command)
        {
            try
            {
                lock (m_HttpClient)
                {
                    return m_HttpClient.GetStringAsync(command).Result;
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
