using Microsoft.Maui.Controls;
using RequestFiend.Core;
using RequestFiend.Models;

namespace RequestFiend.UI.Views;

public partial class NameValuePairCollectionView : ContentView {
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(NameValuePairModelCollection), typeof(NameValuePairCollectionView), default(NameValuePairModelCollection));
    public static readonly BindableProperty NameLabelTextProperty = BindableProperty.Create(nameof(NameLabelText), typeof(string), typeof(NameValuePairCollectionView), default(string));
    public static readonly BindableProperty DeleteButtonTextProperty = BindableProperty.Create(nameof(DeleteButtonText), typeof(string), typeof(NameValuePairCollectionView), default(string));
    public static readonly BindableProperty AddButtonTextProperty = BindableProperty.Create(nameof(AddButtonText), typeof(string), typeof(NameValuePairCollectionView), default(string));
    public static readonly BindableProperty CollectionProperty = BindableProperty.Create(nameof(Collection), typeof(RequestTemplateCollection), typeof(NameValuePairCollectionView), default(RequestTemplateCollection));

    public NameValuePairModelCollection ItemsSource {
        get => (NameValuePairModelCollection)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public string NameLabelText {
        get => (string)GetValue(NameLabelTextProperty);
        set => SetValue(NameLabelTextProperty, value);
    }

    public string DeleteButtonText {
        get => (string)GetValue(DeleteButtonTextProperty);
        set => SetValue(DeleteButtonTextProperty, value);
    }

    public string AddButtonText {
        get => (string)GetValue(AddButtonTextProperty);
        set => SetValue(AddButtonTextProperty, value);
    }

    public RequestTemplateCollection? Collection {
        get => GetValue(CollectionProperty) as RequestTemplateCollection;
        set => SetValue(CollectionProperty, value);
    }

    public NameValuePairCollectionView() {
        InitializeComponent();
    }
}
