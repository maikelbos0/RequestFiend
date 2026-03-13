using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using RequestFiend.Models.PropertyTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RequestFiend.Models;

public class BoundModelBase : ObservableObject {
    private const double widthBreakpoint = 675;

    private IEnumerable<IValidatable> validatables = [];

    public double PageWidth {
        get => field;
        set {
            if (SetProperty(ref field, value)) {
                EvaluateResponsiveProperties();
            }
        }
    }
    public LayoutOptions StackHorizontalOptions { get => field; private set => SetProperty(ref field, value); }
    public StackOrientation StackOrientation { get => field; private set => SetProperty(ref field, value); }
    public bool StackIsHorizontal { get => field; private set => SetProperty(ref field, value); }
    public bool StackIsVertical { get => field; private set => SetProperty(ref field, value); }
    public string PageTitleBase { get => field; protected set => SetProperty(ref field, value); }
    public string PageTitle { get => field; protected set => SetProperty(ref field, value); }
    public string ShellItemTitleBase { get => field; protected set => SetProperty(ref field, value); }
    public string ShellItemTitle { get => field; protected set => SetProperty(ref field, value); }
    public bool HasError { get => field; set => SetProperty(ref field, value); }
    public bool IsModified { get => field; set => SetProperty(ref field, value); }
    public bool IsModifiedWithoutError { get => field; set => SetProperty(ref field, value); }

    public BoundModelBase(string initialPageTitleBase, string initialShellItemTitleBase) {
        PageTitleBase = PageTitle = initialPageTitleBase;
        ShellItemTitleBase = ShellItemTitle = initialShellItemTitleBase;
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

    public void ConfigureState(IEnumerable<IValidatable> validatables) {
        this.validatables = validatables;
        
        foreach (var validatableProperty in validatables) {
            validatableProperty.PropertyChanged += OnValidatablePropertyChanged;
        }

        PropertyChanged += OnPropertyChanged;

        UpdateState();
    }

    private void OnValidatablePropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ValidatableProperty.IsModified) || e.PropertyName == nameof(ValidatableProperty.HasError)) {
            UpdateState();
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(PageTitleBase) || e.PropertyName == nameof(ShellItemTitleBase)) {
            UpdateTitles();
        }
    }

    private void UpdateState() {
        var isModified = IsModified;

        HasError = validatables.Any(validatableProperty => validatableProperty.HasError);
        IsModified = validatables.Any(validatableProperty => validatableProperty.IsModified);
        IsModifiedWithoutError = IsModified && !HasError;

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
