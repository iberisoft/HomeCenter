using Newtonsoft.Json.Linq;
using System;

namespace ZigbeeLib.Devices.Xiaomi
{
    [ZigbeeDevice("lumi.sensor_switch.aq2")]
    class Switch : ZigbeeDevice
    {
        public Switch(string sid)
            : base(sid) { }

        public override void ParseData(JObject data)
        {
            Status = (string)data["click"];
            switch (Status)
            {
                case "single":
                    OnClick?.Invoke(this, EventArgs.Empty);
                    break;
                case "double":
                    OnDoubleClick?.Invoke(this, EventArgs.Empty);
                    break;
                case "triple":
                    OnTripleClick?.Invoke(this, EventArgs.Empty);
                    break;
                case "quadruple":
                    OnQuadrupleClick?.Invoke(this, EventArgs.Empty);
                    break;
            }

            Voltage = (float?)data["voltage"] / 1000;
        }

        public string Status { get; private set; }

        public float? Voltage { get; private set; }

        public event EventHandler OnClick;

        public event EventHandler OnDoubleClick;

        public event EventHandler OnTripleClick;

        public event EventHandler OnQuadrupleClick;

        public override string ToString()
        {
            return $"Status: {Status}, Voltage: {Voltage}V";
        }
    }
}
