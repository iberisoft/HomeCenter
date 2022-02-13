﻿using HomeCenter.Config;
using HomeCenter.Http;
using HomeCenter.Mqtt;
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
using ZigbeeLib;
using ZigbeeLib.Devices;

namespace HomeCenter
{
    public class Automation
    {
        readonly List<MiHome> m_MiHomeObjects = new List<MiHome>();
        readonly List<ZigbeeSniffer> m_ZigbeeSniffers = new List<ZigbeeSniffer>();
        readonly List<MqttBroker> m_MqttBrokers = new List<MqttBroker>();
        readonly Dictionary<string, object> m_Devices = new Dictionary<string, object>();
        readonly Dictionary<string, string> m_DeviceDescriptions = new Dictionary<string, string>();

        public async Task<bool> FindDevicesAsync(HardwareConfig config)
        {
            var modified = false;

            if (config.MiHome != null)
            {
                foreach (var gatewayConfig in config.MiHome.Gateways)
                {
                    var miHome = new MiHome(gatewayConfig.Password, gatewayConfig.Id);
                    m_MiHomeObjects.Add(miHome);
                    await Task.Delay(5000);

                    var gateway = miHome.GetGateway();
                    if (gateway != null)
                    {
                        AddDevice(gatewayConfig.Name, gatewayConfig.Description, gateway);
                        foreach (var device in miHome.GetDevices())
                        {
                            var deviceConfig = gatewayConfig.Devices.SingleOrDefault(deviceConfig => deviceConfig.Id == device.Sid);
                            if (deviceConfig == null)
                            {
                                deviceConfig = CreateDeviceConfig(device);
                                gatewayConfig.Devices.Add(deviceConfig);
                                modified = true;
                            }
                            AddDevice(deviceConfig.Name, deviceConfig.Description, device);
                        }
                    }
                }
            }

            if (config.Mqtt != null)
            {
                foreach (var snifferConfig in config.Mqtt.ZigbeeSniffers)
                {
                    var sniffer = new ZigbeeSniffer();
                    await sniffer.Connect(snifferConfig.Host, snifferConfig.Port);
                    m_ZigbeeSniffers.Add(sniffer);

                    foreach (var device in await sniffer.GetDevices())
                    {
                        var deviceConfig = snifferConfig.Devices.SingleOrDefault(deviceConfig => deviceConfig.Id == device.Sid);
                        if (deviceConfig == null)
                        {
                            deviceConfig = CreateDeviceConfig(device);
                            snifferConfig.Devices.Add(deviceConfig);
                            modified = true;
                        }
                        AddDevice(deviceConfig.Name, deviceConfig.Description, device);
                    }
                }

                foreach (var brokerConfig in config.Mqtt.Brokers)
                {
                    var broker = new MqttBroker();
                    await broker.Connect(brokerConfig.Host, brokerConfig.Port);
                    m_MqttBrokers.Add(broker);

                    foreach (var deviceConfig in brokerConfig.Devices)
                    {
                        var device = MqttDevice.Create(deviceConfig.Type, deviceConfig.Id);
                        if (device != null)
                        {
                            await broker.AddDevice(device);
                            AddDevice(deviceConfig.Name, deviceConfig.Description, device);
                        }
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
                        AddDevice(deviceConfig.Name, deviceConfig.Description, device);
                    }
                }
            }

            if (config.Virtual != null)
            {
                foreach (var switchConfig in config.Virtual.Switches)
                {
                    var @switch = new Virtual.Switch();
                    @switch.SetStatus(switchConfig.Status);
                    AddDevice(switchConfig.Name, switchConfig.Description, @switch);
                }
            }

            return modified;
        }

        private void AddDevice(string name, string description, object device)
        {
            m_Devices.Add(name, device);
            if (description != null)
            {
                m_DeviceDescriptions.Add(name, description);
            }
        }

        private static MiHomeDeviceConfig CreateDeviceConfig(MiHomeDevice device)
        {
            var deviceType = device.GetType().Name;
            return new MiHomeDeviceConfig { Name = deviceType + "_" + device.Sid, Id = device.Sid };
        }

        private static ZigbeeDeviceConfig CreateDeviceConfig(ZigbeeDevice device)
        {
            var deviceType = device.GetType().Name;
            return new ZigbeeDeviceConfig { Name = deviceType + "_" + device.Sid, Id = device.Sid };
        }

        public List<(string Name, object Device, string Description)> GetDeviceInfo() =>
            m_Devices.Keys.Select(name => (name, m_Devices[name], m_DeviceDescriptions.TryGetValue(name, out string description) ? description : null)).ToList();

        private object GetDevice(string name)
        {
            m_Devices.TryGetValue(name, out object device);
            if (device == null)
            {
                Log.Warning("Device {Name} not found", name);
            }
            return device;
        }

        public async Task CloseDevicesAsync()
        {
            foreach (var miHome in m_MiHomeObjects)
            {
                await Task.Run(() => miHome.Dispose());
            }
            m_MiHomeObjects.Clear();

            foreach (var sniffer in m_ZigbeeSniffers)
            {
                await sniffer.Disconnect();
                sniffer.Dispose();
            }
            m_ZigbeeSniffers.Clear();

            foreach (var broker in m_MqttBrokers)
            {
                await broker.Disconnect();
                broker.Dispose();
            }
            m_MqttBrokers.Clear();

            m_Devices.Clear();
            m_DeviceDescriptions.Clear();
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
                        if (triggerConfig.IsActive())
                        {
                            CallTrigger(triggerConfig);
                        }
                    });
                }
            }
        }

        public void Stop()
        {
            UnsubscribeEvents();
        }

        List<(EventInfo EventInfo, Delegate Handler, object Device)> m_SubscribedEvents = new List<(EventInfo EventInfo, Delegate Handler, object Device)>();

        private void SubscribeEvent(EventConfig eventConfig, Action action)
        {
            var device = GetDevice(eventConfig.DeviceName);
            if (device != null)
            {
                var eventInfo = device.GetType().GetEvent(eventConfig.Type);
                if (eventInfo != null)
                {
                    var handler = CreateDelegate(eventInfo, action);
                    eventInfo.AddEventHandler(device, handler);
                    m_SubscribedEvents.Add((eventInfo, handler, device));
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

        private void UnsubscribeEvents()
        {
            foreach (var e in m_SubscribedEvents)
            {
                e.EventInfo.RemoveEventHandler(e.Device, e.Handler);
            }
            m_SubscribedEvents.Clear();
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
                        if (actionConfig.Conditions.All(conditionConfig => CheckCondition(conditionConfig)))
                        {
                            CallAction(actionConfig);
                        }
                        else
                        {
                            break;
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
