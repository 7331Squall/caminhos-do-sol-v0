using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class DateField : MonoBehaviour
{
    [FormerlySerializedAs("hourDropdown")]
    [FormerlySerializedAs("Hour")]
    public TMP_Dropdown dayDropdown;

    [FormerlySerializedAs("minuteDropdown")]
    [FormerlySerializedAs("Minute")]
    public TMP_Dropdown monthDropdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        dayDropdown.ClearOptions();
        dayDropdown.AddOptions(HourField.PopulateList(31, 1));
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

    public System.DateTime GetValue() {
        return new System.DateTime(2000, monthDropdown.value + 1, dayDropdown.value + 1);
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

// using System;
// using UnityEngine;
// using System.Collections.Generic;
// using System.Linq;
// using TMPro;
//
// public class UIDateField : UIField<DateTime>
// {
//     public TMP_Dropdown _dayField;
//     public TMP_Dropdown _monthField;
//     private bool _callBackBlock;
//     private DateTime _actualValue;
//
//     public UIDateField()
//     {
//         PopulateDayDropdown(31);
//         _monthField.SetOptions();
//         UpdateActualValue();
//         _dayField.onValueChanged.AddListener(OnDropdownChanged);
//         _monthField.onValueChanged.AddListener(OnMonthDropdownChanged);
//     }
//
//     private void UpdateActualValue() => _actualValue = new DateTime(2000, _monthField.value + 1, _dayField.value + 1);
//
//     private void OnDropdownChanged(int newValue)
//     {
//         if (_callBackBlock) return;
//         UpdateActualValue();
//     }
//
//     public new DateTime Value
//     {
//         get => _actualValue;
//         set
//         {
//             UpdateActualValue();
//             _callBackBlock = true;
//             _dayField.value = _actualValue.Day     - 1;
//             _monthField.value = _actualValue.Month - 1;
//             _callBackBlock = false;
//         }
//     }
// }