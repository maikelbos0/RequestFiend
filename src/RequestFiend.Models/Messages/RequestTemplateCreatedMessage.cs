using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record RequestTemplateCreatedMessage(string FilePath, RequestTemplateCollection Collection, RequestTemplate Request);
