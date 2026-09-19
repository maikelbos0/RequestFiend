using RequestFiend.Core;

namespace RequestFiend.Models.Messages;

public record OpenCollectionRequestMessage(FileModel File, RequestTemplateCollection Collection);
