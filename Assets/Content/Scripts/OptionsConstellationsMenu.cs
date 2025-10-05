using System.Collections.Generic;
using System.Linq;
using Constellation;
using UnityEngine;

public class OptionsConstellationsMenu : MonoBehaviour {
    static readonly int IsWindowOn = Animator.StringToHash("IsWindowOn");

    public SquackSceneManager sSceneManager;

    public ConstellationSettings constSettings;

    public GameObject celestialSphere, equatorialGrid, geoObject;

    CustomToggle _starsToggle, _linesToggle, _namesToggle, _imagesToggle, _milkyWayToggle, _equatorialGridToggle, _showAtmosphereToggle, _atmosphereHideStarsToggle;
    Animator _anim8R;

    void Awake() {
        _anim8R = GetComponent<Animator>();
        FindHUDComponents();
        AssignEvents();
        return;

        void FindHUDComponents() {
            sSceneManager = GetComponentInParent<SquackSceneManager>();
            List<MeshRenderer> meshes = sSceneManager.GetComponentsInChildren<MeshRenderer>().ToList();
            celestialSphere ??= meshes.Find(x => x.name == "CelestialSphere").gameObject;
            equatorialGrid ??= meshes.Find(x => x.name == "EquatorialGrid").gameObject;
            geoObject ??= meshes.Find(x => x.name == "Geo").gameObject;
            
            constSettings = geoObject.GetComponent<ConstellationSettings>();
            
            _starsToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("StarsToggle"));
            _linesToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("LinesToggle"));
            _namesToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("NamesToggle"));
            _imagesToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("ImagesToggle"));
            _milkyWayToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("MilkyWayToggle"));
            _equatorialGridToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>().ToList().Find(x => x.name.Contains("GridToggle"));
            _showAtmosphereToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>()
                                                 .ToList()
                                                 .Find(x => x.name.Contains("ShowAtmosphereToggle"));
            _atmosphereHideStarsToggle = sSceneManager.HUD.GetComponentsInChildren<CustomToggle>()
                                                      .ToList()
                                                      .Find(x => x.name.Contains("AtmosphereHideStarsToggle"));
        }

        void AssignEvents() {
            _starsToggle.OnValueChanged.AddListener(ToggleStars);
            _linesToggle.OnValueChanged.AddListener(ToggleLines);
            _namesToggle.OnValueChanged.AddListener(ToggleNames);
            _imagesToggle.OnValueChanged.AddListener(ToggleImages);
            _milkyWayToggle.OnValueChanged.AddListener(ToggleMilkyWay);
            _equatorialGridToggle.OnValueChanged.AddListener(ToggleGrid);
            _showAtmosphereToggle.OnValueChanged.AddListener(ToggleShowAtmosphere);
            _atmosphereHideStarsToggle.OnValueChanged.AddListener(ToggleAtmosphereHideStars);
        }

    }

    void Start() {
        ResetChecks();
        // constellationSphere.GetComponent<Renderer>().material = _constellationMaterial;
        return;

        void ResetChecks() {
            ToggleStars(_starsToggle.Value);
            ToggleLines(_linesToggle.Value);
            ToggleNames(_namesToggle.Value);
            ToggleImages(_imagesToggle.Value);
            ToggleMilkyWay(_milkyWayToggle.Value);
            ToggleGrid(_equatorialGridToggle.Value);
            ToggleShowAtmosphere(_showAtmosphereToggle.Value);
            ToggleAtmosphereHideStars(_atmosphereHideStarsToggle.Value);
        }
    }

    void ToggleStars(bool starsToggleValue) {
        constSettings.m_stars = starsToggleValue;
    }

    void ToggleLines(bool linesToggleValue) {
        constSettings.m_lines = linesToggleValue;
    }

    void ToggleNames(bool namesToggleValue) {
        constSettings.m_names = namesToggleValue;
    }

    void ToggleImages(bool imagesToggleValue) {
        constSettings.m_images = imagesToggleValue;
    }

    void ToggleMilkyWay(bool milkyWayToggleValue) {
        constSettings.m_milkyway = milkyWayToggleValue;
    }
    
    void ToggleGrid(bool arg0) {
        equatorialGrid.SetActive(arg0);
    }

    void ToggleShowAtmosphere(bool arg0) {
        celestialSphere.SetActive(arg0);
    }
    
    void ToggleAtmosphereHideStars(bool value) {
        constSettings.m_sunAffect = value ? 1.0f : 0.0f;
    }

    public void OpenOptions(bool isOpen) {
        _anim8R.SetBool(IsWindowOn, isOpen);
    }
}