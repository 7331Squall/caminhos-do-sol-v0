using System;
using UnityEngine;
using UnityEngine.Serialization;
using static AstronomiaSolar;

public class SceneManager : MonoBehaviour
{
    [FormerlySerializedAs("Date")]
    public NewDateField dateField;

    [FormerlySerializedAs("Hour")]
    public HourField hourField;

    [FormerlySerializedAs("Latitude")]
    public NewLatitudeField latitudeField;

    [FormerlySerializedAs("Light Object")]
    public GameObject lightGameObject;

    [FormerlySerializedAs("Light Itself")]
    public GameObject actualLight;

    public float sphereRadius = 10f;
    private float _latitude;
    private DateTime _currentTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        dateField.Value = new(2000, 03, 26);
        dateField.OnValueChanged.AddListener(_ => UpdateTime());
        latitudeField.OnValueChanged.AddListener(_ => UpdateTime());
        UpdateTime();
    }

    void DataUpdated() {
        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        Vector3 direction = GetSunPosition(_currentTime, _latitude);
        if (lightGameObject != null) lightGameObject.transform.position = direction.normalized * sphereRadius;
        lightGameObject.transform.rotation =
            Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);
    }

    void UpdateTime() {
        DateTime date = dateField.Value;
        DateTime hour = hourField.GetValue();
        _latitude = latitudeField.Value;
        _currentTime = new DateTime(date.Year, date.Month, date.Day, hour.Hour, hour.Minute, 0);
        DataUpdated();
    }
}