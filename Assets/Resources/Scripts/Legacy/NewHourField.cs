using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NewHourField : MonoBehaviour
{
    public TMP_Dropdown hourDropdown;
    public TMP_Dropdown minuteDropdown;
    DateTime _actualValue;
    bool _enabled = true;
    bool _isUpdating;
    public UnityEvent<DateTime> OnValueChanged { get; set; } = new();

    public bool Interactable {
        get => _enabled;
        set {
            _enabled = value;
            hourDropdown.interactable = value;
            minuteDropdown.interactable = value;
        }
    }

    public DateTime Value {
        get => _actualValue;
        set {
            _actualValue = value;
            hourDropdown.value = value.Hour;
            minuteDropdown.value = value.Minute;
            Debug.Log(
                $"Time: {value.Hour}:{value.Minute} - HourDD: {hourDropdown.value} - MinDD: {minuteDropdown.value} - _actualValue: {_actualValue} newValue: {value}"
            );
            OnValueChanged.Invoke(value);
        }
    }

    void Awake() {
        hourDropdown.SetOptions(Utilities.PopulateList(24));
        hourDropdown.RefreshShownValue();
        minuteDropdown.SetOptions(Utilities.PopulateList(60));
        minuteDropdown.onValueChanged.AddListener(_ => OnTimeChanged());
        minuteDropdown.onValueChanged.AddListener(_ => OnTimeChanged());
        Value = new DateTime(2000, 1, 1, 12, 0, 0);
    }

    void OnTimeChanged() {
        if (_isUpdating) return;
        _isUpdating = true;
        DateTime newValue = new(2000, 1, 1, hourDropdown.value, minuteDropdown.value, 0);
        Value = newValue;
        _isUpdating = false;
        // OnValueChanged.Invoke(newValue);
    }
}