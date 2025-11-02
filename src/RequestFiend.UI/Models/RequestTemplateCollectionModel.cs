using RequestFiend.Core;

namespace RequestFiend.UI.Models;

public class RequestTemplateCollectionModel {
    public required RequestTemplateCollection Collection { get; set; }
    public required string FilePath { get; set; }
}
