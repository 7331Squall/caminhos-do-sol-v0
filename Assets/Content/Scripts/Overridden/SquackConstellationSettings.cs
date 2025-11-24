using System.Linq;
using UnityEngine;
using static Constellation.SquackConstellationNames;
// ReSharper disable InconsistentNaming

namespace Constellation {
    public class SquackConstellationSettings : MonoBehaviour {
        static readonly int SunAffect = Shader.PropertyToID("_SunAffect");
        bool m_virgo_old;
        [SerializeField, Tooltip("Should draw only Virgo")]
        internal bool m_virgo;
        [SerializeField, Tooltip("Show starts")]
        public bool m_stars = true;
        [SerializeField, Tooltip("Show lines")]
        public bool m_lines = true;
        [SerializeField, Tooltip("Show names")]
        public bool m_names = true;
        [SerializeField, Tooltip("Show images")]
        public bool m_images = true;
        [SerializeField, Tooltip("Show milkyway")]
        public bool m_milkyway = true;
        // [SerializeField, Tooltip("Enable lookAt")]
        // bool m_lookat = true;
        [SerializeField, Tooltip("Name locale")]
        LanguageType m_languageType = LanguageType.Latin;
        [SerializeField, Range(0, 1), Tooltip("Fade out when the sun rises.")]
        float m_sunAffect = 1f;
        SquackConstellationStars m_constellationStars;
        SquackConstellationLines m_constellationLines;
        SquackConstellationNames m_constellationNames;
        SquackConstellationImages constellationImages;
        [SerializeField, HideInInspector]
        GameObject m_milkywayObj;
        // [SerializeField, HideInInspector]
        // LookAtConstellations m_lookAtConstellations;
        float m_prevousSunAffect = -1f;
        Light m_sunLight;

        // Start is called before the first frame update
        void Start() {
            m_virgo_old = m_virgo;
            if (m_constellationStars == null)
                m_constellationStars = FindAnyObjectByType<SquackConstellationStars>();

            if (m_constellationLines == null)
                m_constellationLines = FindAnyObjectByType<SquackConstellationLines>();

            if (m_constellationNames == null)
                m_constellationNames = FindAnyObjectByType<SquackConstellationNames>();

            if (constellationImages == null)
                constellationImages = FindAnyObjectByType<SquackConstellationImages>();

            if (m_milkywayObj == null)
                m_milkywayObj = GameObject.Find("Milkyway");

            // if (m_lookAtConstellations == null)
            //     m_lookAtConstellations = FindAnyObjectByType<LookAtConstellations>();

            Light[] lightArr = FindObjectsByType<Light>(FindObjectsSortMode.None);
            if (lightArr != null) {
                m_sunLight = lightArr.FirstOrDefault(l => l.type == LightType.Directional);
            }
        }

        // Update is called once per frame
        void Update() {
            if (m_virgo_old != m_virgo) {
                m_virgo_old = m_virgo;
                DoDrawVirgo();
            }
            if (m_constellationStars?.gameObject.activeSelf != m_stars)
                m_constellationStars?.gameObject.SetActive(m_stars);

            if (m_constellationLines?.gameObject.activeSelf != m_lines)
                m_constellationLines?.gameObject.SetActive(m_lines);

            if (m_constellationNames?.gameObject.activeSelf != m_names)
                m_constellationNames?.gameObject.SetActive(m_names);

            if (constellationImages?.gameObject.activeSelf != m_images)
                constellationImages?.gameObject.SetActive(m_images);

            if (m_milkywayObj?.activeSelf != m_milkyway)
                m_milkywayObj?.SetActive(m_milkyway);

            // if (m_lookAtConstellations?.gameObject.activeSelf != m_lookat)
            //     m_lookAtConstellations?.gameObject.SetActive(m_lookat);

            if (m_constellationNames?.languageType != m_languageType)
                m_constellationNames.languageType = m_languageType;

            if (!Mathf.Approximately(m_prevousSunAffect, m_sunAffect)) {
                updateSunAffect(m_sunAffect);
            }
            updateTextSunAffect();
        }
        
        void DoDrawVirgo() {
            m_constellationNames.ShouldDrawOnlyVirgo = m_virgo;
            m_constellationLines.ShouldDrawOnlyVirgo = m_virgo;
            m_constellationStars.ShouldDrawOnlyVirgo = m_virgo;
            constellationImages.ShouldDrawOnlyVirgo = m_virgo;
        }

        void updateSunAffect(float _sunAffect) {
            if (m_constellationStars != null) {
                ParticleSystem ps = m_constellationStars.gameObject.GetComponent<ParticleSystem>();
                if (ps != null) {
                    ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
                    if (psr != null) {
                        if (psr.material != null) {
                            if (psr.material.HasProperty(SunAffect)) {
                                psr.material.SetFloat(SunAffect, _sunAffect);
                            }
                        }
                    }
                }
            }
            if (m_constellationLines != null) {
                MeshRenderer mr = m_constellationLines.gameObject.GetComponent<MeshRenderer>();
                if (mr != null) {
                    if (mr.material != null) {
                        if (mr.material.HasProperty(SunAffect)) {
                            mr.material.SetFloat(SunAffect, _sunAffect);
                        }
                    }
                }
            }
            if (constellationImages != null) {
                SpriteRenderer[] srArr = constellationImages.gameObject.GetComponentsInChildren<SpriteRenderer>();
                if (srArr != null) {
                    foreach (SpriteRenderer sr in srArr) {
                        if (sr != null) {
                            if (sr.material != null) {
                                if (sr.material.HasProperty(SunAffect)) {
                                    sr.material.SetFloat(SunAffect, _sunAffect);
                                }
                            }
                        }
                    }
                }
            }

            m_prevousSunAffect = _sunAffect;
        }

        void updateTextSunAffect() {
            if (m_sunLight != null) {
                if (m_constellationNames != null) {
                    float lightAffect = Mathf.Clamp01(Vector3.Dot(m_sunLight.transform.forward, Vector3.up));
                    TextMesh[] tmArr = m_constellationNames.gameObject.GetComponentsInChildren<TextMesh>();
                    if (tmArr != null) {
                        foreach (TextMesh tm in tmArr) {
                            if (tm != null) {
                                tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, Mathf.Lerp(1f, lightAffect, m_sunAffect) * 0.5f);
                            }
                        }
                    }
                }
            }
        }
    }
}