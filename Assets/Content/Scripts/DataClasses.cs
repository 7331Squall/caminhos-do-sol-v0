using System;
using UnityEngine;

public class ModelData {
    /// <summary>
    /// Human readable name to show
    /// </summary>
    public string ModelName;
    /// <summary>
    /// Path to load the model from
    /// </summary>
    public string ModelPath;
    /// <summary>
    /// How many degrees should the model be rotated on spawn
    /// </summary>
    public float RotationDegrees;
    /// <summary>
    /// Data to update Camera
    /// </summary>
    public OrbitalCameraData CamData;
}


[Serializable]
public class OrbitalCameraData {
    /// <summary>
    /// if false, camera is in perspective mode.
    /// this does nothing, but is needed for zoom.
    /// </summary>
    [SerializeField]
    public bool isOrthographic;
    /// <summary>
    /// Camera distance to target
    /// </summary>
    [SerializeField]
    public float distance = 20f;
    /// <summary>
    /// Horizontal Rotation Speed
    /// </summary>
    [SerializeField]
    public float xSpeed = 120f;
    /// <summary>
    /// Vertical Rotation Speed
    /// </summary>
    [SerializeField]
    public float ySpeed = 120f;
    /// <summary>
    /// Lower Vertical Rotation Limit
    /// </summary>
    [SerializeField]
    public float yMinLimit = 1f;
    /// <summary>
    /// Upper Vertical Rotation Limit
    /// </summary>
    [SerializeField]
    public float yMaxLimit = 90f;
    /// <summary>
    /// Camera Zoom Speed
    /// </summary>
    [SerializeField]
    public float zoomSpeed = 5f; // Velocidade de zoom
    /// <summary>
    /// Minimum Zoom Distance
    /// </summary>
    [SerializeField]
    public float minDistance = 2f; // Zoom mínimo
    /// <summary>
    /// Maximum Zoom Distance
    /// </summary>
    [SerializeField]
    public float maxDistance = 30f; // Zoom máximo
    /// <summary>
    /// Sun Distance
    /// </summary>
    [SerializeField]
    public float sunDistance = 10f;
}

[Serializable]
public class StaticCameraData {
    /// <summary>
    /// Horizontal Rotation Speed
    /// </summary>
    [SerializeField]
    public float xSpeed = 120f;
    /// <summary>
    /// Vertical Rotation Speed
    /// </summary>
    [SerializeField]
    public float ySpeed = 120f;
    /// <summary>
    /// Lower Vertical Rotation Limit
    /// </summary>
    [SerializeField]
    public float yMinLimit = -90f;
    /// <summary>
    /// Upper Vertical Rotation Limit
    /// </summary>
    [SerializeField]
    public float yMaxLimit = 90f;
    /// <summary>
    /// Sphere Radius
    /// </summary>
    [SerializeField]
    public float sphereRadius = 10f;
}

[Serializable]
public class CameraPreset
{
    // Se você quiser, pode arrastar um Transform no Inspector e usá-lo diretamente.
    public Transform anchor;     // opcional: se setado, usa position/rotation dele
    public Vector3 targetPos;    // usado se anchor == null
    public Vector3 targetEuler;  // usado se anchor == null (Euler)
    public float moveSpeed = 3f; // unidades por segundo (maior = mais rápido)
    public float rotSpeed = 8f;  // fator de slerp por segundo (maior = mais rápido)
}
