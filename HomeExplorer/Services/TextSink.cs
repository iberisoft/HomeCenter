using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.IO;

namespace HomeExplorer.Services
{
    class TextSink : ILogEventSink
    {
        readonly ITextFormatter m_TextFormatter;
        readonly StringWriter m_Output = new StringWriter();

        public const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:l}{NewLine}{Exception}";

        public TextSink(string outputTemplate = DefaultOutputTemplate, IFormatProvider formatProvider = null)
        {
            m_TextFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
        }

        public void Emit(LogEvent logEvent)
        {
            lock (this)
            {
                m_TextFormatter.Format(logEvent, m_Output);
                m_Output.Flush();
            }
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        public string Text => m_Output.ToString();

        public event EventHandler TextChanged;
    }
}
