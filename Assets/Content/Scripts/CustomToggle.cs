using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour {
    public Toggle customToggle;
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
            customToggle.interactable = value;
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        customToggle = GetComponent<Toggle>();   
        UpdateExperimentalConfig(customToggle.isOn);
        customToggle.onValueChanged.AddListener(UpdateExperimentalConfig);
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