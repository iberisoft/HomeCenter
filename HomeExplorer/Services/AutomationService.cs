using HomeCenter;
using HomeCenter.Config;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeExplorer.Services
{
    class AutomationService : IHostedService
    {
        readonly ConfigService m_ConfigService;

        public AutomationService(ConfigService configService)
        {
            m_ConfigService = configService;
        }

        readonly Automation m_Automation = new();
        HomeConfig m_HomeConfig;

        public async Task StartAsync()
        {
            if (IsStarted)
            {
                return;
            }

            Log.Information("Starting service...");
            await DoWork(async () =>
            {
                Log.Information("Checking hardware...");
                var hardwareConfig = m_ConfigService.LoadConfig<HardwareConfig>("hardware.yml");
                if (await m_Automation.FindDevicesAsync(hardwareConfig))
                {
                    m_ConfigService.SaveConfig("hardware.yml", hardwareConfig);
                    Log.Information("Hardware configuration updated");
                }
                await Task.Delay(1000);

                var deviceInfo = m_Automation.GetDeviceInfo();
                Log.Information("Found devices: {Count}", deviceInfo.Count);
                foreach (var info in deviceInfo)
                {
                    Log.Information("{Name} - {Device}", info.Name, info.Device);
                }

                var automationConfig = m_ConfigService.LoadConfig<AutomationConfig>("automation.yml");
                var automationConfigList = m_ConfigService.LoadConfigFolder<AutomationConfig>("automation", "*.yml");
                automationConfig.Triggers.AddRange(automationConfigList.SelectMany(automationConfig => automationConfig.Triggers));
                m_Automation.Start(automationConfig);
                IsStarted = true;

                m_HomeConfig = m_ConfigService.LoadConfig<HomeConfig>("home.yml");
            });
            Log.Information("Service has started");
        }

        public async Task StopAsync()
        {
            if (!IsStarted)
            {
                return;
            }

            Log.Information("Stopping service...");
            await DoWork(async () =>
            {
                m_Automation.Stop();
                IsStarted = false;

                await m_Automation.CloseDevicesAsync();
            });
            Log.Information("Service has stopped");
        }

        public bool IsStarted { get; private set; }

        private async Task DoWork(Func<Task> work)
        {
            try
            {
                IsBusy = true;
                IsBusyChanged?.Invoke(this, EventArgs.Empty);
                await work();
            }
            finally
            {
                IsBusy = false;
                IsBusyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsBusy { get; private set; }

        public event EventHandler IsBusyChanged;

        public List<Automation.DeviceInfo> GetDeviceInfo() => m_Automation.GetDeviceInfo();

        public RoomConfig GetRoom(string deviceName) => m_HomeConfig.GetRoom(deviceName);

        Task IHostedService.StartAsync(CancellationToken cancellationToken) => StartAsync();

        Task IHostedService.StopAsync(CancellationToken cancellationToken) => StopAsync();
    }
}
