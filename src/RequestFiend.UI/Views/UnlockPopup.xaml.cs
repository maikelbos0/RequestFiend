using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class UnlockPopup : Popup {
    public UnlockPopup(ISecretEncryptor secretEncryptor, ISecretOwner owner) {
        BindingContext = new UnlockModel(CloseAsync, secretEncryptor, owner);
        InitializeComponent();
    }
}
