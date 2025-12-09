using System.Collections.Generic;

namespace RequestFiend.Models.Services;

public interface IRecentCollectionService {
    List<RecentCollectionModel> Get();
    List<RecentCollectionModel> Push(string filePath);
    List<RecentCollectionModel> Remove(string filePath);
}
