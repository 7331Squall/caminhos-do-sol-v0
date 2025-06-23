using UnityEngine;

public class CelestialFloorTextureChange : MonoBehaviour {
    [SerializeField]
    Material northPoleMaterial, southPoleMaterial, regularMaterial;
    [SerializeField]
    // ReSharper disable once InconsistentNaming
    MeshRenderer _renderer;
    [SerializeField]
    NewLatitudeField latitudeField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        latitudeField.OnValueChanged.AddListener(LatitudeChanged);
    }

    void LatitudeChanged(float newLat) {
        if (Mathf.Approximately(newLat, 90))
            _renderer.material = northPoleMaterial;
        else if (Mathf.Approximately(newLat, -90))
            _renderer.material = southPoleMaterial;
        else
            _renderer.material = regularMaterial;
    }
}
