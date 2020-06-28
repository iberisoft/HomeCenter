using Newtonsoft.Json.Linq;

namespace HomeCenter.Http
{
    public class TasmotaRelay : HttpDevice
    {
        public bool Status()
        {
            var result = SendCommand("");
            if (result != null)
            {
                return (string)JObject.Parse(result)["POWER"] == "ON";
            }
            else
            {
                return false;
            }
        }

        public void TurnOn() => SendCommand("on");

        public void TurnOff() => SendCommand("off");

        public void Toggle() => SendCommand("toggle");

        protected override string SendCommand(string command)
        {
            if (command != "")
                command = "%20" + command;
            return base.SendCommand("cm?cmnd=power" + command);
        }

        public override string ToString() => $"Status: {Status()}";
    }
}
