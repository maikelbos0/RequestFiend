using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace RequestFiend.Models;

public partial class PageBoundModelBase : BoundModelBase {
    private const double widthBreakpoint = 540;

    [ObservableProperty] public partial double PageWidth { get; set; }
    [ObservableProperty] public partial LayoutOptions StackHorizontalOptions { get; private set; }
    [ObservableProperty] public partial StackOrientation StackOrientation { get; private set; }
    [ObservableProperty] public partial bool StackIsHorizontal { get; private set; }
    [ObservableProperty] public partial bool StackIsVertical { get; private set; }
    [ObservableProperty] public partial string PageTitleBase { get; protected set; }
    [ObservableProperty] public partial string PageTitle { get; protected set; }
    [ObservableProperty] public partial string ShellItemTitleBase { get; protected set; }
    [ObservableProperty] public partial string ShellItemTitle { get; protected set; }

    public PageBoundModelBase(string initialPageTitleBase, string initialShellItemTitleBase) {
        PageTitleBase = PageTitle = initialPageTitleBase;
        ShellItemTitleBase = ShellItemTitle = initialShellItemTitleBase;

        PropertyChanged += OnPropertyChanged;
    }

    private void EvaluateResponsiveProperties() {
        if (PageWidth < widthBreakpoint) {
            StackHorizontalOptions = LayoutOptions.Fill;
            StackOrientation = StackOrientation.Vertical;
            StackIsHorizontal = false;
            StackIsVertical = true;
        }
        else {
            StackHorizontalOptions = LayoutOptions.End;
            StackOrientation = StackOrientation.Horizontal;
            StackIsHorizontal = true;
            StackIsVertical = false;
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(PageTitleBase) || e.PropertyName == nameof(ShellItemTitleBase)) {
            UpdateTitles();
        }
        else if (e.PropertyName == nameof(PageWidth)) {
            EvaluateResponsiveProperties();
        }
    }

    protected override void UpdateState() {
        var isModified = IsModified;

        base.UpdateState();

        if (isModified != IsModified) {
            UpdateTitles();
        }
    }

    protected void UpdateTitles() {
        var suffix = IsModified ? " ●" : "";

        PageTitle = $"{PageTitleBase}{suffix}";
        ShellItemTitle = $"{ShellItemTitleBase}{suffix}";
    }
}
