using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record CreateExchangeMessage(FileModel File, string Id, RequestTemplateCollection Collection, RequestTemplateSnapshot Request);
