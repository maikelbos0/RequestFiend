using RequestFiend.Core;

namespace RequestFiend.UI.Models;

public class RequestTemplateCollectionModel {
    public required RequestTemplateCollection Collection { get; init; }
    public required string FilePath { get; init; }
}
