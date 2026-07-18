using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record CreateExchangeMessage(string FilePath, string Id, RequestTemplateCollection Collection, RequestTemplateSnapshot Request);
