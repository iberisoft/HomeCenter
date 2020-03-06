using Newtonsoft.Json.Linq;
using System;

namespace ZigbeeLib.Devices.Xiaomi
{
    [ZigbeeDevice("lumi.sensor_ht")]
    class ThSensor : ZigbeeDevice
    {
        public ThSensor(string sid)
            : base(sid) { }

        public override void ParseData(JObject data)
        {
            var oldTemperature = Temperature;
            Temperature = (float?)data["temperature"];
            if (oldTemperature != Temperature)
            {
                OnTemperatureChange?.Invoke(this, EventArgs.Empty);
            }

            var oldHumidity = Humidity;
            Humidity = (float?)data["humidity"];
            if (oldHumidity != Humidity)
            {
                OnHumidityChange?.Invoke(this, EventArgs.Empty);
            }

            Voltage = (float?)data["voltage"] / 1000;
        }

        public float? Temperature { get; private set; }

        public float? Humidity { get; private set; }

        public float? Voltage { get; private set; }

        public event EventHandler OnTemperatureChange;

        public event EventHandler OnHumidityChange;

        public override string ToString()
        {
            return $"Temperature: {Temperature}°C, Humidity: {Humidity}%, Voltage: {Voltage}V";
        }
    }
}
