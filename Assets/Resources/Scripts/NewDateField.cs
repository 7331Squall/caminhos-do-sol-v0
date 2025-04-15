using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NewDateField : MonoBehaviour
{
    public UnityEvent<DateTime> OnValueChanged { get; set; } = new();
    public TMP_Dropdown dayDropdown;
    public TMP_Dropdown monthDropdown;
    DateTime _actualValue;
    bool _isUpdating;

    public DateTime Value {
        get => _actualValue;
        set {
            _actualValue = value;
            dayDropdown.value = value.Day - 1;
            monthDropdown.value = value.Month - 1;
            OnValueChanged.Invoke(value);
        }
    }
    
    void Awake() {
        dayDropdown.ClearOptions();
        dayDropdown.AddOptions(HourField.PopulateList(31, 1));
        dayDropdown.onValueChanged.AddListener(OnDayDropdownChanged);
        monthDropdown.ClearOptions();
        monthDropdown.AddOptions(
            new List<string> {
                "Jan",
                "Fev",
                "Mar",
                "Abr",
                "Mai",
                "Jun",
                "Jul",
                "Ago",
                "Set",
                "Out",
                "Nov",
                "Dez"
            }
        );
        monthDropdown.onValueChanged.AddListener(OnMonthDropdownChanged);
    }

    void OnDayDropdownChanged(int index) {
        if (_isUpdating) return;
        _isUpdating = true;
        DateTime newValue = new(2000, monthDropdown.value + 1, dayDropdown.value + 1);
        _actualValue = newValue;
        _isUpdating = false;
        OnValueChanged.Invoke(newValue);
    }

    void OnMonthDropdownChanged(int index) {
        int[] thirtyOneDays = { 0, 2, 4, 6, 7, 9, 11 };
        int[] thirtyDays = { 3, 5, 8, 10 };
        if (thirtyOneDays.Contains(index)) {
            PopulateDayDropdown(31);
            return;
        }
        if (thirtyDays.Contains(index)) {
            PopulateDayDropdown(30);
            return;
        }
        PopulateDayDropdown(28);
    }

    void PopulateDayDropdown(int daysAmount) => dayDropdown.SetOptions(HourField.PopulateList(daysAmount, 1));
}