using System.Collections.Generic;

namespace RequestFiend.UI.Models;

public static class Options {
    public static List<string> Methods { get; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
}
