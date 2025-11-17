using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace RequestFiend.Models;

public class BoundModelBase : ObservableObject {
    private const double widthBreakpoint = 675;

    private double pageWidth;
    private LayoutOptions stackHorizontalOptions = LayoutOptions.End;
    private StackOrientation stackOrientation;

    public double PageWidth {
        get => pageWidth;
        set {
            if (SetProperty(ref pageWidth, value)) {
                EvaluateResponsiveProperties();
            }
        }
    }

    public LayoutOptions StackHorizontalOptions {
        get => stackHorizontalOptions; 
        private set => SetProperty(ref stackHorizontalOptions, value);
    }

    public StackOrientation StackOrientation {
        get => stackOrientation; 
        private set => SetProperty(ref stackOrientation, value);
    }

    private void EvaluateResponsiveProperties() {
        if (PageWidth < widthBreakpoint) {
            StackHorizontalOptions = LayoutOptions.Fill;
            StackOrientation = StackOrientation.Vertical;
        }
        else {
            StackHorizontalOptions = LayoutOptions.End;
            StackOrientation = StackOrientation.Horizontal;
        }
    }
}
