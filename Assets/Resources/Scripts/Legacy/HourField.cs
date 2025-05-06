using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HourField : MonoBehaviour
{
    [FormerlySerializedAs("Hour")]
    public TMP_Dropdown hourDropdown;

    [FormerlySerializedAs("Minute")]
    public TMP_Dropdown minuteDropdown;

    bool _enabled = true;

    public bool Interactable {
        get => _enabled;
        set {
            _enabled = value;
            hourDropdown.interactable = value;
            minuteDropdown.interactable = value;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //hourDropdown.ClearOptions();
        hourDropdown.SetOptions(PopulateList(24));
        hourDropdown.value = 12;
        hourDropdown.RefreshShownValue();
        //minuteDropdown.ClearOptions();
        minuteDropdown.SetOptions(PopulateList(60));
    }

    public static List<string> PopulateList(int amount, int offset = 0) {
        var options = new List<string>();
        for (int i = 0; i < amount; i++) { options.Add((i + offset).ToString("D2")); }
        return options;
    }

    public DateTime GetValue() { return new DateTime(2000, 1, 1, hourDropdown.value, minuteDropdown.value, 0); }
}