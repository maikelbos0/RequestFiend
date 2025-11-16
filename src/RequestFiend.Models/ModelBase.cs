using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace RequestFiend.Models;

public abstract class ModelBase : ObservableObject {
    private const double widthBreakpoint = 500;

    private double pageWidth;
    private DeviceIdiom deviceIdiom;
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

    public DeviceIdiom DeviceIdiom {
        get => deviceIdiom;
        set {
            if (SetProperty(ref deviceIdiom, value)) {
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
        if (DeviceIdiom == DeviceIdiom.Phone || DeviceIdiom == DeviceIdiom.Watch || DeviceIdiom == DeviceIdiom.Unknown || PageWidth < widthBreakpoint) {
            StackHorizontalOptions = LayoutOptions.Fill;
            StackOrientation = StackOrientation.Vertical;
        }
        else {
            StackHorizontalOptions = LayoutOptions.End;
            StackOrientation = StackOrientation.Horizontal;
        }
    }
}
