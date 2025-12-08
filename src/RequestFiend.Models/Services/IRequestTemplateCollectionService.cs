using RequestFiend.Core;
using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IRequestTemplateCollectionService {
    Task Save(string filePath, RequestTemplateCollection collection);
}
