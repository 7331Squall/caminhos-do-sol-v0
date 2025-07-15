using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpeedSettings;
using static SpeedSetting;
using static IntervalSettings;
using static IntervalSetting;
using static UnityEngine.ParticleSystem;

public class SquackSceneManager : MonoBehaviour {
    // ReSharper disable once InconsistentNaming

#region HUD
    public Canvas HUD;
    NewDateTimeField _datetimeField;
    NewLatitudeField _latitudeField;
    Button _simButton;
    SimSliderField _simSpeedField, _simIntervalField;
    [SerializeField]
    GameObject messagePanel;
    TMP_Text _messageText;
    [SerializeField]
    GameObject loadingPanel;
    [SerializeField]
    TMP_Text loadingText;
    CustomToggle _celestialSphereToggle;
#endregion

#region ExternalVariables
    DateTime CurrentTime {
        get => _datetimeField.Value;
        set => _datetimeField.Value = value;
    }
    float Latitude {
        get => _latitudeField.Value;
        set => _latitudeField.Value = value;
    }
#endregion

#region SimulationVariables
    bool _isSimulating;
    int _loadingCount;
    bool particleChanged;

    bool _initialized;

    DateTime _simulationDateTime;
    DateTime SimStartTime;
#endregion

#region Others
    OrbitalCamera _camera;
    public GameObject lightGameObject;
    ParticleSystem _lightParticle;
    public float sphereRadius = 10f;
    List<Task<GameObject>> _promises;
    public MeshFilter FallbackModel { get; private set; }
#endregion

    void Awake() {
        _datetimeField = HUD.GetComponentInChildren<NewDateTimeField>();
        _latitudeField = HUD.GetComponentInChildren<NewLatitudeField>();
        _simButton = HUD.GetComponentsInChildren<Button>().ToList().Find(x => x.name.Contains("SimButton"));
        _simSpeedField = HUD.GetComponentsInChildren<SimSliderField>().ToList().Find(x => x.name.Contains("SimSpeedField"));
        _simIntervalField = HUD.GetComponentsInChildren<SimSliderField>().ToList().Find(x => x.name.Contains("SimIntervalField"));
        _celestialSphereToggle = HUD.GetComponentInChildren<CustomToggle>();
        _lightParticle = lightGameObject.GetComponent<ParticleSystem>();
        _messageText = messagePanel.GetComponentInChildren<TMP_Text>();
        FallbackModel = GetComponentsInChildren<MeshFilter>().ToList().Find(x => x.name.Contains("Model"));
        _camera = GetComponentInChildren<OrbitalCamera>();
    }

    public void DisplayMessage(string message) {
        Debug.LogError(message);
        _messageText.text = message;
        StartCoroutine(ShowThenHide());
        return;

        IEnumerator ShowThenHide() {
            messagePanel.SetActive(true);       // ativa
            yield return new WaitForSeconds(5); // espera 5 segundos
            messagePanel.SetActive(false);      // desativa
        }
    }

    void Start() {
        // CurrentTime = new(2000, 01, 01, 12, 0, 0);
        CurrentTime = new DateTime(2000, 12, 23, 12, 00, 0);
        Latitude = -23f;
        _simButton.onClick.AddListener(ToggleSimulation);
        _datetimeField.OnValueChanged.AddListener(_ => DataUpdated());
        _simSpeedField.OnValueChanged.AddListener(_ => ResetParticle());
        _simIntervalField.OnValueChanged.AddListener(_ => ResetParticle());
        _latitudeField.OnValueChanged.AddListener(_ => DataUpdated());
        _celestialSphereToggle.OnValueChanged.AddListener(_ => ResetParticle());
        DataUpdated();
    }

    void Update() {
        if (_isSimulating) {
            if (_simulationDateTime.Year == 1999)
                _simulationDateTime = CurrentTime;
            int simSecondsPerSecond = SpeedInSeconds(_simSpeedField.Value);
            double simValue = simSecondsPerSecond * Time.deltaTime;
            _simulationDateTime = _simulationDateTime.AddSeconds(simValue);
            if (_simIntervalField.Value > (int) Continuous) {
                if (TimeBetween(SimStartTime, CurrentTime, _simulationDateTime)) {
                    // if (particleChanged) {
                    _lightParticle.Pause(true);
                    _simulationDateTime = _simulationDateTime.AddDays(IntervalInDays(_simIntervalField.Value));
                    _lightParticle.Play(true);
                    // particleChanged = false;
                    // }
                    // particleChanged = true;
                }
            }
            CurrentTime = _simulationDateTime;
            DataUpdated();
        } else if (_simulationDateTime.Year != 1999) {
            _simulationDateTime = new DateTime(1999, 1, 1, 12, 0, 0);
        }
    }

    static bool TimeBetween(DateTime evalTime, DateTime startTime, DateTime endTime) {
        TimeSpan eval = evalTime.TimeOfDay;
        TimeSpan start = startTime.TimeOfDay;
        TimeSpan end = endTime.TimeOfDay;
        bool differentDays = startTime.Date != endTime.Date;
        return (!differentDays && start < eval && end >= eval) || (differentDays && (start < eval || end >= eval));
        // int clamped = start <= end ? Math.Clamp(eval, start, end) : Math.Clamp(eval, end, start);
        // return clamped != start && clamped != end;
    }

    void DataUpdated() {
        ResetParticle();
        (Vector3 position, Quaternion rotation) calc = GPTSolarCalc.GetPositionNOAA(Latitude, CurrentTime);
        lightGameObject.transform.position = calc.position * sphereRadius;
        lightGameObject.transform.rotation = calc.rotation;
        // Debug.Log($"Sun Position = {lightGameObject.transform.position}, Sun Rotation = {lightGameObject.transform.rotation}.");
    }

    void ResetParticle() {
        if (!_isSimulating)
            _lightParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void UpdateProps(OrbitalCameraData props) {
        _camera.camData = props ?? new OrbitalCameraData();
        sphereRadius = _camera.camData.sunDistance;
        lightGameObject.transform.localScale = Vector3.one * sphereRadius / 10f;
        DataUpdated();
    }

    void ToggleSimulation() {
        if (!_isSimulating) {
        }
        AdjustHudForSim();
        MainModule main = _lightParticle.main;
        SimStartTime = CurrentTime;
        if (_simIntervalField.Value == (int) Continuous) {
            main.startLifetime = SpeedInSeconds(OneDay) / SpeedInSeconds(_simSpeedField.Value);
        } else {
            main.startLifetime = float.MaxValue;
            // SimSecondsPerSecond(ThreeMonths) * 4 / SimSecondsPerSecond(_simSliderField.Value);
        }
        if (_isSimulating)
            _lightParticle.Play();
    }

    void AdjustHudForSim() {
        _isSimulating = !_isSimulating;
        _latitudeField.Interactable = !_isSimulating;
        _datetimeField.Interactable = !_isSimulating;
        _simSpeedField.Interactable = !_isSimulating;
        _simIntervalField.Interactable = !_isSimulating;
        _celestialSphereToggle.Interactable = !_isSimulating;
        _simButton.GetComponentInChildren<TMP_Text>().text = _isSimulating ? "Simulando..." : "Simular";
    }

    // ReSharper disable once InconsistentNaming
    public void DoLoading(string LoadingText = null) {
        _loadingCount++;
        loadingPanel.SetActive(true);
        loadingText.text = LoadingText ?? "Por favor, aguarde...";
    }

    public void TryToFinishLoading() {
        _loadingCount--;
        if (_loadingCount <= 0)
            loadingPanel.SetActive(false);
    }
}