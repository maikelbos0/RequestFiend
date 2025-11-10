using RequestFiend.Core;

namespace RequestFiend.UI.Messages;

public record RequestTemplateCollectionUpdatedMessage(string FilePath, RequestTemplateCollection Collection);
