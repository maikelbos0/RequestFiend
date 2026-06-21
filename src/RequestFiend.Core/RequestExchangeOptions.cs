namespace RequestFiend.Core;

public record RequestExchangeOptions(bool AllowScriptEvaluation, int? RequestTimeoutInSeconds, Environment? Environment);
