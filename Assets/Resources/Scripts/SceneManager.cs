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
    public LatitudeField latitudeField;

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
        dateField.OnValueChanged.AddListener(UpdateTime);
        UpdateTime(new());
    }

    void DataUpdated() {
        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        Vector3 direction = GetSunPosition(_currentTime, _latitude);
        if (lightGameObject != null) lightGameObject.transform.position = direction.normalized * sphereRadius;
        lightGameObject.transform.rotation =
            Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);

        // actualLight.transform.rotation.SetLookRotation(Vector3.zero, Vector3.up);
        Debug.Log($"{lightGameObject.transform.position} - {lightGameObject.transform.rotation}");
    }

    void UpdateTime(DateTime time) {
        DateTime date = dateField.Value;
        DateTime hour = hourField.GetValue();
        _latitude = latitudeField.GetValue();
        _currentTime = new DateTime(date.Year, date.Month, date.Day, hour.Hour, hour.Minute, 0);
        Debug.Log(_latitude);
        Debug.Log(_currentTime);
        DataUpdated();
    }
    //
    // void DataUpdated()
    // {
    //     var data = DateUpdateData(_currentTime);
    //     //TODO: REMOVE FROM HERE
    //     double declinationRad = GetSunDeclinationRadius(50, data.sunDeclination);
    //     lightGameObject.transform.position =
    //         new Vector3(
    //                     x: (float)(declinationRad * Math.Cos(-DegToRad(90))),
    //                     z: (float)(declinationRad * Math.Sin(-DegToRad(90))),
    //                     y: (float)(RadToDeg(data.sunDeclination))
    //                    );
    //     lightGameObject.transform.rotation = Quaternion.Euler((float)data.sunDeclination, 0f, 0f);
    // }

    // Update is called once per frame
    // void Update()
    // {
    // }
}