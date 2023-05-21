using System;
using System.Collections.Generic;

namespace HomeCenter.Config
{
    public class VirtualConfig : IValidator
    {
        public List<VirtualSwitchConfig> Switches { get; set; } = new();

        public List<VirtualTimerConfig> Timers { get; set; } = new();

        public void Validate()
        {
            foreach (var switchConfig in Switches)
            {
                switchConfig.Validate();
            }
            foreach (var timerConfig in Timers)
            {
                timerConfig.Validate();
            }
        }
    }

    public class VirtualSwitchConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
        }
    }

    public class VirtualTimerConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Interval { get; set; }

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
            if (Interval < 1)
            {
                throw new InvalidOperationException($"{nameof(Interval)} less 1");
            }
        }
    }
}
