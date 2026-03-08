using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record OpenCollectionRequestMessage(string FilePath, RequestTemplateCollection Collection);
