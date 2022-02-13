using System;
using System.Collections.Generic;

namespace HomeCenter.Config
{
    public class VirtualConfig : IValidator
    {
        public List<VirtualSwitchConfig> Switches { get; set; } = new List<VirtualSwitchConfig>();

        public void Validate()
        {
            foreach (var switchConfig in Switches)
            {
                switchConfig.Validate();
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
}
