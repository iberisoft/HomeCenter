using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.Config
{
    public class HomeConfig : IValidator
    {
        public List<RoomConfig> Rooms { get; set; } = new List<RoomConfig>();

        public RoomConfig GetRoom(string deviceName) => Rooms.FirstOrDefault(roomConfig => roomConfig.DeviceNames.Contains(deviceName));

        public void Validate()
        {
            foreach (var roomConfig in Rooms)
            {
                roomConfig.Validate();
            }
        }
    }

    public class RoomConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> DeviceNames { get; set; } = new List<string>();

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
        }
    }
}
