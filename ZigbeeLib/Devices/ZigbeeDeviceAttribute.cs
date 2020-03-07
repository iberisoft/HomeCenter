using System;
using System.Collections.Generic;
using System.Linq;

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

        static List<Type> m_DeviceTypes;

        public static Type GetDeviceType(string model)
        {
            if (m_DeviceTypes == null)
            {
                var baseDeviceType = typeof(ZigbeeDevice);
                var namespacePrefix = baseDeviceType.Namespace + ".";
                m_DeviceTypes = baseDeviceType.Assembly.GetTypes().Where(type => type.Namespace.StartsWith(namespacePrefix)).ToList();
            }
            return m_DeviceTypes.FirstOrDefault(type => GetAttribute(type)?.Model == model);
        }

        private static ZigbeeDeviceAttribute GetAttribute(Type type) => (ZigbeeDeviceAttribute)type.GetCustomAttributes(typeof(ZigbeeDeviceAttribute), true).SingleOrDefault();
    }
}
