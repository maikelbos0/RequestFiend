using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.IO;

namespace RequestFiend.Models.Services;

public class ExchangeLogSink : ILogEventSink {
    private readonly ExchangeLogModel exchangeLogModel;
    private readonly MessageTemplateTextFormatter textFormatter;

    public ExchangeLogSink(ExchangeLogModel exchangeLogModel, string outputTemplate) {
        this.exchangeLogModel = exchangeLogModel;
        textFormatter = new MessageTemplateTextFormatter(outputTemplate);
    }

    public void Emit(LogEvent logEvent) {
        using var stringWriter = new StringWriter();
        textFormatter.Format(logEvent, stringWriter);
        exchangeLogModel.Add(stringWriter.ToString());
    }
}
