using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;

namespace RequestFiend.UI;

public partial class App : Application {
    public static T GetRequiredService<T>() where T : notnull
        => ((App)Current!).serviceProvider.GetRequiredService<T>();

    private readonly IServiceProvider serviceProvider;

    public App(IServiceProvider serviceProvider) {
        InitializeComponent();
        this.serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        return new Window(new AppShell());
    }
}
