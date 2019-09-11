using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCenter.Virtual
{
    public class Switch
    {
        static readonly List<Switch> m_Objects = new List<Switch>();

        public Switch()
        {
            m_Objects.Add(this);
        }

        static Switch()
        {
            Task.Run(() => Poll());
        }

        public ConsoleKey Key { get; set; }

        public bool Status { get; private set; }

        private static void Poll()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                foreach (var obj in m_Objects)
                {
                    obj.Status = obj.Key == key;
                    if (obj.Status)
                    {
                        obj.OnClick?.Invoke(obj, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler OnClick;

        public override string ToString() => $"Last status: {Status}";
    }
}
