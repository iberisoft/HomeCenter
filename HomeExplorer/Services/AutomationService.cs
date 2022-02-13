using HomeCenter;
using HomeCenter.Config;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeExplorer.Services
{
    class AutomationService
    {
        readonly ConfigService m_ConfigService;

        public AutomationService(ConfigService configService)
        {
            m_ConfigService = configService;
        }

        public void Start() => Task.Run(async () => await StartAsync());

        public void Stop() => Task.Run(async () => await StopAsync());

        readonly Automation m_Automation = new Automation();
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

        public List<(string Name, object Device, string Description)> GetDeviceInfo() => m_Automation.GetDeviceInfo();

        public RoomConfig GetRoom(string deviceName) => m_HomeConfig.GetRoom(deviceName);
    }
}
