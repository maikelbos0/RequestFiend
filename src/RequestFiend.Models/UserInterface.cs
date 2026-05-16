using Microsoft.Maui.ApplicationModel;
using System;

namespace RequestFiend.Models;

public class UserInterface : IUserInterface {
    public void BeginInvokeOnMainThread(Action action)
        => MainThread.BeginInvokeOnMainThread(action);
}
