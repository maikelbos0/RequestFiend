using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class NewRequestTemplateCollectionPage : ContentPage {
    public NewRequestTemplateCollectionPage() {
        BindingContext = new NewRequestTemplateCollection();
        InitializeComponent();        
    }

    private void OnCreateClicked(object sender, EventArgs e) {
        var context = BindingContext as NewRequestTemplateCollection ?? throw new InvalidOperationException();

        Shell.Current.Items.Add(new ShellContent() {
            Title = context.Name,
            Content = new RequestTemplateCollectionPage(context.Name, "TODO")
        });
    }
}

// TODO move or something
public class NewRequestTemplateCollection {
    public string Name { get; set; } = "New collection";
}
