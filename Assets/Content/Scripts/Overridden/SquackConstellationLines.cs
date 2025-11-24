using System.Collections.Generic;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace Constellation {
    public class SquackConstellationLines : MonoBehaviour {
        bool _shouldDrawOnlyVirgo;
        public bool ShouldDrawOnlyVirgo {
            get => _shouldDrawOnlyVirgo;
            set {
                _shouldDrawOnlyVirgo = value;
                hipLineList = null;
            }
        }

        [System.Serializable]
        public class GLine {
            public Vector3 p0;
            public Vector3 p1;
            public Color color;

            public GLine(Vector3 _p0, Vector3 _p1, Color _color) {
                p0 = _p0;
                p1 = _p1;
                color = _color;
            }
        }

        [SerializeField, HideInInspector]
        Hipparcos hipparcosScr;
        private List<Hipparcos.HipLine> hipLineList;

        void Start() {
            if (hipparcosScr == null)
                hipparcosScr = FindAnyObjectByType<Hipparcos>();

            hipLineList = hipparcosScr.hipLineList;
            //if (hideZodiac){ hipLineList = Hipparcos.GetLineListWithoutZodiac(hipparcosScr.hipLineList); }
            setMeshLines(hipLineList);
        }

        private void Update() {
            if (hipLineList == null) {
                hipLineList = hipparcosScr.hipLineList;
                setMeshLines(hipLineList);
            }
        }

        void setMeshLines(List<Hipparcos.HipLine> _hipLineList) {
            if (_hipLineList == null) return;

            var posList = new List<Vector3>(_hipLineList.Count * 2);
            foreach (var line in _hipLineList) {
                if (!ShouldDrawOnlyVirgo || line.constellationNameShort == "Vir") {
                    posList.Add(line.sttData.direction * hipparcosScr.distance);
                    posList.Add(line.endData.direction * hipparcosScr.distance);
                }
            }

            Mesh mesh = ConsTmMesh.CreateLine(posList.ToArray(), ConsTmMesh.LineMeshType.Lines, Color.gray);
            var mf = transform.GetComponent<MeshFilter>();
            var mr = transform.GetComponent<MeshRenderer>();
            mf.mesh = mesh;
            mr.material.color = new Color(0.5f, 0.5f, 1f, 1f);
        }

    }
}