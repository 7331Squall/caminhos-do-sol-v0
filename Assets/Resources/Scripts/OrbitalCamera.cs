using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    public Transform target;        // Alvo (pode ser vazio no (0,0,0))
    public OrbitalCameraProperties props;
    float _x;
    float _y;

    void Start() {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;
        if (target) return;
        GameObject go = new("Camera Target") { transform = { position = Vector3.zero } };
        target = go.transform;
    }

    void LateUpdate() {
        bool clicking = Input.GetMouseButton(0);
        bool touching = Input.touchCount == 1;
        bool rotating = clicking || touching;
        bool overUI = IsPointerOverUI();
        bool dropdownOpen = AnyDropdownOpen();
        if (rotating && !overUI && !dropdownOpen) {
            Vector2 delta = Vector2.zero;
            if (clicking) { delta = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y")); } else {
                //if not clicking, is touching
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) {
                    delta = touch.deltaPosition * 0.1f; // ajuste de sensibilidade pro toque
                }
            }
            _x += delta.x * props.xSpeed * Time.deltaTime;
            _y += delta.y * props.ySpeed * Time.deltaTime;
            _y = Mathf.Clamp(_y, props.yMinLimit, props.yMaxLimit);
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel") * (dropdownOpen ? 0 : 1);
        if (Input.touchCount == 2) {
            // Zoom (scroll no mouse ou pinça no touchscreen)
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            float prevMag = (t1.position - t1.deltaPosition - (t2.position - t2.deltaPosition)).magnitude;
            float currMag = (t1.position - t2.position).magnitude;
            scroll = (prevMag - currMag) * 0.01f;
        }
        props.distance = Mathf.Clamp(props.distance - scroll * props.zoomSpeed, props.minDistance, props.maxDistance);
        Quaternion rotation = Quaternion.Euler(_y, _x, 0);
        Vector3 negDistance = new(0, 0, -props.distance);
        Vector3 position = rotation * negDistance + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }

    // ✅ Verifica se qualquer TMP_Dropdown está expandido
    static bool AnyDropdownOpen() {
        var dropdowns = FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None); // FindObjectsOfType<TMP_Dropdown>();
        return dropdowns.Any(dd => dd.IsExpanded);
    }

    static bool IsPointerOverUI() {
        // Mouse
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        // Touch
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return true;
        return false;
    }
}