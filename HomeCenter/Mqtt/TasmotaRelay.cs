using System;
using System.Threading.Tasks;

namespace HomeCenter.Mqtt
{
    public class TasmotaRelay : MqttDevice
    {
        public TasmotaRelay(string sid)
            : base(sid) { }

        public override string[] Topics => new[] { $"stat/{Sid}/POWER" };

        public override async Task Initialize()
        {
            await SendCommand(null);
        }

        public override void ParseData(string data)
        {
            Status = data == "ON";
            OnStatusChange?.Invoke(this, EventArgs.Empty);
        }

        public bool Status { get; private set; }

        public event EventHandler OnStatusChange;

        public async Task TurnOn() => await SendCommand("on");

        public async Task TurnOff() => await SendCommand("off");

        public async Task Toggle() => await SendCommand("toggle");

        private async Task SendCommand(string command) => await SendMessage($"cmnd/{Sid}/POWER", command);

        public override string ToString() => $"Status: {Status}";
    }
}
