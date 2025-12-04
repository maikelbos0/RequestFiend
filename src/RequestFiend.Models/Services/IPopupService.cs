using System.Threading.Tasks;

namespace RequestFiend.Models.Services;

public interface IPopupService {
    Task<bool> ShowConfirmPopup(string text);
}
