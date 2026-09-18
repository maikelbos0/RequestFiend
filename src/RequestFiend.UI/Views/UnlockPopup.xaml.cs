using CommunityToolkit.Maui.Views;
using RequestFiend.Core;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class UnlockPopup : Popup<bool> {
    public UnlockPopup(ISecretEncryptor secretEncryptor, ISecretOwner owner) {
        BindingContext = new UnlockModel(CloseAsync, secretEncryptor, owner);
        InitializeComponent();
    }

    public async void OnCancelClicked(object sender, System.EventArgs e) {
        await CloseAsync();
    }
}
