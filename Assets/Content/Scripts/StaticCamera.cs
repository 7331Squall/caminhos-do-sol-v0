using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaticCamera : MonoBehaviour {
    [SerializeField]
    CameraLocations cams;
    [SerializeField]
    int max, cur;
    [SerializeField]
    CameraData camData;

    [SerializeField]
    InputActionReference deltaAction;
    [SerializeField]
    InputActionReference clickAction;

    // [SerializeField]
    // ConstellationNames GeoNames;
    // Dropdown ConstellationsDropdown;
    Transform _desiredTransform;
    Button _prevButton, _nextButton;
    TMP_Text _camNameLbl;

    [SerializeField]
    CameraState camState;
    [SerializeField, Range(0, 1)]
    float lerpSpeed = 0.25f;

    enum CameraState {
        Idle,
        Automatic,
        Manual,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        GameObject panel = FindObjectsByType<GameObject>(FindObjectsSortMode.None).First(x => x.name == "CameraPanel");
        _prevButton = panel.GetComponentsInChildren<Button>().First(x => x.name == "PrevButton");
        _prevButton.onClick.AddListener(PrevCam);
        _nextButton = panel.GetComponentsInChildren<Button>().First(x => x.name == "NextButton");
        _nextButton.onClick.AddListener(NextCam);
        _camNameLbl = panel.GetComponentsInChildren<TMP_Text>().First(x => x.name == "CamNameLbl");
        cams ??= GetComponent<CameraLocations>();
        // ConstellationsDropdown ??= GetComponent<Dropdown>();
        // GeoNames ??= FindFirstObjectByType<ConstellationNames>();
        max = cams.GetMaxCamPositions() - 1;
        ChangeCam(0);
        transform.SetPositionAndRotation(_desiredTransform.position, _desiredTransform.rotation);
        // InitDropdown();
    }

//     void InitDropdown() {
//         List<ConstellationNames.NamePosData> namePosList = GeoNames.namePosList;
//         ConstellationNames.LanguageType langType = GeoNames.languageType;
//         if (namePosList != null && ConstellationsDropdown != null) {
//             int currentVal = 0;
//             if (!ConstellationsDropdown.options.Count.Equals(0)) { currentVal = ConstellationsDropdown.value; }
//             ConstellationsDropdown.ClearOptions();
//             List<Dropdown.OptionData> options = new() { new Dropdown.OptionData("-") };
//             foreach (ConstellationNames.NamePosData pos in namePosList) {
//                 string constellationName = pos.secondNameArr[(int) langType];
//                 options.Add(new Dropdown.OptionData(constellationName));
//             }
//             Debug.Log(JsonUtility.ToJson(options));
//             ConstellationsDropdown.AddOptions(options);
//             ConstellationsDropdown.SetValueWithoutNotify(currentVal);
// //m_targetDir = m_namesScr.namePosList[currentVal].pos; 
// //m_targetDir = (m_namesScr.nameObjList[currentVal].transform.localPosition - transform.position).normalized;
//         }
//     }

    void ChangeCam(int newCam = -1) {
        if (newCam == -1) return;
        cur = Math.Clamp(newCam, 0, max);
        _desiredTransform = cams.GetCamPosition(cur);
        camState = CameraState.Automatic;
        _prevButton.interactable = cur > 0;
        _nextButton.interactable = cur < max;
        _camNameLbl.text = cams.GetCamName(cur);
        // transform.SetPositionAndRotation(_desiredTransform.position, _desiredTransform.rotation);
    }

    public void NextCam() {
        ChangeCam(cur + 1);
    }

    public void PrevCam() {
        ChangeCam(cur - 1);
    }

    public void Update() {
        bool hasMoved = Utilities.ShouldMoveCamera(clickAction);
        if (camState == CameraState.Idle && hasMoved)
            camState = CameraState.Manual;
        if (camState == CameraState.Manual && !hasMoved)
            camState = CameraState.Idle;
        // Vector2 coords mv = GetMouseValues();
        switch (camState) {
            case CameraState.Automatic:
                HandleAutomaticMovement();
            break;
            case CameraState.Manual:
                HandleManualMovement();
            break;
        }
    }

    void HandleAutomaticMovement() {
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, _desiredTransform.position, lerpSpeed),
            Quaternion.Lerp(transform.rotation, _desiredTransform.rotation, lerpSpeed)
        );
        if (Quaternion.Angle(transform.rotation, _desiredTransform.rotation) < 0.01f
         && Vector3.Distance(transform.position, _desiredTransform.position) < 0.01f) {
            camState = CameraState.Manual;
        }
    }

    void HandleManualMovement() {
        transform.localRotation = Utilities.CalcCamLocalRotation(transform, deltaAction, camData);
    }
}