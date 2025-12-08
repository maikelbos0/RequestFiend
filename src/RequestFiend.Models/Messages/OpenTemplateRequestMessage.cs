using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record OpenTemplateRequestMessage(string FilePath, RequestTemplateCollection Collection, RequestTemplate Request);