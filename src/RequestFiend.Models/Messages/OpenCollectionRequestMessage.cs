using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record OpenCollectionRequestMessage(string FilePath, RequestTemplateCollection Collection);

public record ExecuteRequestMessage(string FilePath, RequestTemplateCollection Collection, RequestTemplate Request);
