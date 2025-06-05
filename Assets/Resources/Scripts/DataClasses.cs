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
    public float zoomSpeed = 2f; // Velocidade de zoom
    /// <summary>
    /// Minimum Zoom Distance
    /// </summary>
    [SerializeField]
    public float minDistance = 2f; // Zoom mínimo
    /// <summary>
    /// Maximum Zoom Distance
    /// </summary>
    [SerializeField]
    public float maxDistance = 20f; // Zoom máximo
    /// <summary>
    /// Sun Distance
    /// </summary>
    [SerializeField]
    public float sunDistance = 10f;
}