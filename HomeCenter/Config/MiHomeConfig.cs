using System;
using System.Collections.Generic;

namespace HomeCenter.Config
{
    public class MiHomeConfig : IValidator
    {
        public List<MiHomeGatewayConfig> Gateways { get; set; } = new List<MiHomeGatewayConfig>();

        public void Validate()
        {
            foreach (var gatewayConfig in Gateways)
            {
                gatewayConfig.Validate();
            }
        }
    }

    public class MiHomeGatewayConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public string Password { get; set; }

        public List<MiHomeDeviceConfig> Devices { get; set; } = new List<MiHomeDeviceConfig>();

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
            foreach (var deviceConfig in Devices)
            {
                deviceConfig.Validate();
            }
        }
    }

    public class MiHomeDeviceConfig : IValidator
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public void Validate()
        {
            if (Name == null)
            {
                throw new InvalidOperationException($"{nameof(Name)} is null");
            }
            if (Id == null)
            {
                throw new InvalidOperationException($"{nameof(Id)} is null");
            }
        }
    }
}
