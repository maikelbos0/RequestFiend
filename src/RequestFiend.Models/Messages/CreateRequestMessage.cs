using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record CreateRequestMessage(string FilePath, RequestTemplateCollection Collection, RequestTemplate Request);
