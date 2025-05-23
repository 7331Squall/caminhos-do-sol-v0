using System;
using UnityEngine;

[Serializable]
public class OrbitalCameraProperties : MonoBehaviour
{
    public float distance = 20f; // Distância da câmera até o alvo
    public float xSpeed = 120f; // Velocidade de rotação horizontal
    public float ySpeed = 120f; // Velocidade de rotação vertical
    public float yMinLimit = 1f; // Limite inferior da rotação vertical
    public float yMaxLimit = 90f; // Limite superior da rotação vertical
    public float zoomSpeed = 2f; // Velocidade de zoom
    public float minDistance = 2f; // Zoom mínimo
    public float maxDistance = 20f; // Zoom máximo
    public float sunDistance = 10f;
}