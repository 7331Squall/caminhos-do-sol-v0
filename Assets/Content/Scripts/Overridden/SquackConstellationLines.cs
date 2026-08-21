using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Constellation
{
    public class SquackConstellationLines : MonoBehaviour
    {
        int _starsDrawMode;

        public int StarsDrawMode
        {
            get => _starsDrawMode;
            set
            {
                _starsDrawMode = value;
                hipLineList = null;
            }
        }

        [System.Serializable]
        public class GLine
        {
            public Vector3 p0;
            public Vector3 p1;
            public Color color;

            public GLine(Vector3 _p0, Vector3 _p1, Color _color)
            {
                p0 = _p0;
                p1 = _p1;
                color = _color;
            }
        }

        [SerializeField, HideInInspector] Hipparcos hipparcosScr;
        private List<Hipparcos.HipLine> hipLineList;

        void Start()
        {
            Debug.Log($"Start Lines - {StarsDrawMode}");

            if (hipparcosScr == null)
                hipparcosScr = FindAnyObjectByType<Hipparcos>();

            hipLineList = hipparcosScr.hipLineList;
            //if (hideZodiac){ hipLineList = Hipparcos.GetLineListWithoutZodiac(hipparcosScr.hipLineList); }
            setMeshLines(hipLineList);
        }

        private void Update()
        {
            if (hipLineList == null)
            {
                hipLineList = hipparcosScr.hipLineList;
                setMeshLines(hipLineList);
            }
        }

        void setMeshLines(List<Hipparcos.HipLine> _hipLineList)
        {
            if (_hipLineList == null) return;

            var posList = new List<Vector3>(_hipLineList.Count * 2);
            foreach (var line in _hipLineList)
            {
                bool shouldDraw = StarsDrawMode switch
                {
                    0 => false,
                    1 => line.constellationNameShort == "Vir",
                    2 => Utilities.ConstShortNames.Contains(line.constellationNameShort),
                    _ => true
                };
                if (shouldDraw)
                {
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