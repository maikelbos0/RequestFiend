using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record RequestTemplateCollectionUpdatedMessage(string FilePath, RequestTemplateCollection Collection);
