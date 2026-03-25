using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace RequestFiend.Models;

public class PageBoundModelBase : BoundModelBase {
    private const double widthBreakpoint = 540;

    public double PageWidth { get => field; set => SetProperty(ref field, value); }
    public LayoutOptions StackHorizontalOptions { get => field; private set => SetProperty(ref field, value); }
    public StackOrientation StackOrientation { get => field; private set => SetProperty(ref field, value); }
    public bool StackIsHorizontal { get => field; private set => SetProperty(ref field, value); }
    public bool StackIsVertical { get => field; private set => SetProperty(ref field, value); }
    public string PageTitleBase { get => field; protected set => SetProperty(ref field, value); }
    public string PageTitle { get => field; protected set => SetProperty(ref field, value); }
    public string ShellItemTitleBase { get => field; protected set => SetProperty(ref field, value); }
    public string ShellItemTitle { get => field; protected set => SetProperty(ref field, value); }

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
