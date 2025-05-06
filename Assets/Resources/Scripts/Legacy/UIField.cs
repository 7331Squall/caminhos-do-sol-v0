using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UIField<T>
{
    public T Value { get; set; }
}

public class FloatInputField : UIField<string>
{
    readonly TMP_InputField _field;
    float _actualValue;
    bool _callbackBlock;

    public FloatInputField(TMP_InputField field) {
        _field = field;
        _actualValue = float.TryParse(field.text, out float val) ? val : 0f;
        field.onValueChanged.AddListener(OnInputChanged);
    }

    public new float Value {
        get => _actualValue;
        set {
            _actualValue = value;
            _callbackBlock = true;
            _field.text = _actualValue.ToString("F1");
            _callbackBlock = false;
        }
    }

    void OnInputChanged(string value) {
        if (_callbackBlock) return;
        if (float.TryParse(value, out float newValue)) { _actualValue = newValue; }
    }
}

public class IntDropdownField : UIField<int>
{
    int _actualValue;
    bool _callBackBlock;
    readonly TMP_Dropdown _field;

    public IntDropdownField(TMP_Dropdown field) {
        _field = field;
        _actualValue = field.value;
        field.onValueChanged.AddListener(OnDropdownChanged);
    }

    public new int Value {
        get => _actualValue;
        set {
            _actualValue = value;
            _callBackBlock = true;
            _field.value = value;
            _callBackBlock = false;
        }
    }

    void OnDropdownChanged(int newValue) {
        if (_callBackBlock) return;
        _actualValue = newValue;
    }

    public void SetOptions(List<string> options) {
        int value = _field.value;
        _field.ClearOptions();
        _field.AddOptions(options);
        _field.value = Mathf.Max(Mathf.Min(options.Count - 1, value), 0);
    }
}