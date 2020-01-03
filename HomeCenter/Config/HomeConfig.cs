using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class HomeConfig
    {
        public List<RoomConfig> Rooms { get; } = new List<RoomConfig>();

        public RoomConfig GetRoom(string deviceName) => Rooms.FirstOrDefault(room => room.DeviceNames.Contains(deviceName));

        public static HomeConfig FromXml(XElement element)
        {
            var obj = new HomeConfig();
            obj.Rooms.AddRange(element.Elements("Room").Select(element => RoomConfig.FromXml(element)));
            return obj;
        }
    }

    public class RoomConfig
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> DeviceNames { get; } = new List<string>();

        public void Check()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
        }

        public static RoomConfig FromXml(XElement element)
        {
            var obj = new RoomConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            obj.Description = (string)element.Attribute(nameof(obj.Description));
            obj.DeviceNames.AddRange(element.Elements("Device").Select(element => (string)element.Attribute("Name")));
            obj.Check();
            return obj;
        }
    }
}
