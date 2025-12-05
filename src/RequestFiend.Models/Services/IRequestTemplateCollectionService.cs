using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IRequestTemplateCollectionService {
    RequestTemplateCollection Collection { get; }
    string Title { get; }

    Task Save();
}
