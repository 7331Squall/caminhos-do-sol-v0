using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UIField<T>
{
    public T Value { get; set; }
}

public class FloatInputField : UIField<string>
{
    private readonly TMP_InputField _field;
    private bool _callbackBlock;
    private float _actualValue;

    public FloatInputField(TMP_InputField field) {
        this._field = field;
        _actualValue = float.TryParse(field.text, out var val) ? val : 0f;
        field.onValueChanged.AddListener(OnInputChanged);
    }

    private void OnInputChanged(string value) {
        if (_callbackBlock) return;
        if (float.TryParse(value, out var newValue)) { _actualValue = newValue; }
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
}

public class IntDropdownField : UIField<int>
{
    private TMP_Dropdown _field;
    private bool _callBackBlock;
    private int _actualValue;

    public IntDropdownField(TMP_Dropdown field) {
        this._field = field;
        _actualValue = field.value;
        field.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int newValue) {
        if (_callBackBlock) return;
        _actualValue = newValue;
    }

    public void SetOptions(List<string> options) {
        int value = _field.value;
        _field.ClearOptions();
        _field.AddOptions(options);
        _field.value = Mathf.Max(Mathf.Min(options.Count - 1, value), 0);
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
}