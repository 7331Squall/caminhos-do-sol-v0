using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Extensions
{
    public static void SetOptions(this TMP_Dropdown dropdown, List<string> options) {
        int value = dropdown.value;
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = Mathf.Clamp(value, 0, options.Count - 1);
    }
}