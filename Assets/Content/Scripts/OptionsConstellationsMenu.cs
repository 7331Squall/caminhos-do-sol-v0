using Constellation;
using UnityEngine;

public class OptionsConstellationsMenu : MonoBehaviour {
    static readonly int IsWindowOn = Animator.StringToHash("IsWindowOn");
    Animator _anim8R;
    [Header("SceneManager")]
    public SquackSceneManager sSceneManager;
    [Header("Settings")]
    public SquackConstellationSettings geoSettings;
    [Tooltip("Should expose the \"Celestial Sphere\" Option in the menu?")]
    public bool shouldExposeCelestialSphereOption = true;

    [Header("Objects")]
    public GameObject celestialSphere;
    public GameObject equatorialGrid;
    public GameObject occluderObject;

    [Header("Options")]
    public CustomSlider starsSlider;
    public CustomToggle starsToggle;
    public CustomToggle linesToggle;
    public CustomToggle namesToggle;
    public CustomToggle imagesToggle;
    public CustomToggle milkyWayToggle;
    public CustomToggle equatorialGridToggle;
    public CustomToggle showCelestialSphereToggle;
    public CustomToggle showAtmosphereToggle;
    public CustomToggle sunTrailsToggle;

    void Start() {
        _anim8R = GetComponent<Animator>();
        AssignEvents();
        ResetChecks();
        showCelestialSphereToggle.gameObject.SetActive(shouldExposeCelestialSphereOption);
        return;

        void AssignEvents() {
            starsToggle.onValueChanged.AddListener(ToggleStars);
            linesToggle.onValueChanged.AddListener(ToggleLines);
            namesToggle.onValueChanged.AddListener(ToggleNames);
            imagesToggle.onValueChanged.AddListener(ToggleImages);
            milkyWayToggle.onValueChanged.AddListener(ToggleMilkyWay);
            equatorialGridToggle.onValueChanged.AddListener(ToggleGrid);
            showAtmosphereToggle.onValueChanged.AddListener(ToggleShowAtmosphere);
            showCelestialSphereToggle.onValueChanged.AddListener(ToggleShowCelestialSphere);
            sunTrailsToggle.onValueChanged.AddListener(ToggleSunTrails);
            starsSlider.onValueChanged.AddListener(ChangeStarSlider);
        }

        void ResetChecks() {
            ToggleStars(starsToggle.Value);
            ToggleLines(linesToggle.Value);
            ToggleNames(namesToggle.Value);
            ToggleImages(imagesToggle.Value);
            ToggleMilkyWay(milkyWayToggle.Value);
            ToggleGrid(equatorialGridToggle.Value);
            ToggleShowCelestialSphere(showCelestialSphereToggle.Value);
            ToggleShowAtmosphere(showAtmosphereToggle.Value);
            ToggleSunTrails(sunTrailsToggle.Value);
        }
    }


    void ChangeStarSlider(float val) => ChangeStarSlider((int) val);

    void ChangeStarSlider(int val) {
        bool shallDisable = val == 0;
        starsToggle.Interactable = !shallDisable;
        ToggleStars(!shallDisable && starsToggle.Value);
        linesToggle.Interactable = !shallDisable;
        ToggleLines(!shallDisable && linesToggle.Value);
        namesToggle.Interactable = !shallDisable;
        ToggleNames(!shallDisable && namesToggle.Value);
        imagesToggle.Interactable = !shallDisable;
        ToggleImages(!shallDisable && imagesToggle.Value);
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

    void ToggleShowCelestialSphere(bool arg0) {
        occluderObject.SetActive(!arg0);
    }

    void ToggleShowAtmosphere(bool arg0) {
        celestialSphere.SetActive(arg0);
    }

    void ToggleSunTrails(bool arg0) {
        sSceneManager.DoSunTrail = arg0;
    }

    public void OpenOptions(bool isOpen) {
        _anim8R.SetBool(IsWindowOn, isOpen);
    }
}