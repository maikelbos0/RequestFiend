using System;

namespace RequestFiend.Models;

public interface IUserInterface {
    void BeginInvokeOnMainThread(Action action);
}
