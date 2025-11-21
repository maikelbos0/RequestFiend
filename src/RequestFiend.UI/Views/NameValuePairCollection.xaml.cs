using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;
using System.Collections.ObjectModel;

namespace RequestFiend.UI.Views;

public partial class NameValuePairCollection : CollectionView {

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

        if (ItemsSource is ObservableCollection<NameValuePairModel> collection) {
            collection.Remove((NameValuePairModel)button.BindingContext);
        }
    }

    public void OnAddClicked(object sender, EventArgs e) {
        if (ItemsSource is ObservableCollection<NameValuePairModel> collection) {
            collection.Add(new());
        }
    }
}
