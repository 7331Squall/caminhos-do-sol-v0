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
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SquackSceneManager : MonoBehaviour {

#region HUD
    [SerializeField]
    GameObject messagePanel;
    [SerializeField]
    GameObject loadingPanel;
    [SerializeField]
    TMP_Text loadingText;

    // ReSharper disable once InconsistentNaming
    public Canvas HUD;
    NewDateTimeField _datetimeField;
    NewLatitudeField _latitudeField;
    SimSliderField _simSpeedField, _simIntervalField;
    TMP_Text _messageText;
    Button _simButton, _optButton;
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
    bool _isSimulating, _particleChanged, _initialized;
    bool _doSunTrail;
    public bool DoSunTrail {
        get => _doSunTrail;
        set {
            _doSunTrail = value;
            TryAndResetParticle();
        }
    }
    int _loadingCount;
    DateTime _simulationDateTime, _simStartTime;
#endregion

#region Others
    public GameObject lightsGameObject, constellationGameObject;
    public float sphereRadius = 10f;
    OrbitalCamera _camera;
    ParticleSystem _lightParticle;
    List<Task<GameObject>> _promises;
    public MeshFilter FallbackModel { get; private set; }
#endregion

    void Awake() {
        _datetimeField = HUD.GetComponentInChildren<NewDateTimeField>();
        _latitudeField = HUD.GetComponentInChildren<NewLatitudeField>();
        _simButton = HUD.GetComponentsInChildren<Button>().ToList().Find(x => x.name.Contains("SimButton"));
        _optButton = HUD.GetComponentsInChildren<Button>().ToList().Find(x => x.name.Contains("OptButton"));
        _simSpeedField = HUD.GetComponentsInChildren<SimSliderField>().ToList().Find(x => x.name.Contains("SimSpeedField"));
        _simIntervalField = HUD.GetComponentsInChildren<SimSliderField>().ToList().Find(x => x.name.Contains("SimIntervalField"));
        _lightParticle = lightsGameObject.GetComponent<ParticleSystem>();
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
        _simSpeedField.OnValueChanged.AddListener(_ => TryAndResetParticle());
        _simIntervalField.OnValueChanged.AddListener(_ => TryAndResetParticle());
        _latitudeField.OnValueChanged.AddListener(_ => DataUpdated());
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
                if (TimeBetween(_simStartTime, CurrentTime, _simulationDateTime) && _doSunTrail) {
                    // if (particleChanged) {
                    _lightParticle.Pause(true);
                    _simulationDateTime = _simulationDateTime.AddDays(IntervalInDays(_simIntervalField.Value));
                    PlayParticle();
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
        TryAndResetParticle();
        (Vector3 position, Quaternion rotation) calc = GPTSolarCalc.GetPositionNOAA(Latitude, CurrentTime, false);
        lightsGameObject.transform.position = calc.position * sphereRadius;
        lightsGameObject.transform.rotation = calc.rotation;
        //      calc = GPTSolarCalc.GetPositionNOAA(Latitude, CurrentTime, true);
        // float tilt = 0f;// 23.44f; // inclinação do eixo da Terra
        // // Polo norte celeste no espaço (inclinado)
        // Vector3 northCelestial = Quaternion.Euler(tilt, 0f, 0f) * Vector3.up;
        // // Ajusta pelo ponto de vista da latitude (inclinando o “horizonte” local)
        // Vector3 northAxis = Quaternion.Euler(Latitude, 0f, 0f) * northCelestial;
        // Vector3 forward = (calc.position - northAxis).normalized;
        // // agora calcula right/up seguros usando cross:
        // Vector3 right = Vector3.Cross(northAxis, forward).normalized;
        // Vector3 up = Vector3.Cross(forward, right).normalized;
        // constellationGameObject.transform.rotation = Quaternion.LookRotation(forward, up);

        // constellationGameObject.transform.rotation = calc.rotation;
        constellationGameObject.transform.rotation = GPTSolarCalc.OrientationForCelestialPole(Latitude, CurrentTime);
    }

    void TryAndResetParticle() {
        if (!_isSimulating)
            _lightParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void UpdateProps(OrbitalCameraData props) {
        _camera.camData = props ?? new OrbitalCameraData();
        sphereRadius = _camera.camData.sunDistance;
        lightsGameObject.transform.localScale = Vector3.one * sphereRadius / 10f;
        DataUpdated();
    }

    void ToggleSimulation() {
        if (!_isSimulating) {
        }
        AdjustHudForSim();
        MainModule main = _lightParticle.main;
        _simStartTime = CurrentTime;
        if (_simIntervalField.Value == (int) Continuous) {
            main.startLifetime = SpeedInSeconds(OneDay) / SpeedInSeconds(_simSpeedField.Value);
        } else {
            main.startLifetime = float.MaxValue;
            // SimSecondsPerSecond(ThreeMonths) * 4 / SimSecondsPerSecond(_simSliderField.Value);
        }
        if (_isSimulating && _doSunTrail)
            PlayParticle();
    }

    void PlayParticle() {
        if (!_doSunTrail) return;
        _lightParticle.Play(true);
    }

    void AdjustHudForSim() {
        _isSimulating = !_isSimulating;
        _latitudeField.Interactable = !_isSimulating;
        _datetimeField.Interactable = !_isSimulating;
        _simSpeedField.Interactable = !_isSimulating;
        _simIntervalField.Interactable = !_isSimulating;
        _optButton.interactable = !_isSimulating;
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