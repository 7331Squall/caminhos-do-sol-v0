using TMPro;
using UnityEngine;

public class NewLatitudeField : UIField<string>
{
    private readonly TMP_InputField _latitudeInputField;
    private readonly TMP_Text _latitudeDirection;
    private bool _callbackBlock;
    private float _latitude;

    public NewLatitudeField(TMP_InputField latitudeInputField, TMP_Text latitudeDirection) {
        this._latitudeInputField = latitudeInputField;
        this._latitudeDirection = latitudeDirection;
        latitudeInputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void OnInputChanged(string value) {
        if (_callbackBlock) return;
        if (float.TryParse(value, out var newValue)) { Latitude = newValue; }
    }

    public float Latitude {
        get => _latitude;
        set {
            _latitude = value;
            _callbackBlock = true;
            _latitudeInputField.text = Mathf.Abs(_latitude).ToString("F1");
            _latitudeDirection.text = _latitude > 0 ? "N" : "S";
            _callbackBlock = false;
        }
    }
}