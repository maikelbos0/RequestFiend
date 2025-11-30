using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace RequestFiend.Models;

public class BoundModelBase : ObservableObject {
    private const double widthBreakpoint = 675;

    public double PageWidth {
        get => field;
        set {
            if (SetProperty(ref field, value)) {
                EvaluateResponsiveProperties();
            }
        }
    }
    public LayoutOptions StackHorizontalOptions {
        get => field; 
        private set => SetProperty(ref field, value);
    }
    public StackOrientation StackOrientation {
        get => field; 
        private set => SetProperty(ref field, value);
    }
    public bool StackIsHorizontal {
        get => field;
        private set => SetProperty(ref field, value);
    }
    public bool StackIsVertical {
        get => field;
        private set => SetProperty(ref field, value);
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
}
