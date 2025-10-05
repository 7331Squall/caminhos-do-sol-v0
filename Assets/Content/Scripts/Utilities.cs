using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utilities
{
    public static List<string> PopulateList(int amount, int offset = 0) {
        List<string> options = new();
        for (int i = 0; i < amount; i++) { options.Add((i + offset).ToString("D2")); }
        return options;
    }
    
    // ✅ Verifica se qualquer TMP_Dropdown está expandido
    public static bool AnyDropdownOpen() {
        TMP_Dropdown[] dropdowns = Object.FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None); // FindObjectsOfType<TMP_Dropdown>();
        return dropdowns.Any(dd => dd.IsExpanded);
    }

    public static bool IsPointerOverUI() {
        // Mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
        // Touch
        return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }
}