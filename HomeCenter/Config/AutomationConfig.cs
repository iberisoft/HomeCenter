using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeCenter.Config
{
    public class AutomationConfig : IValidator
    {
        public List<TriggerConfig> Triggers { get; set; } = new List<TriggerConfig>();

        public void Validate()
        {
            foreach (var triggerConfig in Triggers)
            {
                triggerConfig.Validate();
            }
        }
    }

    public class TriggerConfig : IValidator
    {
        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool IsActive()
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

        public List<EventConfig> Events { get; set; } = new List<EventConfig>();

        public List<ActionConfig> Actions { get; set; } = new List<ActionConfig>();

        public void Validate()
        {
            foreach (var eventConfig in Events)
            {
                eventConfig.Validate();
            }
            foreach (var actionConfig in Actions)
            {
                actionConfig.Validate();
            }
        }

        public override string ToString() => Name;
    }

    public class EventConfig : IValidator
    {
        public string DeviceName { get; set; }

        public string Type { get; set; }

        public void Validate()
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

        public override string ToString() => $"{DeviceName}.{Type}";
    }

    public class ConditionConfig : IValidator
    {
        public string DeviceName { get; set; }

        public string Property { get; set; }

        public string Value { get; set; }

        public int Comparison { get; set; }

        private char ComparisonAsChar => "<=>"[Comparison + 1];

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

        public void Validate()
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

        public override string ToString() => $"{DeviceName}.{Property} {ComparisonAsChar} {Value}";
    }

    public class ActionConfig : IValidator
    {
        public string DeviceName { get; set; }

        public string Command { get; set; }

        public Dictionary<string, string> CommandData { get; set; }

        public float Delay { get; set; }

        public List<ConditionConfig> Conditions { get; set; } = new List<ConditionConfig>();

        public void Validate()
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
            foreach (var conditionConfig in Conditions)
            {
                conditionConfig.Validate();
            }
        }

        public override string ToString() => $"{DeviceName}.{Command}({CommandDataToString})";

        private string CommandDataToString => CommandData != null ? string.Join(',', CommandData.Select(pair => $"{pair.Key}: {pair.Value}")) : null;
    }
}
