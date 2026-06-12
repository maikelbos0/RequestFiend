using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.IO;

namespace RequestFiend.Models.Services;

public class RequestLogSink : ILogEventSink {
    private readonly RequestLogModel requestLogModel;
    private readonly MessageTemplateTextFormatter textFormatter;

    public RequestLogSink(RequestLogModel requestLogModel, string outputTemplate) {
        this.requestLogModel = requestLogModel;
        textFormatter = new MessageTemplateTextFormatter(outputTemplate);
    }

    public void Emit(LogEvent logEvent) {
        using var stringWriter = new StringWriter();
        textFormatter.Format(logEvent, stringWriter);
        requestLogModel.Add(stringWriter.ToString());
    }
}
