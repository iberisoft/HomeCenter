using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using System;

namespace HomeExplorer.Services
{
    static class TextSinkExtension
    {
        public static void AddTextSink(this IServiceCollection services, string outputTemplate = TextSink.DefaultOutputTemplate, IFormatProvider formatProvider = null)
        {
            services.AddSingleton(new TextSink(outputTemplate, formatProvider));
        }

        public static void UseTextSink(this IApplicationBuilder builder)
        {
            var sink = builder.ApplicationServices.GetService<TextSink>();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink((ILogEventSink)Log.Logger)
                .WriteTo.Sink(sink)
                .CreateLogger();
        }
    }
}
