using System;

namespace ZigbeeLib.Devices
{
    [AttributeUsage(AttributeTargets.Class)]
    class ZigbeeDeviceAttribute : Attribute
    {
        public ZigbeeDeviceAttribute(string model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public string Model { get; }
    }
}
