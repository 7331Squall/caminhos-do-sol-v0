using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Constellation {
    public class SquackConstellationNames : MonoBehaviour {
        bool _shouldDrawOnlyVirgo;
        
        public float fontSizeMultiplier = 0.12f;
        
        public bool ShouldDrawOnlyVirgo {
            get => _shouldDrawOnlyVirgo;
            set {
                _shouldDrawOnlyVirgo = value;
                DoShowOnlyVirgo();
            }
        }

        public enum LanguageType {
            Latin = 0,
            English,
            Japanese,
            Hangul,
            Portuguese,
        }

        public struct NamePosData {
            public int constellationId;
            public readonly string shortName;
            public readonly string[] secondNameArr;
            public Vector3 pos;
            public float rot;

            public NamePosData(int _id, Vector3 _pos, float _rot, string _sName, string[] _2ndNameArr) {
                constellationId = _id;
                pos = _pos;
                rot = _rot;
                shortName = _sName;
                secondNameArr = _2ndNameArr;
            }
        }

        [SerializeField, HideInInspector]
        Hipparcos hipparcosScr;
        [SerializeField]
        TextAsset posNameFile;
        [SerializeField]
        GameObject textPrefab;
        [SerializeField]
        Camera targetCamera;
        [SerializeField]
        float distance = 99f;
        [SerializeField]
        LanguageType m_languageType = LanguageType.Latin;
        LanguageType m_prevLangType;
        public LanguageType languageType {
            get => m_languageType;
            set => m_languageType = value;
        }
        List<NamePosData> m_namePosList;
        List<GameObject> m_nameObjList;
        public List<NamePosData> namePosList {
            get => m_namePosList;
        }
        public List<GameObject> nameObjList {
            get => m_nameObjList;
        }

        // Use this for initialization
        void Start() {
            m_nameObjList = new List<GameObject>();
            m_namePosList = CreateNamePosList(posNameFile);
            m_prevLangType = m_languageType;
            if (hipparcosScr == null) {
                hipparcosScr = FindAnyObjectByType<Hipparcos>();
            }
            if (targetCamera == null) {
                targetCamera = Camera.main;
            }

            foreach (NamePosData data in m_namePosList) {
                Vector3 dir = data.pos;
                if (hipparcosScr != null && hipparcosScr.hipLineList != null) {
                    dir = Hipparcos.GetCenterOfGravity(hipparcosScr.hipLineList, data.shortName);
                }
                Vector3 pos = dir * distance;

                Quaternion rot = Quaternion.LookRotation(pos);
                GameObject go = Instantiate(textPrefab, pos, rot);
                go.layer = LayerMask.NameToLayer("StarsNames");
                go.transform.SetParent(transform);
                go.transform.localPosition = pos;
                go.transform.localRotation = rot * transform.root.rotation;
                go.transform.localScale = Vector3.one;
                m_nameObjList.Add(go);
                TMP_Text tmpText = go.GetComponent<TMP_Text>();
                TextMesh textMesh = go.GetComponent<TextMesh>();
                if (tmpText != null) {
                    tmpText.name = data.secondNameArr[0];
                    tmpText.text = data.secondNameArr[(int) m_languageType];
                    tmpText.fontSize = distance * fontSizeMultiplier; // 0.01 * 12 = 0.12
                }
                if (textMesh != null) {
                    textMesh.name = data.secondNameArr[0];
                    textMesh.text = data.secondNameArr[(int) m_languageType];
                    textMesh.characterSize = distance * fontSizeMultiplier * 0.1f; // 0.01 * 12 = 0.12
                }
            }
        }

        // Update is called once per frame
        void LateUpdate() {
            foreach (GameObject go in m_nameObjList) {
                go.transform.rotation = Quaternion.LookRotation(go.transform.position, targetCamera.transform.up);
            }
            if (m_prevLangType != m_languageType) {
                changeLanguage(m_languageType);
                m_prevLangType = m_languageType;
            }
        }

        void changeLanguage(LanguageType _langType) {
            m_languageType = _langType;
            if (m_nameObjList == null)
                return;

            int cnt = 0;
            foreach (GameObject go in m_nameObjList) {
                TMP_Text textMesh = go.GetComponent<TMP_Text>();
                if (textMesh != null) {
                    textMesh.text = m_namePosList[cnt].secondNameArr[(int) m_languageType];
                }
                cnt++;
            }
        }
        
        void DoShowOnlyVirgo() {
            if (m_nameObjList == null) return;
            foreach (GameObject go in m_nameObjList) {
                go.SetActive(!ShouldDrawOnlyVirgo || go.name.Contains("Virgo"));
            }
        }

        public static List<NamePosData> CreateNamePosList(TextAsset _asset) {
            // id,IH,IM,sD,code,name,name_ja

            List<NamePosData> list = new List<NamePosData>();
            StringReader sr = new StringReader(_asset.text);
            int id = 1;
            while (sr.Peek() > -1) {
                string lineStr = sr.ReadLine();
                // Ignora linhas vazias e comentários
                if (string.IsNullOrWhiteSpace(lineStr) || lineStr.TrimStart().StartsWith("//")) { continue; }
                if (lineStr != null) {
                    string[] dataArr = lineStr.Split(',');
                    //            int id = int.Parse(dataArr[0]);
                    float lH = float.Parse(dataArr[1]);
                    float lM = float.Parse(dataArr[2]);
                    float lD = lH * (360f / 24f) + lM * (1f / 60f);
                    float sD = float.Parse(dataArr[3]);
                    Quaternion rotL = Quaternion.AngleAxis(lD, Vector3.up);
                    Quaternion rotS = Quaternion.AngleAxis(sD, Vector3.right);
                    Vector3 pos = rotL * rotS * Vector3.forward;
                    string[] secondNameArr = new string[] { dataArr[5], dataArr[6], dataArr[7], dataArr[8] }; //, (dataArr[9] ?? dataArr[6]) };
                    list.Add(new NamePosData(id, pos, 0f, dataArr[4], secondNameArr));
                }
            }
            sr.Close();
            return list;
        }
    }
}