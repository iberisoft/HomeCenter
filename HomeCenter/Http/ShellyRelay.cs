using Newtonsoft.Json;

namespace HomeCenter.Http
{
    public class ShellyRelay : Device
    {
        public bool Status
        {
            get
            {
                var result = SendCommand("");
                if (result != null)
                {
                    dynamic resultObject = JsonConvert.DeserializeObject(result);
                    return resultObject.ison;
                }
                else
                {
                    return false;
                }
            }
        }

        public void TurnOn() => SendCommand("on");

        public void TurnOff() => SendCommand("off");

        protected override string SendCommand(string command)
        {
            if (command != "")
                command = "?turn=" + command;
            return base.SendCommand($"relay/0" + command);
        }

        public override string ToString() => $"Status: {Status}";
    }
}
