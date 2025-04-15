using UnityEngine;
using TMPro;

public class OrbitalCamera : MonoBehaviour
{
    public Transform target;        // Alvo (pode ser vazio no (0,0,0))
    public float distance = 20f;    // Distância da câmera até o alvo
    public float xSpeed = 120f;     // Velocidade de rotação horizontal
    public float ySpeed = 120f;     // Velocidade de rotação vertical
    public float yMinLimit = 1f;    // Limite inferior da rotação vertical
    public float yMaxLimit = 90f;   // Limite superior da rotação vertical
    public float zoomSpeed = 2f;    // Velocidade de zoom
    public float minDistance = 2f;  // Zoom mínimo
    public float maxDistance = 20f; // Zoom máximo
    private float _x = 0f;
    private float _y = 0f;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
        if (target == null) {
            GameObject go = new GameObject("Camera Target") { transform = { position = Vector3.zero } };
            target = go.transform;
        }
    }

    void LateUpdate() {
        if (AnyDropdownOpen()) return;
        if (Input.GetMouseButton(0) || Input.touchCount == 1) {
            Vector2 delta = Vector2.zero;
            if (Input.GetMouseButton(0)) {
                delta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            } else if (Input.touchCount == 1) {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) {
                    delta = touch.deltaPosition * 0.1f; // ajuste de sensibilidade pro toque
                }
            }
            _x += delta.x * xSpeed * Time.deltaTime;
            _y += delta.y * ySpeed * Time.deltaTime;
            _y = Mathf.Clamp(_y, yMinLimit, yMaxLimit);
        }

        // Zoom (scroll no mouse ou pinça no touchscreen)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.touchCount == 2) {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            float prevMag = (t1.position - t1.deltaPosition - (t2.position - t2.deltaPosition)).magnitude;
            float currMag = (t1.position - t2.position).magnitude;
            scroll = (prevMag - currMag) * 0.01f;
        }
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
        Quaternion rotation = Quaternion.Euler(_y, _x, 0);
        Vector3 negDistance = new(0, 0, -distance);
        Vector3 position = rotation * negDistance + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }

    // ✅ Verifica se qualquer TMP_Dropdown está expandido
    private bool AnyDropdownOpen() {
        TMP_Dropdown[]
            dropdowns = FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None); // FindObjectsOfType<TMP_Dropdown>();
        foreach (var dd in dropdowns) {
            if (dd.IsExpanded) return true;
        }
        return false;
    }
}