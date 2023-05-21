using System;
using System.Timers;

namespace HomeCenter.Virtual
{
    public class Timer : IDisposable
    {
        readonly System.Timers.Timer m_Timer;

        public Timer(double interval)
        {
            m_Timer = new(interval);
            m_Timer.Elapsed += Timer_Elapsed;
            m_Timer.Start();
        }

        public void Dispose()
        {
            m_Timer.Elapsed -= Timer_Elapsed;
            m_Timer.Stop();
            m_Timer.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnTick?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnTick;

        public override string ToString() => "Timer";
    }
}
