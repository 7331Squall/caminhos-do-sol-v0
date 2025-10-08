using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Constellation {
    public class SquackLookAtConstellations : MonoBehaviour {
// ReSharper disable InconsistentNaming 
        [SerializeField, HideInInspector]
        ConstellationNames m_namesScr;
        [SerializeField]
        GameObject m_geoPivot;
        [SerializeField]
        Dropdown m_selDd;
        [SerializeField]
        Transform m_cameraTarget;
        ConstellationNames.LanguageType m_prevLangType;
        private Vector3 m_targetDir = new Vector3(0, 0, 1);
// ReSharper restore InconsistentNaming

        [SerializeField]
        public CameraData camData;
        [SerializeField]
        public float camSens = 0.1f;
        bool _hasManualMoved;

// Use this for initialization
        void Start() {
            _hasManualMoved = true;
            if (m_namesScr == null) m_namesScr = FindAnyObjectByType<ConstellationNames>();
            if (m_cameraTarget == null) { m_cameraTarget = Camera.main?.transform.root; }
            m_selDd.onValueChanged.AddListener(OnConstellationChanged);
            m_prevLangType = m_namesScr.languageType;
            CreateOptions(m_namesScr.namePosList, m_namesScr.languageType);
        }

// Update is called once per frame 
        void Update() {
            if (m_selDd && m_selDd.options.Count == 0) { CreateOptions(m_namesScr.namePosList, m_namesScr.languageType); } else {
                if (m_prevLangType != m_namesScr.languageType) {
                    CreateOptions(m_namesScr.namePosList, m_namesScr.languageType);
                    m_prevLangType = m_namesScr.languageType;
                }
                Vector3 dir = m_geoPivot.transform.rotation * m_targetDir;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up); //m_cameraTarget.up); 
// Debug.Log( 
// $"hMM: {_hasManualMoved}, rot: {m_cameraTarget.rotation} dRot: {targetRot}, angle: {Quaternion.Angle(m_cameraTarget.rotation, targetRot)}, condition: {!_hasManualMoved && Quaternion.Angle(m_cameraTarget.rotation, targetRot) > 0.1f}" 
// ); 
                if (!_hasManualMoved && Quaternion.Angle(m_cameraTarget.rotation, targetRot) > 0.1f) {
                    m_cameraTarget.rotation = Quaternion.Lerp(m_cameraTarget.rotation, targetRot, 0.1f);
                } else {
                    bool clicking = Input.GetMouseButton(0);
                    bool touching = Input.touchCount == 1;
                    bool rotating = clicking || touching;
                    bool overUI = Utilities.IsPointerOverUI();
                    bool dropdownOpen = Utilities.AnyDropdownOpen();
                    if (rotating && !overUI && !dropdownOpen) {
                        Vector2 delta = Vector2.zero;
                        if (clicking) { delta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")); } else {
//if not clicking, is touching 
                            Touch touch = Input.GetTouch(0);
                            if (touch.phase == TouchPhase.Moved) {
                                delta = touch.deltaPosition * 0.1f;
// ajuste de sensibilidade pro toque
                            }
                        }
                        if (delta.magnitude > camSens) {
                            m_selDd.value = 0;
                            _hasManualMoved = true;
                            Vector3 angles = m_cameraTarget.eulerAngles;
                            angles.x += delta.x * camData.xSpeed * Time.deltaTime;
                            angles.y += Mathf.Clamp(delta.y * camData.ySpeed * Time.deltaTime, camData.yMinLimit, camData.yMaxLimit);
                            m_cameraTarget.eulerAngles += angles;
                            Debug.Log($"Should move by {angles}");
                        }
// Vector3 newTarget = m_cameraTarget.position; 
// 
// newTarget.x += delta.x * camData.xSpeed * Time.deltaTime; 
// newTarget.y += delta.y * camData.ySpeed * Time.deltaTime; 
// newTarget.y = Mathf.Clamp(newTarget.y, camData.yMinLimit, camData.yMaxLimit); 
// m_cameraTarget.position = newTarget.normalized * camData.sphereRadius; 
// 
//m_cameraTarget.rotation = Quaternion.Lerp(m_cameraTarget.rotation, targetRot, 0.1f); 
// } 
                    }
                }
            }
        }

        private void OnDisable() {
            if (m_selDd) m_selDd.gameObject.SetActive(false);
        }

        private void OnEnable() {
            if (m_selDd) m_selDd.gameObject.SetActive(true);
        }

        void CreateOptions(List<ConstellationNames.NamePosData> namePosList, ConstellationNames.LanguageType langType) {
            if (namePosList != null && m_selDd != null) {
                int currentVal = 0;
                if (!m_selDd.options.Count.Equals(0)) { currentVal = m_selDd.value; }
                m_selDd.ClearOptions();
                List<Dropdown.OptionData> options = new() { new Dropdown.OptionData("-") };
                foreach (ConstellationNames.NamePosData pos in namePosList) {
                    string constellationName = pos.secondNameArr[(int) langType];
                    options.Add(new Dropdown.OptionData(constellationName));
                }
                m_selDd.AddOptions(options);
                m_selDd.SetValueWithoutNotify(currentVal);
//m_targetDir = m_namesScr.namePosList[currentVal].pos; 
//m_targetDir = (m_namesScr.nameObjList[currentVal].transform.localPosition - transform.position).normalized;
            }
        }

        public void OnConstellationChanged(int value) {
            if (value == 0) return;
            _hasManualMoved = false;
//m_targetDir = m_namesScr.namePosList[dd.value].pos; m_targetDir = (m_namesScr.nameObjList[value - 1].transform.localPosition - transform.position).normalized; 
// string name = m_namesScr.namePosList[value - 1].secondNameArr[(int) m_namesScr.languageType]; 
//Debug.Log(name);
        }
    }
}