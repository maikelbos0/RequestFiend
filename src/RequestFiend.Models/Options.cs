using System.Collections.Generic;

namespace RequestFiend.Models;

public static class Options {
    public static List<string> Methods { get; } = ["GET", "PUT", "POST", "DELETE", "HEAD", "OPTIONS", "TRACE", "PATCH"];
}
