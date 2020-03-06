using Newtonsoft.Json.Linq;
using System;

namespace ZigbeeLib.Devices.Xiaomi
{
    [ZigbeeDevice("lumi.sensor_switch")]
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
                case "long":
                    OnLongPress?.Invoke(this, EventArgs.Empty);
                    break;
                case "long_release":
                    OnLongRelease?.Invoke(this, EventArgs.Empty);
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

        public event EventHandler OnLongPress;

        public event EventHandler OnLongRelease;

        public override string ToString()
        {
            return $"Status: {Status}, Voltage: {Voltage}V";
        }
    }
}
