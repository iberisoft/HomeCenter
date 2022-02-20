using MqttHelper;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCenter.Mqtt
{
    public class MqttBroker : IDisposable
    {
        NetClient m_NetClient;

        public MqttBroker()
        {
            m_NetClient = new();
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
        }

        public async Task Disconnect()
        {
            await m_NetClient.StopAsync();
        }

        ConcurrentBag<MqttDevice> m_Devices = new();

        public async Task AddDevice(MqttDevice device)
        {
            device.SendMessage = (topic, data) => m_NetClient.PublishAsync(topic, data);
            m_Devices.Add(device);

            foreach (var topic in device.Topics)
            {
                await m_NetClient.SubscribeAsync(topic);
            }
            await Task.Delay(100);
            await device.Initialize();
        }

        private void NetClient_MessageReceived(object sender, NetMessage e)
        {
            foreach (var device in m_Devices.Where(device => device.SupportsTopic(e.Topic)))
            {
                device.ParseData(e.Payload);
            }
        }
    }
}
