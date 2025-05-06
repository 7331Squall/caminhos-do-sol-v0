using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public NewDateTimeField datetimeField;
    public NewLatitudeField latitudeField;
    public SimSliderField simSliderField;

    // public EntropediaSun sunLight;
    public Button simButton;
    public GameObject lightGameObject;
    public float sphereRadius = 10f;
    bool _isSimulating; //, _isUpdatingTime;
    DateTime _simulationDateTime;

    DateTime CurrentTime {
        get => datetimeField.Value;
        set => datetimeField.Value = value;
    }

    float Latitude {
        get => latitudeField.Value;
        set => latitudeField.Value = value;
    }

    void Start() {
        // CurrentTime = new(2000, 01, 01, 12, 0, 0);
        CurrentTime = new DateTime(2000, 12, 23, 10, 05, 0);
        Latitude = -23f;
        simButton.onClick.AddListener(ToggleSimulation);
        datetimeField.OnValueChanged.AddListener(_ => DataUpdated());
        latitudeField.OnValueChanged.AddListener(_ => DataUpdated());
        DataUpdated();
    }

    void Update() {
        if (_isSimulating) {
            if (_simulationDateTime.Year == 1999) _simulationDateTime = CurrentTime;
            int simSecondsPerSecond = SpeedSetting.SimSecondsPerSecond(simSliderField.Value);
            double simValue = simSecondsPerSecond * Time.deltaTime;
            _simulationDateTime = _simulationDateTime.AddSeconds(simValue);
            CurrentTime =
                simSliderField.Value >= SpeedPerSecond.OneWeek
                    ? new DateTime(
                        _simulationDateTime.Year, _simulationDateTime.Month, _simulationDateTime.Day, CurrentTime.Hour,
                        CurrentTime.Minute, 0
                    )
                    : _simulationDateTime;
            DataUpdated();
            Debug.Log(
                $"Lat: {Latitude}, Dat: {CurrentTime}, IDT: {_simulationDateTime}, SSP: {SpeedSetting.SimSecondsPerSecond(simSliderField.Value)}"
            );
        } else if (_simulationDateTime.Year != 1999) { _simulationDateTime = new DateTime(1999, 1, 1, 12, 0, 0); }
    }

    void DataUpdated() {
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SOL 3D AQUI
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // sunLight.SetDate(datetimeField.Value);
        // sunLight.SetLocation(0.0f, latitudeField.Value);
        // double ApparentLongitude = SolarMath.ApparentLongitude(SolarMath.Century(datetimeField.Value));
        SunPosition.CalculateSunPosition(
            datetimeField.Value, latitudeField.Value, 0, out double azimuth, out double altitude
        );

        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        Vector3 sunDirection =
            new(
                (float)(Math.Sin(azimuth) * Math.Cos(altitude)), (float)Math.Sin(altitude),
                (float)(Math.Cos(azimuth) * Math.Cos(altitude))
            );

        // float azRad = Mathf.Deg2Rad * (float)azimuth;
        // float altRad = Mathf.Deg2Rad * (float)altitude;
        // float x = Mathf.Cos(altRad) * Mathf.Sin(azRad); // Leste-Oeste
        // float y = Mathf.Sin(altRad);                   // Altura
        // float z = Mathf.Cos(altRad) * Mathf.Cos(azRad); // Norte-Sul
        // Vector3 sunDirection = new Vector3(x, y, z).normalized;

        // throw new NotImplementedException();
        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        // Vector3 sunDirection = SolarMath.GetSunPosition(CurrentTime, Latitude);
        if (lightGameObject) lightGameObject.transform.position = sunDirection * sphereRadius;
        lightGameObject.transform.rotation =
            Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);
        Debug.Log(
            $"Sun Position = {lightGameObject.transform.position} should be {sunDirection} * {sphereRadius} which is {sunDirection * sphereRadius}"
        );
    }

    //
    // void UpdateTime() {
    //     DateTime datetime = datetimeField.Value;
    //     Latitude = latitudeField.Value;
    //     CurrentTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
    //     DataUpdated();
    // }
    //
    void ToggleSimulation() {
        _isSimulating = !_isSimulating;
        latitudeField.Interactable = !_isSimulating;
        datetimeField.Interactable = !_isSimulating;
        simSliderField.Interactable = !_isSimulating;
        simButton.GetComponentInChildren<TMP_Text>().text = _isSimulating ? "Simulando..." : "Simular";
    }
}