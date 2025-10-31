using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplateCollectionPage : ContentPage {
    public NewRequestTemplateCollectionPage() {
        BindingContext = new NewRequestTemplateCollection();
        InitializeComponent();        
    }

    private async void OnCreateClicked(object sender, EventArgs e) {
        var context = BindingContext as NewRequestTemplateCollection ?? throw new InvalidOperationException();

        var newContent = new ShellContent() {
            Title = context.Name,
            Content = new RequestTemplateCollectionPage(context.Name, "TODO"),
            Route = $"RequestTemplateCollection_{Guid.NewGuid()}"
        };

        Shell.Current.Items.Add(newContent);

        await Shell.Current.GoToAsync($"//{newContent.Route}");
    }
}

// TODO move or something
public class NewRequestTemplateCollection {
    public string Name { get; set; } = "New collection";
}
