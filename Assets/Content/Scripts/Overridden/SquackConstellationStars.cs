using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Constellation {
    public class SquackConstellationStars : MonoBehaviour {

        bool _shouldDrawOnlyVirgo;
        public bool ShouldDrawOnlyVirgo {
            get => _shouldDrawOnlyVirgo;
            set {
                _shouldDrawOnlyVirgo = value;
                m_hipList = null;
            }
        }

        static readonly HashSet<int> VirgoStars = new() { 57380, 60030, 61941, 63090, 63608, 65474, 66249, 68520, 69427, 69701, 71957, 72220 };

        [SerializeField, HideInInspector]
        Hipparcos hipparcosScr;
        [SerializeField]
        Texture2D m_starTex;
        [SerializeField, Range(0.1f, 10f)]
        float m_scale = 1f;
        //[SerializeField] Material pointCloudMaterial;
        readonly bool useParallax = false;
        Texture2D m_prevousTex;
        float m_prevousScale;
        ParticleSystem m_ps;
        List<Hipparcos.HipData> m_hipList;

        // Use this for initialization
        void Start() {
            if (hipparcosScr == null)
                hipparcosScr = FindAnyObjectByType<Hipparcos>();

            if (hipparcosScr.hipAppendList != null) {
                m_hipList = (hipparcosScr.hipAppendList.Count > 0) ? hipparcosScr.hipAppendList : hipparcosScr.hipList;
                //if (hideZodiac){ m_hipList = Hipparcos.GetHipListWithoutZodiac(m_hipList, hipparcosScr.hipLineList); }
                //SetMeshStars(m_hipList, transform, pointCloudMaterial, hipparcosScr.distance, useParallax);
                m_ps = SetEfcStars(m_hipList, transform, m_scale, hipparcosScr.distance, m_starTex, useParallax, ShouldDrawOnlyVirgo);
            }
            m_prevousTex = m_starTex;
            m_prevousScale = m_scale;
        }

        // Update is called once per frame
        void Update() {
            if (m_hipList == null && hipparcosScr.hipAppendList != null) {
                m_hipList = (hipparcosScr.hipAppendList.Count > 0) ? hipparcosScr.hipAppendList : hipparcosScr.hipList;
                if (ShouldDrawOnlyVirgo) {
                    m_hipList = m_hipList.Where(x => VirgoStars.Contains(x.hipId)).ToList();
                }
                m_ps = SetEfcStars(m_hipList, transform, m_scale, hipparcosScr.distance, m_starTex, useParallax, ShouldDrawOnlyVirgo);
            }

            if (m_ps != null && !Mathf.Approximately(m_prevousScale, m_scale)) {
                UpdateEfcScale(m_hipList, m_ps, m_scale);
                m_prevousScale = m_scale;
            }
            if (m_ps != null && m_prevousTex != m_starTex) {
                ParticleSystemRenderer psr = m_ps.GetComponent<ParticleSystemRenderer>();
                psr.material.mainTexture = m_starTex;
                m_prevousTex = m_starTex;
            }
        }

        private void OnDisable() {
            if (m_ps != null) {
                m_ps?.Clear();
                m_ps = null;
            }
        }

        private void OnEnable() {
            if (m_ps == null && m_hipList != null) {
                m_ps = SetEfcStars(m_hipList, transform, m_scale, hipparcosScr.distance, m_starTex, useParallax, ShouldDrawOnlyVirgo);
            }
        }

        public static void SetMeshStars(List<Hipparcos.HipData> _hipList, Transform _meshTr, Material _meshMat, float _distance, bool _useParallax) {
            if (_meshTr != null) {
                List<Vector3> vtcs = new List<Vector3>();
                List<Color> cols = new List<Color>();
                for (int i = 0; i < _hipList.Count; ++i) {
                    float paradist = 1f;
                    if ((_useParallax) && (_hipList[i].parallax != 0f)) {
                        paradist = Mathf.Clamp(1000f / Mathf.Abs(_hipList[i].parallax), 5, 5000f) * 0.005f;
                    }
                    Vector3 pos = _hipList[i].direction * _distance * paradist;
                    vtcs.Add(pos);
                    Color col = _hipList[i].color; // 色補正
                    col.r *= 4f;
                    col.g *= 2f;
                    col.b *= 3f;
                    cols.Add(col);
                    if ((vtcs.Count > 65533) || (i == _hipList.Count - 1)) {
                        GameObject meshObj = new GameObject("starMesh", new[] { typeof(MeshFilter), typeof(MeshRenderer) });
                        meshObj.transform.SetParent(_meshTr);
                        meshObj.transform.localPosition = Vector3.zero;
                        meshObj.transform.localRotation = Quaternion.identity;
                        meshObj.transform.localScale = Vector3.one;
                        meshObj.GetComponent<MeshRenderer>().material = _meshMat;
                        MeshFilter mf = meshObj.GetComponent<MeshFilter>();
                        Mesh mesh = ConsTmMesh.CreatePointCloud(vtcs.ToArray(), cols.ToArray());
                        mf.mesh = mesh;
                        vtcs.Clear();
                        cols.Clear();
                    }
                }
            }
        }

        static ParticleSystem SetEfcStars(
            List<Hipparcos.HipData> _hipList, Transform _paricleTr, float _scale, float _distance, Texture2D _efcTex, bool _useParallax,
            bool shouldDrawOnlyVirgo
        ) {
            if (shouldDrawOnlyVirgo) {
                _hipList = _hipList.Where(h => VirgoStars.Contains(h.hipId)).ToList();
            }
            ParticleSystem _ps = _paricleTr.GetComponent<ParticleSystem>();
            if (_ps != null) {
                ParticleSystem.MainModule pmm = _ps.main;
                _ps.Clear();
                pmm.maxParticles = Mathf.Min(_hipList.Count, 65535);
                pmm.playOnAwake = false;
                pmm.startSize = _scale;
                pmm.simulationSpace = ParticleSystemSimulationSpace.Local;
                pmm.scalingMode = ParticleSystemScalingMode.Hierarchy;
                _ps.Emit(_hipList.Count);
                ParticleSystem.Particle[] stars = new ParticleSystem.Particle[_hipList.Count];
                _ps.GetParticles(stars);
                for (int i = 0; i < _hipList.Count; ++i) {
                    Color col = _hipList[i].color * new Color(4f, 2f, 3f, 1f); // 色補正
                    float paradist = 1f;
                    if (_useParallax) {
                        paradist = Mathf.Clamp(1000f / _hipList[i].parallax, 50, 500f) * 0.005f;
                    }
                    stars[i].position = _hipList[i].direction * (_distance * paradist);
                    stars[i].startColor = col;
                    stars[i].startSize *= Mathf.Clamp(10f / _hipList[i].magnitude, 0.5f, 10f);
                }
                if (_efcTex != null) {
                    ParticleSystemRenderer psr = _ps.GetComponent<ParticleSystemRenderer>();
                    psr.material.mainTexture = _efcTex;
                }
                _ps.Play();
                _ps.SetParticles(stars, _hipList.Count);
                _ps.Pause();
            }
            return _ps;
        }

        static void UpdateEfcScale(List<Hipparcos.HipData> _hipList, ParticleSystem _ps, float _scale) {
            ParticleSystem.MainModule pmm = _ps.main;
            pmm.startSize = _scale;
            ParticleSystem.Particle[] stars = new ParticleSystem.Particle[_hipList.Count];
            _ps.GetParticles(stars);
            for (int i = 0; i < stars.Length; ++i) {
                stars[i].startSize = _scale * Mathf.Clamp(10f / _hipList[i].magnitude, 0.5f, 10f);
            }
            _ps.SetParticles(stars, _hipList.Count);
        }
    }
}