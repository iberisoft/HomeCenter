﻿using Newtonsoft.Json.Linq;

namespace HomeCenter.Http
{
    public class ShellyRelay : HttpDevice
    {
        public bool Status()
        {
            var result = SendCommand("");
            if (result != null)
            {
                return (bool)JObject.Parse(result)["ison"];
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
                command = "?turn=" + command;
            return base.SendCommand("relay/0" + command);
        }

        public override string ToString() => $"Status: {Status()}";
    }
}
