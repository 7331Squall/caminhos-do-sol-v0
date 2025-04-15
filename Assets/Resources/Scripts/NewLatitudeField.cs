using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewLatitudeField : MonoBehaviour
{
    public UnityEvent<float> OnValueChanged { get; set; } = new();
    public TMP_InputField latitudeNumberField;
    public Button latitudeButton;
    TMP_Text _latitudeButtonText;
    bool _isUpdating;
    float _value = -27.5f;

    public float Value {
        get => _value;
        set {
            _value = Mathf.Clamp(value, -90, 90);
            latitudeNumberField.text = Mathf.Abs(_value).ToString("F1");
            _latitudeButtonText.text = _value < 0 ? "S" : "N";
            OnValueChanged.Invoke(_value);
        }
    }

    public void Awake() {
        _latitudeButtonText = latitudeButton.GetComponentInChildren<TMP_Text>();
        latitudeNumberField.onValueChanged.AddListener(UpdateValue);
        latitudeButton.onClick.AddListener(ButtonClicked);
    }

    void ButtonClicked() { Value *= -1; }

    void UpdateValue(string value) {
        if (_isUpdating) return;
        _isUpdating = true;
        try { Value = float.Parse(value); } catch (FormatException e) { Debug.LogError(e); }
        _isUpdating = false;
        OnValueChanged.Invoke(_value);
    }
}

// using TMPro;
// using UnityEngine;
//
// public class NewLatitudeField : UIField<string>
// {
//     private readonly TMP_InputField _latitudeInputField;
//     
//     private readonly TMP_Text _latitudeDirection;
//     private bool _callbackBlock;
//     private float _latitude;
//
//     public NewLatitudeField(TMP_InputField latitudeInputField, TMP_Text latitudeDirection) {
//         this._latitudeInputField = latitudeInputField;
//         this._latitudeDirection = latitudeDirection;
//         latitudeInputField.onValueChanged.AddListener(OnInputChanged);
//     }
//
//     private void OnInputChanged(string value) {
//         if (_callbackBlock) return;
//         if (float.TryParse(value, out var newValue)) { Latitude = newValue; }
//     }
//
//     public float Latitude {
//         get => _latitude;
//         set {
//             _latitude = value;
//             _callbackBlock = true;
//             _latitudeInputField.text = Mathf.Abs(_latitude).ToString("F1");
//             _latitudeDirection.text = _latitude > 0 ? "N" : "S";
//             _callbackBlock = false;
//         }
//     }
// }