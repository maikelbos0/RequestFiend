using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record RequestTemplateCreatedMessage(FileModel File, RequestTemplateCollection Collection, RequestTemplate Request);
