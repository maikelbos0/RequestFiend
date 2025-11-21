using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class NameValuePairCollection : ContentView {
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(NameValuePairModelCollection), typeof(NameValuePairCollection), default(NameValuePairModelCollection));

    public NameValuePairModelCollection ItemsSource {
        get => (NameValuePairModelCollection)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty DeleteButtonTextProperty = BindableProperty.Create(nameof(DeleteButtonText), typeof(string), typeof(NameValuePairCollection), default(string));

    public string DeleteButtonText {
        get => (string)GetValue(DeleteButtonTextProperty);
        set => SetValue(DeleteButtonTextProperty, value);
    }

    public static readonly BindableProperty AddButtonTextProperty = BindableProperty.Create(nameof(AddButtonText), typeof(string), typeof(NameValuePairCollection), default(string));

    public string AddButtonText {
        get => (string)GetValue(AddButtonTextProperty);
        set => SetValue(AddButtonTextProperty, value);
    }

    public NameValuePairCollection() {
        InitializeComponent();
    }

    public void OnRemoveClicked(object sender, EventArgs e) {
        var button = (Button)sender;

        ItemsSource.Remove((NameValuePairModel)button.BindingContext);
    }

    public void OnAddClicked(object sender, EventArgs e) {
        ItemsSource.Add(new());
    }
}
