using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class HourField : MonoBehaviour
{
    [FormerlySerializedAs("Hour")]
    public TMP_Dropdown hourDropdown;

    [FormerlySerializedAs("Minute")]
    public TMP_Dropdown minuteDropdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        hourDropdown.ClearOptions();
        hourDropdown.AddOptions(PopulateList(24));
        hourDropdown.value = 12;
        hourDropdown.RefreshShownValue();
        minuteDropdown.ClearOptions();
        minuteDropdown.AddOptions(PopulateList(60));
    }

    public static List<string> PopulateList(int amount, int offset = 0) {
        List<string> options = new List<string>();
        for (var i = 0; i < amount; i++) { options.Add((i + offset).ToString("D2")); }
        return options;
    }

    public System.DateTime GetValue() {
        return new System.DateTime(2000, 1, 1, hourDropdown.value, minuteDropdown.value, 0);
    }
}