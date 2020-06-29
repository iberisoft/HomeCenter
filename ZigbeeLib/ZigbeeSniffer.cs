using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZigbeeLib.Devices;

namespace ZigbeeLib
{
    public class ZigbeeSniffer : IDisposable
    {
        NetClient m_NetClient;

        public ZigbeeSniffer()
        {
            m_NetClient = new NetClient("zigbee2mqtt");
            m_NetClient.MessageReceived += NetClient_MessageReceived;
        }

        public void Dispose()
        {
            if (m_NetClient != null)
            {
                m_NetClient.Dispose();
                m_NetClient = null;
            }
        }

        public async Task Connect(string host, int? port)
        {
            await m_NetClient.StartAsync(host, port);

            await m_NetClient.SubscribeAsync("bridge/config/devices");
            await m_NetClient.SubscribeAsync("+");
            await m_NetClient.PublishAsync("bridge/config/devices/get");
        }

        public async Task Disconnect()
        {
            await m_NetClient.StopAsync();
        }

        ConcurrentDictionary<string, ZigbeeDevice> m_Devices = new ConcurrentDictionary<string, ZigbeeDevice>();
        TaskCompletionSource<IEnumerable<ZigbeeDevice>> m_DevicesSource = new TaskCompletionSource<IEnumerable<ZigbeeDevice>>();

        public async Task<IEnumerable<ZigbeeDevice>> GetDevices() => await m_DevicesSource.Task;

        private void NetClient_MessageReceived(object sender, NetClient.Message e)
        {
            if (e.Topic == "bridge/config/devices")
            {
                OnDeviceListMessage(e.Payload);
            }
            else
            {
                OnDeviceEventMessage(e.Topic, e.Payload);
            }
        }

        private void OnDeviceListMessage(string payload)
        {
            foreach (var device in JArray.Parse(payload).Where(device => (string)device["type"] == "EndDevice"))
            {
                var sid = CreateSid((string)device["ieeeAddr"]);
                if (!m_Devices.ContainsKey(sid))
                {
                    var model = (string)device["modelID"];
                    var type = ZigbeeDeviceAttribute.GetDeviceType(model);
                    if (type != null)
                    {
                        m_Devices[sid] = (ZigbeeDevice)Activator.CreateInstance(type, sid);
                    }
                }
            }
            m_DevicesSource.SetResult(m_Devices.Values);
        }

        private void OnDeviceEventMessage(string topic, string payload)
        {
            var sid = CreateSid(topic);
            if (m_Devices.TryGetValue(sid, out ZigbeeDevice device))
            {
                var data = JObject.Parse(payload);
                device.ParseData(data);
            }
        }

        private static string CreateSid(string addr) => addr.Substring(2).TrimStart('0');
    }
}
