using System;

namespace RequestFiend.Core;

public static class AppHost {
    public static IServiceProvider Services { 
        get => field ?? throw new InvalidOperationException($"{nameof(Services)} must be set before usage.");
        set;
    }
}
