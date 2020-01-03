using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HomeCenter.Config
{
    public class AutomationConfig
    {
        public List<TriggerConfig> Triggers { get; } = new List<TriggerConfig>();

        public static AutomationConfig FromXml(XElement element)
        {
            var obj = new AutomationConfig();
            obj.Triggers.AddRange(element.Elements("Trigger").Select(element => TriggerConfig.FromXml(element)));
            return obj;
        }
    }

    public class TriggerConfig
    {
        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool IsActive
        {
            get
            {
                var time = DateTime.Now.TimeOfDay;
                if (StartTime != null && EndTime != null)
                {
                    return StartTime <= EndTime ? time >= StartTime && time < EndTime : time >= EndTime || time < StartTime;
                }
                if (StartTime != null && EndTime == null)
                {
                    return time >= StartTime;
                }
                if (StartTime == null && EndTime != null)
                {
                    return time < EndTime;
                }
                return true;
            }
        }

        public List<EventConfig> Events { get; } = new List<EventConfig>();

        public List<ConditionConfig> Conditions { get; } = new List<ConditionConfig>();

        public List<ActionConfig> Actions { get; } = new List<ActionConfig>();

        public static TriggerConfig FromXml(XElement element)
        {
            var obj = new TriggerConfig();
            obj.Name = (string)element.Attribute(nameof(obj.Name));
            if (element.Attribute(nameof(obj.StartTime)) != null)
            {
                obj.StartTime = TimeSpan.Parse((string)element.Attribute(nameof(obj.StartTime)));
            }
            if (element.Attribute(nameof(obj.EndTime)) != null)
            {
                obj.EndTime = TimeSpan.Parse((string)element.Attribute(nameof(obj.EndTime)));
            }
            obj.Events.AddRange(element.Elements("Event").Select(element => EventConfig.FromXml(element)));
            obj.Conditions.AddRange(element.Elements("Condition").Select(element => ConditionConfig.FromXml(element)));
            obj.Actions.AddRange(element.Elements("Action").Select(element => ActionConfig.FromXml(element)));
            return obj;
        }

        public override string ToString() => Name;
    }

    public class EventConfig
    {
        public string DeviceName { get; set; }

        public string Type { get; set; }

        public void Check()
        {
            if (DeviceName == null)
            {
                throw new InvalidOperationException($"{nameof(DeviceName)} is null");
            }
            if (Type == null)
            {
                throw new InvalidOperationException($"{nameof(Type)} is null");
            }
        }

        public static EventConfig FromXml(XElement element)
        {
            var obj = new EventConfig();
            obj.DeviceName = (string)element.Attribute(nameof(obj.DeviceName));
            obj.Type = (string)element.Attribute(nameof(obj.Type));
            obj.Check();
            return obj;
        }

        public override string ToString() => $"{DeviceName}.{Type}";
    }

    public class ConditionConfig
    {
        public string DeviceName { get; set; }

        public string Property { get; set; }

        public string Value { get; set; }

        public int Comparison { get; set; }

        public char ComparisonAsChar => "<=>"[Comparison + 1];

        public bool Compare(object op1, object op2)
        {
            if (Comparison == 0)
            {
                return Equals(op1, op2);
            }

            var comp1 = op1 as IComparable;
            var comp2 = op2 as IComparable;
            return comp1 != null && comp2 != null && comp1.CompareTo(comp2) == Comparison;
        }

        public void Check()
        {
            if (DeviceName == null)
            {
                throw new InvalidOperationException($"{nameof(DeviceName)} is null");
            }
            if (Property == null)
            {
                throw new InvalidOperationException($"{nameof(Property)} is null");
            }
            if (Value == null)
            {
                throw new InvalidOperationException($"{nameof(Value)} is null");
            }
            if (Comparison < -1 || Comparison > 1)
            {
                throw new InvalidOperationException($"{nameof(Comparison)} must be -1, 0, 1");
            }
        }

        public static ConditionConfig FromXml(XElement element)
        {
            var obj = new ConditionConfig();
            obj.DeviceName = (string)element.Attribute(nameof(obj.DeviceName));
            obj.Property = (string)element.Attribute(nameof(obj.Property));
            obj.Value = (string)element.Attribute(nameof(obj.Value));
            obj.Comparison = (int?)element.Attribute(nameof(obj.Comparison)) ?? 0;
            obj.Check();
            return obj;
        }

        public override string ToString() => $"{DeviceName}.{Property} {ComparisonAsChar} {Value}";
    }

    public class ActionConfig
    {
        public string DeviceName { get; set; }

        public string Command { get; set; }

        public Dictionary<string, string> CommandData { get; private set; }

        public float Delay { get; set; }

        public void Check()
        {
            if (DeviceName == null)
            {
                throw new InvalidOperationException($"{nameof(DeviceName)} is null");
            }
            if (Command == null)
            {
                throw new InvalidOperationException($"{nameof(Command)} is null");
            }
            if (Delay < 0)
            {
                throw new InvalidOperationException($"{nameof(Delay)} less 0");
            }
        }

        public static ActionConfig FromXml(XElement element)
        {
            var obj = new ActionConfig();
            obj.DeviceName = (string)element.Attribute(nameof(obj.DeviceName));
            obj.Command = (string)element.Attribute(nameof(obj.Command));
            obj.CommandData = element.Element(nameof(obj.CommandData))?.Attributes().ToDictionary(attr => attr.Name.LocalName, attr => attr.Value);
            obj.Delay = (float?)element.Attribute(nameof(obj.Delay)) ?? 0;
            obj.Check();
            return obj;
        }

        public override string ToString() => $"{DeviceName}.{Command}";
    }
}
