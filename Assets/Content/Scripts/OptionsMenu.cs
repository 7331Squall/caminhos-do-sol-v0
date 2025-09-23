using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OptionsMenu : MonoBehaviour {
    static readonly int IsWindowOn = Animator.StringToHash("IsWindowOn");

    static readonly int ShowBackground = Shader.PropertyToID("_ShowBackground");
    static readonly int ShowConstellations = Shader.PropertyToID("_ShowConstellations");
    static readonly int ShowConstBounds = Shader.PropertyToID("_ShowConstBounds");
    static readonly int ShowEquatorialGrid = Shader.PropertyToID("_ShowEquatorialGrid");

    public SquackSceneManager sSceneManager;
    public GameObject celestialSphere, constellationSphere;
    public Shader constellationShader;
    Material _constellationMaterial;
    CustomToggle _backgroundToggle, _celestialSphereToggle, _constellationToggle, _constBoundsToggle, _equatorialGridToggle;
    Animator _anim8R;

    void Awake() {
        _anim8R = GetComponent<Animator>();
        FindHUDComponents();
        AssignEvents();
        _constellationMaterial = new Material(constellationShader);
        return;

        void FindHUDComponents() {
            sSceneManager = GetComponentInParent<SquackSceneManager>();
            List<MeshRenderer> meshes = sSceneManager.GetComponentsInChildren<MeshRenderer>().ToList();
            celestialSphere = meshes.Find(x => x.name == "CelestialSphere").gameObject;
            constellationSphere = meshes.Find(x => x.name == "ConstellationSphere").gameObject;
            _backgroundToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("BackgroundField"));
            _celestialSphereToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>()
                                                  .ToList()
                                                  .Find(x => x.name.Contains("CelestialSphereField"));
            _constellationToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>()
                                                .ToList()
                                                .Find(x => x.name.Contains("ConstellationField"));
            _constBoundsToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>()
                                              .ToList()
                                              .Find(x => x.name.Contains("ConstellationRegionField"));
            _equatorialGridToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("EquatorialField"));
        }

        void AssignEvents() {
            _celestialSphereToggle.OnValueChanged.AddListener(ToggleCelestialSphere);
            _backgroundToggle.OnValueChanged.AddListener(ToggleBackground);
            _constellationToggle.OnValueChanged.AddListener(ToggleConstellations);
            _constBoundsToggle.OnValueChanged.AddListener(ToggleConstellationBounds);
            _equatorialGridToggle.OnValueChanged.AddListener(ToggleEquatorialGrid);
        }

    }

    void Start() {
        ResetChecks();
        constellationSphere.GetComponent<Renderer>().material = _constellationMaterial;
        return;

        void ResetChecks() {
            ToggleCelestialSphere(_celestialSphereToggle.Value);
            ToggleConstellations(_constellationToggle.Value);
            ToggleConstellationBounds(_constBoundsToggle.Value);
            ToggleEquatorialGrid(_equatorialGridToggle.Value);
            ToggleBackground(_backgroundToggle.Value);
        }
    }


    void ToggleEquatorialGrid(bool arg0) {
        SetMaterialBoolParameter(ShowEquatorialGrid, arg0);
    }

    void ToggleConstellationBounds(bool arg0) {
        SetMaterialBoolParameter(ShowConstBounds, arg0);
    }

    void ToggleConstellations(bool arg0) {
        SetMaterialBoolParameter(ShowConstellations, arg0);
    }

    void ToggleBackground(bool arg0) {
        SetMaterialBoolParameter(ShowBackground, arg0);
    }

    void SetMaterialBoolParameter(int parameterName, bool arg0) {
        _constellationMaterial.SetInt(parameterName, arg0 ? 1 : 0);
    }

    void ToggleCelestialSphere(bool arg0) {
        celestialSphere.SetActive(arg0);
    }

    public void OpenOptions(bool isOpen) {
        _anim8R.SetBool(IsWindowOn, isOpen);
    }
}