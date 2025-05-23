using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {
    // ReSharper disable once InconsistentNaming
    public Canvas HUD;
    NewDateTimeField _datetimeField;
    NewLatitudeField _latitudeField;
    TMP_Dropdown _modelSelectDropdown;
    Button _simButton;
    SimSliderField _simSliderField;
    MeshFilter _fallbackModel;
    OrbitalCamera _camera;

    [SerializeField]
    AssetReference[] meshes;
    [SerializeField]
    Mesh fallbackMesh;
    GameObject _loadedModel;
    // [SerializeField] private AssetReferenceGameObject[] Objects;

    // public EntropediaSun sunLight;
    public GameObject lightGameObject;
    public float sphereRadius = 10f;
    bool _isSimulating; //, _isUpdatingTime;
    DateTime _simulationDateTime;
    [SerializeField]
    GameObject messagePanel;
    [SerializeField]
    GameObject loadingPanel;

    DateTime CurrentTime {
        get => _datetimeField.Value;
        set => _datetimeField.Value = value;
    }

    float Latitude {
        get => _latitudeField.Value;
        set => _latitudeField.Value = value;
    }

    void Awake() {
        _datetimeField = HUD.GetComponentInChildren<NewDateTimeField>();
        _latitudeField = HUD.GetComponentInChildren<NewLatitudeField>();
        _simButton = HUD.GetComponentsInChildren<Button>().ToList().Find(x => x.name.Contains("SimButton"));
        _simSliderField = HUD.GetComponentInChildren<SimSliderField>();
        _modelSelectDropdown = HUD.GetComponentsInChildren<TMP_Dropdown>().ToList().Find(x => x.name.Contains("ModelSelectField"));
        PopulateModelSelectDropdown();
        // _modelSelectDropdown.options = (meshes =>)
        _fallbackModel = GetComponentsInChildren<MeshFilter>().ToList().Find(x => x.name.Contains("Model"));
        _camera = GetComponentInChildren<OrbitalCamera>();
    }

    void PopulateModelSelectDropdown() {
        // TODO: Tirar o Hard-Code e Refatorar essa classe toda
        List<string> options = new() {
            "- Cilindro -", "Cristo Redentor", "Estátua da Liberdade", "Torre Eiffel"
          , "Taj Mahal"
        };
        // foreach (var mesh in meshes) {
        //     // Cria nome legível a partir da RuntimeKey
        //     string label = mesh.RuntimeKey.ToString();
        //     if (label.Contains("/"))
        //         label = label.Substring(label.LastIndexOf("/") + 1);
        //     label = label.Replace(".prefab", "");
        //     Debug.LogWarning($"Opção: {label}");
        //     options.Add(label);
        // }

        _modelSelectDropdown.ClearOptions();
        _modelSelectDropdown.AddOptions(options);
        _modelSelectDropdown.onValueChanged.AddListener(ChangeModel);
    }

    void ChangeModel(int index) {
        int actualIdx = index - 1;
        loadingPanel.SetActive(true);
        if (Math.Clamp(actualIdx, -1, meshes.Length) == actualIdx) {
            if (actualIdx == -1) {
                DisplayFallbackModel();
                return;
            }
            meshes[actualIdx].InstantiateAsync().Completed += MeshLoadingCompleted;
        }
    }

    void DisplayFallbackModel() {
        loadingPanel.SetActive(false);
        DestroyModel();
        _camera.props = _fallbackModel.GetComponent<OrbitalCameraProperties>();
        sphereRadius = _camera.props.sunDistance;
        lightGameObject.transform.localScale = Vector3.one * sphereRadius / 10f;
        _fallbackModel.gameObject.SetActive(true);
    }

    void DestroyModel() {
        foreach (AssetReference mesh in meshes) {
            try {
                mesh.ReleaseInstance(_loadedModel);
            } catch (Exception e) {
                Debug.LogError(JsonUtility.ToJson(e));
            }
        }
    }

    void MeshLoadingCompleted(AsyncOperationHandle<GameObject> obj) {
        Debug.LogWarning(JsonUtility.ToJson(obj));
        if (obj.Status == AsyncOperationStatus.Succeeded) {
            DestroyModel();
            _loadedModel = obj.Result;
            Debug.Log("Mesh loaded: " + JsonUtility.ToJson(obj));
            _camera.props = _loadedModel.GetComponent<OrbitalCameraProperties>();
            sphereRadius = _camera.props.sunDistance;
            lightGameObject.transform.localScale = Vector3.one * sphereRadius / 10f;
            DataUpdated();
            loadingPanel.SetActive(false);
        } else {
            DisplayMessage(obj.OperationException.Message);
            DisplayFallbackModel();
        }
    }

    public void DisplayMessage(string message) {
        TMP_Text text = messagePanel.GetComponentInChildren<TMP_Text>();
        text.text = message;
        StartCoroutine(ShowThenHide());

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
        _latitudeField.OnValueChanged.AddListener(_ => DataUpdated());
        DataUpdated();
    }

    void Update() {
        if (_isSimulating) {
            if (_simulationDateTime.Year == 1999)
                _simulationDateTime = CurrentTime;
            int simSecondsPerSecond = SpeedSetting.SimSecondsPerSecond(_simSliderField.Value);
            double simValue = simSecondsPerSecond * Time.deltaTime;
            _simulationDateTime = _simulationDateTime.AddSeconds(simValue);
            CurrentTime = _simSliderField.Value >= SpeedPerSecond.OneWeek
                ? new DateTime(
                    _simulationDateTime.Year, _simulationDateTime.Month, _simulationDateTime.Day, CurrentTime.Hour, CurrentTime.Minute, 0
                )
                : _simulationDateTime;
            DataUpdated();
            Debug.Log(
                $"Lat: {Latitude}, Dat: {CurrentTime}, IDT: {_simulationDateTime}, SSP: {SpeedSetting.SimSecondsPerSecond(_simSliderField.Value)}"
            );
        } else if (_simulationDateTime.Year != 1999) {
            _simulationDateTime = new DateTime(1999, 1, 1, 12, 0, 0);
        }
    }

    void DataUpdated() {
        (Vector3 position, Quaternion rotation) calc = GPTSolarCalc.GetPositionNOAA(Latitude, CurrentTime);
        lightGameObject.transform.position = calc.position * sphereRadius;
        lightGameObject.transform.rotation = calc.rotation;
        Debug.Log($"Sun Position = {lightGameObject.transform.position}, Sun Rotation = {lightGameObject.transform.rotation}.");
    }

    // void OldDataUpdated() {
    //     /////////////////////////////////////////////////////////////////////////////////////////////////////
    //     // SOL 3D AQUI
    //     /////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //     // sunLight.SetDate(datetimeField.Value);
    //     // sunLight.SetLocation(0.0f, latitudeField.Value);
    //     // double ApparentLongitude = SolarMath.ApparentLongitude(SolarMath.Century(datetimeField.Value));
    //     SunPosition.CalculateSunPosition(_datetimeField.Value, _latitudeField.Value, 0, out double azimuth, out double altitude);
    //
    //     // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
    //     Vector3 sunDirection = new(
    //         (float) (Math.Sin(azimuth) * Math.Cos(altitude)), (float) Math.Sin(altitude), (float) (Math.Cos(azimuth) * Math.Cos(altitude))
    //     );
    //
    //     // float azRad = Mathf.Deg2Rad * (float)azimuth;
    //     // float altRad = Mathf.Deg2Rad * (float)altitude;
    //     // float x = Mathf.Cos(altRad) * Mathf.Sin(azRad); // Leste-Oeste
    //     // float y = Mathf.Sin(altRad);                   // Altura
    //     // float z = Mathf.Cos(altRad) * Mathf.Cos(azRad); // Norte-Sul
    //     // Vector3 sunDirection = new Vector3(x, y, z).normalized;
    //
    //     // throw new NotImplementedException();
    //     // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
    //     // Vector3 sunDirection = SolarMath.GetSunPosition(CurrentTime, Latitude);
    //     if (lightGameObject)
    //         lightGameObject.transform.position = sunDirection * sphereRadius;
    //     lightGameObject.transform.rotation = Quaternion.LookRotation((Vector3.zero - lightGameObject.transform.position).normalized);
    //     Debug.Log(
    //         $"Sun Position = {lightGameObject.transform.position} should be {sunDirection} * {sphereRadius} which is {sunDirection * sphereRadius}"
    //     );
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
        _isSimulating = !_isSimulating;
        _latitudeField.Interactable = !_isSimulating;
        _datetimeField.Interactable = !_isSimulating;
        _simSliderField.Interactable = !_isSimulating;
        _simButton.GetComponentInChildren<TMP_Text>().text = _isSimulating ? "Simulando..." : "Simular";
    }
}