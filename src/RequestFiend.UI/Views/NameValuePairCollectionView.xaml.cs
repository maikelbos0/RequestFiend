using Microsoft.Maui.Controls;
using RequestFiend.Models;
using System;

namespace RequestFiend.UI.Views;

public partial class NameValuePairCollectionView : ContentView {
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(NameValuePairModelCollection), typeof(NameValuePairCollectionView), default(NameValuePairModelCollection));
    public static readonly BindableProperty DeleteButtonTextProperty = BindableProperty.Create(nameof(DeleteButtonText), typeof(string), typeof(NameValuePairCollectionView), default(string));
    public static readonly BindableProperty AddButtonTextProperty = BindableProperty.Create(nameof(AddButtonText), typeof(string), typeof(NameValuePairCollectionView), default(string));

    public NameValuePairModelCollection ItemsSource {
        get => (NameValuePairModelCollection)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string DeleteButtonText {
        get => (string)GetValue(DeleteButtonTextProperty);
        set => SetValue(DeleteButtonTextProperty, value);
    }

    public string AddButtonText {
        get => (string)GetValue(AddButtonTextProperty);
        set => SetValue(AddButtonTextProperty, value);
    }

    public NameValuePairCollectionView() {
        InitializeComponent();
    }
}
