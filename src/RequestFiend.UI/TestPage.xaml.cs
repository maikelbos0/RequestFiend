using Microsoft.Maui.Controls;

namespace RequestFiend.UI;

public partial class TestPage : ContentPage {
    public TestPage() {
        InitializeComponent();
    }

    private void OnAddClicked(object sender, System.EventArgs e) {
        AppShell.Current.Items.Add(new ShellContent() {
            Title = "Collection 3",
            Content = new TestPage()
        });
    }
}
