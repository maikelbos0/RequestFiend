using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record CreateRequestMessage(string FilePath, string Id, RequestTemplateCollection Collection, RequestTemplate Request);
