using System.Collections.Generic;

public static class Utilities
{
    public static List<string> PopulateList(int amount, int offset = 0) {
        List<string> options = new List<string>();
        for (var i = 0; i < amount; i++) { options.Add((i + offset).ToString("D2")); }
        return options;
    }
}