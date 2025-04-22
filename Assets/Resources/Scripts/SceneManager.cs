using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SolarMath;

public class SceneManager : MonoBehaviour
{
    public NewDateTimeField datetimeField;
    public NewLatitudeField latitudeField;
    public SimSliderField simSliderField;
    public Button simButton;
    public GameObject lightGameObject;
    bool _isSimulating; //, _isUpdatingTime;
    public float sphereRadius = 10f;
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
        CurrentTime = new(2000, 01, 01, 12, 0, 0);
        Latitude = -27.5f;
        simButton.onClick.AddListener(ToggleSimulation);
        datetimeField.OnValueChanged.AddListener(_ => DataUpdated());
        latitudeField.OnValueChanged.AddListener(_ => DataUpdated());
        DataUpdated();
    }

    //
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start() {
    //     simButton.onClick.AddListener(ToggleSimulation);
    //     CurrentTime = new(2000, 03, 26, 12, 0, 0);
    //     UpdateTime();
    // }
    //
    void DataUpdated() {
        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        Vector3 direction = GetSunPosition(CurrentTime, Latitude);
        if (lightGameObject) lightGameObject.transform.position = direction.normalized * sphereRadius;
        lightGameObject.transform.rotation =
            Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);
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

    void Update() {
        if (_isSimulating) {
            if (_simulationDateTime.Year == 1999) _simulationDateTime = CurrentTime;
            int simSecondsPerSecond = SpeedSetting.SimSecondsPerSecond(simSliderField.Value);
            double simValue = simSecondsPerSecond * Time.deltaTime;
            _simulationDateTime = _simulationDateTime.AddSeconds(simValue);
            CurrentTime =
                simSliderField.Value >= SpeedPerSecond.OneWeek
                    ? new(
                        _simulationDateTime.Year, _simulationDateTime.Month, _simulationDateTime.Day, CurrentTime.Hour,
                        CurrentTime.Minute, 0
                    )
                    : _simulationDateTime;
            DataUpdated();
            Debug.Log(
                $"Lat: {Latitude}, Dat: {CurrentTime}, IDT: {_simulationDateTime}, SSP: {SpeedSetting.SimSecondsPerSecond(simSliderField.Value)}"
            );
        } else if (_simulationDateTime.Year != 1999) { _simulationDateTime = new(1999, 1, 1, 12, 0, 0); }
    }
}