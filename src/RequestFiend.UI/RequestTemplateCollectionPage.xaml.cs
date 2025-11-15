using RequestFiend.Core;
using RequestFiend.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RequestFiend.UI;

public partial class RequestTemplateCollectionPage : RequestTemplateCollectionPageBase<RequestTemplateCollectionModel> {
    public RequestTemplateCollectionPage(string filePath, RequestTemplateCollection collection) : base(filePath, collection) {
        Model = new(collection);
        InitializeComponent();
    }

    private CancellationTokenSource? successMessageCancellationTokenSource;

    private async void OnUpdateCollectionClicked(object sender, System.EventArgs e) {
        if (!Model.TryUpdateRequestTemplateCollection(collection)) {
            return;
        }

        await SaveCollection();

        successMessageCancellationTokenSource?.Cancel();
        successMessageCancellationTokenSource = new();

        await ShowSuccessMessage("Changes have been saved", successMessageCancellationTokenSource.Token);

        async Task ShowSuccessMessage(string text, CancellationToken cancellationToken) {
            SuccessLabel.Text = text;
            SuccessLabel.Opacity = 1;
            SuccessLabel.IsVisible = true;

            await Task.Delay(1000);

            for (var i = 0; i < 50; i++) {
                if (cancellationToken.IsCancellationRequested) {
                    return;
                }

                SuccessLabel.Opacity = (50 - i) / 50.0;
                await Task.Delay(20);
            }

            SuccessLabel.IsVisible = false;
        }
    }
}
