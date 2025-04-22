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
    // public GameObject lightGameObject;
    // public float sphereRadius = 10f;
    // bool _isSimulating, _isUpdatingTime;
    //
    DateTime CurrentTime {
        get => datetimeField.Value;
        set => datetimeField.Value = value;
    }
    
    float Latitude {
        get => latitudeField.Value;
        set => latitudeField.Value = value;
    }

    void Awake() {
        CurrentTime = new(2000, 03, 26, 12, 0, 0);
        Latitude = -27.5f;
        simButton.onClick.AddListener(ToggleSimulation);
    }

    //
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start() {
    //     datetimeField.OnValueChanged.AddListener(_ => UpdateTime());
    //     latitudeField.OnValueChanged.AddListener(_ => UpdateTime());
    //     simButton.onClick.AddListener(ToggleSimulation);
    //     CurrentTime = new(2000, 03, 26, 12, 0, 0);
    //     UpdateTime();
    // }
    //
    // void DataUpdated() {
    //     // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
    //     Vector3 direction = GetSunPosition(CurrentTime, Latitude);
    //     if (lightGameObject) lightGameObject.transform.position = direction.normalized * sphereRadius;
    //     lightGameObject.transform.rotation =
    //         Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);
    // }
    //
    // void UpdateTime() {
    //     DateTime datetime = datetimeField.Value;
    //     Latitude = latitudeField.Value;
    //     CurrentTime = new DateTime(datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, 0);
    //     DataUpdated();
    // }
    //
    void ToggleSimulation() {
        Debug.Log(
            $"Lat: {Latitude}, Dat: {CurrentTime}, SSP: {SpeedSetting.SimSecondsPerSecond(simSliderField.Value)}"
        );

        // _isSimulating = !_isSimulating;
        // latitudeField.Interactable = !_isSimulating;
        // datetimeField.Interactable = !_isSimulating;
        // simSliderField.Interactable = !_isSimulating;
        // simButton.GetComponentInChildren<TMP_Text>().text = _isSimulating ? "Simulando..." : "Simular";
    }
    //
    // void Update() {
    //     if (_isSimulating) {
    //         int simSecondsPerSecond = SpeedSetting.SimSecondsPerSecond(simSliderField.Value);
    //         double simValue = simSecondsPerSecond * Time.deltaTime;
    //         Debug.Log($"Time: {CurrentTime}\nS/s: {simValue}");
    //         CurrentTime.AddSeconds(simValue);
    //         Debug.LogWarning($"New Time: {CurrentTime}");
    //         DataUpdated();
    //     }
    // }
}