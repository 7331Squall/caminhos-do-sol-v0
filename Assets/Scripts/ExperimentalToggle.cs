using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExperimentalToggle : MonoBehaviour {
    public Toggle experimentalToggle;
    public Camera cam;
    public GameObject sky;
    [SerializeField]
    public bool emitSunTrails;
    public UnityEvent<bool> OnValueChanged { get; set; } = new();
    bool _interactable = true;

    public bool Interactable {
        get => _interactable;
        set {
            _interactable = value;
            experimentalToggle.interactable = value;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        UpdateExperimentalConfig(experimentalToggle.isOn);
        experimentalToggle.onValueChanged.AddListener(UpdateExperimentalConfig);
    }

    void UpdateExperimentalConfig(bool value) {
        OnValueChanged.Invoke(value);
        emitSunTrails = value;
        if (cam != null)
            cam.clearFlags = value ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        if (sky != null)
            sky.SetActive(value);
    }
}