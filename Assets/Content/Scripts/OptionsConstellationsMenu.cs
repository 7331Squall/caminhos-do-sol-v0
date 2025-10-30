using System.Collections.Generic;
using System.Linq;
using Constellation;
using UnityEngine;

public class OptionsConstellationsMenu : MonoBehaviour {
    static readonly int IsWindowOn = Animator.StringToHash("IsWindowOn");
    Animator _anim8R;
    [Header("SceneManager")]
    public SquackSceneManager sSceneManager;
    [Header("Settings")]
    public ConstellationSettings geoSettings;

    [Header("Objects")]
    public GameObject celestialSphere;
    public GameObject equatorialGrid, geoObject, occluderObject;

    [Header("Options")]
    public CustomSlider _starsSlider;
    public CustomToggle _starsToggle, _linesToggle, _namesToggle, _imagesToggle, _milkyWayToggle, _equatorialGridToggle, _showAtmosphereToggle,
        _sunTrailsToggle;

    void Start() {
        _anim8R = GetComponent<Animator>();
        AssignEvents();
        ResetChecks();
        return;

        void AssignEvents() {
            _starsToggle.onValueChanged.AddListener(ToggleStars);
            _linesToggle.onValueChanged.AddListener(ToggleLines);
            _namesToggle.onValueChanged.AddListener(ToggleNames);
            _imagesToggle.onValueChanged.AddListener(ToggleImages);
            _milkyWayToggle.onValueChanged.AddListener(ToggleMilkyWay);
            _equatorialGridToggle.onValueChanged.AddListener(ToggleGrid);
            _showAtmosphereToggle.onValueChanged.AddListener(ToggleShowAtmosphere);
            _sunTrailsToggle.onValueChanged.AddListener(ToggleSunTrails);
            _starsSlider.onValueChanged.AddListener(ChangeStarSlider);
        }

        void ResetChecks() {
            ToggleStars(_starsToggle.Value);
            ToggleLines(_linesToggle.Value);
            ToggleNames(_namesToggle.Value);
            ToggleImages(_imagesToggle.Value);
            ToggleMilkyWay(_milkyWayToggle.Value);
            ToggleGrid(_equatorialGridToggle.Value);
            ToggleShowAtmosphere(_showAtmosphereToggle.Value);
            ToggleSunTrails(_sunTrailsToggle.Value);
        }
    }

    void ChangeStarSlider(float val) => ChangeStarSlider((int) val);

    void ChangeStarSlider(int val) {
        bool shallDisable = val == 0;
        _starsToggle.Interactable = !shallDisable;
        ToggleStars(!shallDisable && _starsToggle.Value);
        _linesToggle.Interactable = !shallDisable;
        ToggleLines(!shallDisable && _linesToggle.Value);
        _namesToggle.Interactable = !shallDisable;
        ToggleNames(!shallDisable && _namesToggle.Value);
        _imagesToggle.Interactable = !shallDisable;
        ToggleImages(!shallDisable && _imagesToggle.Value);
        ToggleVirgo(val == 1);
    }

    void ToggleStars(bool starsToggleValue) {
        geoSettings.m_stars = starsToggleValue;
    }

    void ToggleLines(bool linesToggleValue) {
        geoSettings.m_lines = linesToggleValue;
    }

    void ToggleNames(bool namesToggleValue) {
        geoSettings.m_names = namesToggleValue;
    }

    void ToggleImages(bool imagesToggleValue) {
        geoSettings.m_images = imagesToggleValue;
    }

    void ToggleMilkyWay(bool milkyWayToggleValue) {
        geoSettings.m_milkyway = milkyWayToggleValue;
    }

    void ToggleVirgo(bool virgoValue) {
        geoSettings.m_virgo = virgoValue;
    }

    void ToggleGrid(bool arg0) {
        equatorialGrid.SetActive(arg0);
    }

    void ToggleShowAtmosphere(bool arg0) {
        celestialSphere.SetActive(arg0);
        occluderObject.SetActive(!arg0);
    }

    void ToggleSunTrails(bool arg0) {
        sSceneManager.DoSunTrail = arg0;
    }

    public void OpenOptions(bool isOpen) {
        _anim8R.SetBool(IsWindowOn, isOpen);
    }
}