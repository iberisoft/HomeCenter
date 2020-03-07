using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZigbeeLib.Devices;

namespace ZigbeeLib
{
    public class ZigbeeSniffer : IDisposable
    {
        IMqttClient m_MqttClient;

        public ZigbeeSniffer()
        {
            m_MqttClient = new MqttFactory().CreateMqttClient();
            m_MqttClient.UseApplicationMessageReceivedHandler(e => OnMessage(e));
        }

        public void Dispose()
        {
            if (m_MqttClient != null)
            {
                m_MqttClient.Dispose();
                m_MqttClient = null;
            }
        }

        public async Task Connect(string host, int? port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(host, port)
                .Build();
            await m_MqttClient.ConnectAsync(options);

            await m_MqttClient.SubscribeAsync("zigbee2mqtt/bridge/config/devices");
            await m_MqttClient.SubscribeAsync("zigbee2mqtt/+");

            await m_MqttClient.PublishAsync("zigbee2mqtt/bridge/config/devices/get");
        }

        public async Task Disconnect()
        {
            await m_MqttClient.DisconnectAsync();
        }

        ConcurrentDictionary<string, ZigbeeDevice> m_Devices = new ConcurrentDictionary<string, ZigbeeDevice>();

        public IEnumerable<ZigbeeDevice> GetDevices() => m_Devices.Values;

        private void OnMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            if (topic == "zigbee2mqtt/bridge/config/devices")
            {
                OnDeviceListMessage(payload);
            }
            else
            {
                OnDeviceEventMessage(topic, payload);
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

        private static string CreateSid(string addr) => addr.Substring(addr.IndexOf("/0x") + 3).TrimStart('0');
    }
}
