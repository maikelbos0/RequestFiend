using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.IO;

namespace RequestFiend.Models.Services;

public class LogModelSink : ILogEventSink {
    private readonly LogModel logModel;
    private readonly MessageTemplateTextFormatter textFormatter;

    public LogModelSink(LogModel logModel, string outputTemplate) {
        this.logModel = logModel;
        textFormatter = new MessageTemplateTextFormatter(outputTemplate);
    }

    public void Emit(LogEvent logEvent) {
        using var stringWriter = new StringWriter();
        textFormatter.Format(logEvent, stringWriter);
        logModel.Add(stringWriter.ToString());
    }
}
