using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Constellation {
    // If you want to use more images, you can add more images to the m_zodiacImgArr.
    // http://www.domenavi.com/SearchResults.php?jen1=168&keywrd=%E7%A7%8B%E3%81%AE%E6%98%9F%E5%BA%A7
    // http://www.domenavi.com/SearchResults.php?jen2=%E6%98%9F%E5%BA%A7%E3%82%BB%E3%83%83%E3%83%88

    public class SquackConstellationImages : MonoBehaviour {
        bool _shouldDrawOnlyVirgo;
        public bool ShouldDrawOnlyVirgo {
            get => _shouldDrawOnlyVirgo;
            set {
                _shouldDrawOnlyVirgo = value;
                DoShowOnlyVirgo();
            }
        }

        void DoShowOnlyVirgo() {
            if (m_imageObjList == null) return;
            foreach (GameObject go in m_imageObjList) {
                go.SetActive(!ShouldDrawOnlyVirgo || go.name.Contains("Virgo"));
            }
        }

        [SerializeField, HideInInspector]
        Hipparcos m_hipparcosScr;
        [SerializeField]
        GameObject m_imgPrefab;
        [SerializeField]
        Sprite[] m_zodiacImgArr;
        [SerializeField]
        ConstellationImagesSO m_imageInfoSO;
        List<GameObject> m_imageObjList;

        // Start is called before the first frame update
        void Start() {
            if (m_hipparcosScr == null)
                m_hipparcosScr = FindAnyObjectByType<Hipparcos>();

            m_imageObjList = new List<GameObject>();
            createImageList();
        }

        // Update is called once per frame
        void Update() {
            if (m_imageObjList.Count == 0 && m_zodiacImgArr != null)
                createImageList();
        }

        void createImageList() {
            if (m_hipparcosScr.hipLineList == null || m_hipparcosScr.hipLineList.Count == 0)
                return;

            if (m_imageInfoSO != null && m_imageInfoSO.infoArr != null) {
                createImageListFromSO(m_imageInfoSO);
            } else if (m_imageObjList.Count == 0 && m_zodiacImgArr != null) {
                for (int i = 0; i < m_zodiacImgArr.Length; ++i) {
                    Hipparcos.ZodiacType zod = (Hipparcos.ZodiacType) (i + 1);
                    addImageObjFromShortName(m_zodiacImgArr[i], Hipparcos.ZodiacTypeToShortName(zod));
                }
            }
        }

        void createImageListFromSO(ConstellationImagesSO _data) {
            if (m_hipparcosScr.hipLineList == null || m_hipparcosScr.hipLineList.Count == 0)
                return;

            foreach (ConstellationImagesSO.ConstellationImageInfo img in _data.infoArr) {
                if (img.Image == null) {
                    Debug.LogWarning($"ConstellationImagesSO:{img.Name}({img.ShortName}) image is null");
                    continue;
                }

                GameObject go = addImageObjFromShortName(img.Image, img.ShortName, img.Name);
                if (go == null) {
                    Debug.LogWarning(
                        $"{img.Name}: No such short name '{img.ShortName}'. Check 'constellation_posName_utf8.csv'"
                    );
                    continue;
                }

                Transform imageTr = go.transform.GetChild(0);
                if (imageTr != null) {
                    imageTr.localScale = Vector3.one * img.LocalScale;
                    imageTr.localRotation = Quaternion.Euler(0f, 0f, img.LocalRotZ);
                    imageTr.localPosition = img.LocalOffset;
                }
            }

        }

        GameObject addImageObjFromShortName(Sprite _sprite, string _shortName, string _displayName = "") {
            string displayName = string.IsNullOrEmpty(_displayName) ? _shortName : _displayName;
            List<Hipparcos.HipLine> hipLineList = Hipparcos.GetLineList(m_hipparcosScr.hipLineList, _shortName);

            if (hipLineList == null || hipLineList.Count == 0)
                return null;

            Vector3 localCenter = Hipparcos.GetCenterOfGravity(m_hipparcosScr.hipLineList, _shortName);
            GameObject zodLocalObj = Instantiate(m_imgPrefab, transform);
            zodLocalObj.name = "local_" + displayName;
            SpriteRenderer sr = zodLocalObj.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = _sprite;
            sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            zodLocalObj.transform.localPosition = localCenter * m_hipparcosScr.distance;
            zodLocalObj.transform.localRotation = Quaternion.LookRotation(localCenter);
            zodLocalObj.transform.localScale = Vector3.one * (m_hipparcosScr.distance * 0.24f);
            m_imageObjList.Add(zodLocalObj);
            return zodLocalObj;
        }

    }
}