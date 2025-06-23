using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimSliderField : MonoBehaviour
{
    bool _enabled = true;
    bool _isUpdating;
    TMP_Text _label;
    Slider _slider;
    SpeedPerSecond _value;
    public UnityEvent<SpeedPerSecond> OnValueChanged { get; set; } = new();

    public bool Interactable {
        get => _enabled;
        set {
            _enabled = value;
            _slider.interactable = value;
        }
    }
    public SpeedPerSecond Value {
        get => _value;
        set {
            _value = value;
            float newValue = (float)_value;
            _slider.value = newValue;
            OnValueChanged.Invoke(value);
            _slider.onValueChanged.Invoke(newValue);
        }
    }

    void Awake() {
        _slider = GetComponentInChildren<Slider>();
        _label = GetComponentInChildren<TMP_Text>();
        _slider.maxValue = Enum.GetValues(typeof(SpeedPerSecond)).Length - 1;
        _slider.onValueChanged.AddListener(ValueChanged);
        Value = SpeedPerSecond.OneHour;
        UpdateLabel();
    }

    void ValueChanged(float value) {
        if (_isUpdating || (SpeedPerSecond)value == _value) return;
        _isUpdating = true;
        Value = (SpeedPerSecond)value;
        UpdateLabel();
        _isUpdating = false;
    }

    void UpdateLabel() {
        _label.text = SpeedSetting.ToDisplayString(Value);
        _label.faceColor = Value >= SpeedPerSecond.OneWeek ? new Color32(50, 0, 0, 255) : new Color32(10, 10, 10, 255);
    }
}