namespace RequestFiend.Core;

public record ExchangeOptions(bool AllowScriptEvaluation, int? RequestTimeoutInSeconds, Environment? Environment);
