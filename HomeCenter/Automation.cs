using HomeCenter.Config;
using HomeCenter.Http;
using MiHomeLib;
using MiHomeLib.Devices;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCenter
{
    public class Automation
    {
        readonly List<MiHome> m_MiHomeObjects = new List<MiHome>();
        readonly Dictionary<string, object> m_Devices = new Dictionary<string, object>();

        public async Task<bool> FindDevices(HardwareConfig config)
        {
            var modified = false;

            if (config.MiHome != null)
            {
                foreach (var gatewayConfig in config.MiHome.Gateways)
                {
                    var miHome = new MiHome(gatewayConfig.Password, gatewayConfig.Id);
                    m_MiHomeObjects.Add(miHome);
                    await Task.Delay(5000);

                    m_Devices.Add(gatewayConfig.Name, miHome.GetGateway());
                    foreach (var device in miHome.GetDevices())
                    {
                        var deviceConfig = gatewayConfig.Devices.SingleOrDefault(deviceConfig2 => deviceConfig2.Id == device.Sid);
                        if (deviceConfig == null)
                        {
                            deviceConfig = CreateDeviceConfig(device);
                            gatewayConfig.Devices.Add(deviceConfig);
                            modified = true;
                        }
                        m_Devices.Add(deviceConfig.Name, device);
                    }
                }
            }
            if (config.Http != null)
            {
                foreach (var deviceConfig in config.Http.Devices)
                {
                    var device = HttpDevice.Create(deviceConfig.Type);
                    if (device != null)
                    {
                        device.Host = deviceConfig.Host;
                        m_Devices.Add(deviceConfig.Name, device);
                    }
                }
            }

            if (config.Virtual != null)
            {
                foreach (var switchConfig in config.Virtual.Switches)
                {
                    var @switch = new Virtual.Switch();
                    @switch.Key = switchConfig.Key;
                    m_Devices.Add(switchConfig.Name, @switch);
                }
            }

            return modified;
        }

        private static MiHomeDeviceConfig CreateDeviceConfig(MiHomeDevice device)
        {
            var deviceType = device.GetType().Name;
            return new MiHomeDeviceConfig { Name = deviceType + "_" + device.Sid, Id = device.Sid };
        }

        public IEnumerable<string> DeviceNames => m_Devices.Keys.OrderBy(name => name);

        public object GetDevice(string name)
        {
            m_Devices.TryGetValue(name, out object device);
            if (device == null)
            {
                Log.Warning("Device {Name} not found", name);
            }
            return device;
        }

        public async Task CloseDevices()
        {
            foreach (var miHome in m_MiHomeObjects)
            {
                miHome.Dispose();
            }
            await Task.Delay(1000);

            m_Devices.Clear();
        }

        public void Start(AutomationConfig config)
        {
            foreach (var triggerConfig in config.Triggers)
            {
                foreach (var eventConfig in triggerConfig.Events)
                {
                    Log.Information("Subscribing event {Event}", eventConfig);
                    SubscribeEvent(eventConfig, () =>
                    {
                        Log.Information("Handling event {Event}", eventConfig);
                        if (triggerConfig.IsActive)
                        {
                            CallTrigger(triggerConfig);
                        }
                    });
                }
            }
        }

        private void SubscribeEvent(EventConfig eventConfig, Action action)
        {
            var device = GetDevice(eventConfig.DeviceName);
            if (device != null)
            {
                var eventInfo = device.GetType().GetEvent(eventConfig.Type);
                if (eventInfo != null)
                {
                    eventInfo.AddEventHandler(device, CreateDelegate(eventInfo, action));
                }
            }
        }

        private static Delegate CreateDelegate(EventInfo eventInfo, Action action)
        {
            var actionCallExpression = Expression.Call(Expression.Constant(action), action.GetType().GetMethod("Invoke"));
            var methodInfo = eventInfo.EventHandlerType.GetMethod("Invoke");
            var lambdaExpression = Expression.Lambda(actionCallExpression, methodInfo.GetParameters().Select(parameterInfo => Expression.Parameter(parameterInfo.ParameterType, "_")));
            return Delegate.CreateDelegate(eventInfo.EventHandlerType, lambdaExpression.Compile(), "Invoke");
        }

        private void CallTrigger(TriggerConfig triggerConfig)
        {
            Task.Run(() =>
            {
                lock (triggerConfig)
                {
                    foreach (var actionConfig in triggerConfig.Actions)
                    {
                        if (actionConfig.Delay > 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(actionConfig.Delay));
                        }
                        if (triggerConfig.Conditions.All(conditionConfig => CheckCondition(conditionConfig)))
                        {
                            CallAction(actionConfig);
                        }
                    }
                }
            });
        }

        private bool CheckCondition(ConditionConfig conditionConfig)
        {
            var success = true;
            var device = GetDevice(conditionConfig.DeviceName);
            if (device != null)
            {
                var propertyInfo = device.GetType().GetProperty(conditionConfig.Property);
                if (propertyInfo != null)
                {
                    var propertyValue = propertyInfo.GetValue(device);
                    var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                    var conditionValue = converter.ConvertFromInvariantString(conditionConfig.Value);
                    success = conditionConfig.Compare(propertyValue, conditionValue);
                }
            }
            if (success)
            {
                Log.Information("Validating condition {Condition}", conditionConfig);
            }
            return success;
        }

        private void CallAction(ActionConfig actionConfig)
        {
            Log.Information("Calling action {Action}", actionConfig);
            var device = GetDevice(actionConfig.DeviceName);
            if (device != null)
            {
                var methodInfo = device.GetType().GetMethod(actionConfig.Command);
                if (methodInfo != null)
                {
                    var parameters = methodInfo.GetParameters().Select(parameterInfo => GetCommandParameter(actionConfig, parameterInfo)).ToArray();
                    methodInfo.Invoke(device, parameters);
                }
            }
        }

        private static object GetCommandParameter(ActionConfig actionConfig, ParameterInfo parameterInfo)
        {
            if (actionConfig.CommandData != null && actionConfig.CommandData.TryGetValue(parameterInfo.Name, out string parameterValue))
            {
                var converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
                return converter.ConvertFromInvariantString(parameterValue);
            }
            if (parameterInfo.HasDefaultValue)
            {
                return parameterInfo.DefaultValue;
            }
            return null;
        }
    }
}
