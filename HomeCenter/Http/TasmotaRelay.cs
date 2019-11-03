using Newtonsoft.Json;

namespace HomeCenter.Http
{
    public class TasmotaRelay : HttpDevice
    {
        public bool Status
        {
            get
            {
                var result = SendCommand("");
                if (result != null)
                {
                    dynamic resultObject = JsonConvert.DeserializeObject(result);
                    return resultObject.Power == "ON";
                }
                else
                {
                    return false;
                }
            }
        }

        public void TurnOn() => SendCommand("on");

        public void TurnOff() => SendCommand("off");

        public void Toggle() => SendCommand("toggle");

        protected override string SendCommand(string command)
        {
            if (command != "")
                command = "%20" + command;
            return base.SendCommand($"cm?cmnd=power" + command);
        }

        public override string ToString() => $"Status: {Status}";
    }
}
