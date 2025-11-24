using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class OrbitalCamera : MonoBehaviour {
    public Transform target; // Alvo (pode ser vazio no (0,0,0))
    [SerializeField]
    public OrbitalCameraData camData;

    [SerializeField]
    InputActionReference clickAction;
    [SerializeField]
    InputActionReference deltaAction;
    [SerializeField]
    InputActionReference scrollAction;

    [SerializeField]
    public Camera[] cameras;

    void Awake() {
        camData ??= new OrbitalCameraData();
        cameras = GetComponentsInChildren<Camera>();
    }

    void Start() {
        InitTarget();
    }

    void InitTarget() {

        if (target) return;
        GameObject go = new("Camera Target") { transform = { position = Vector3.zero } };
        target = go.transform;
    }

    void LateUpdate() {
        Debug.Log($"Click: {clickAction.action.ReadValue<float>()} | Scroll: {scrollAction.action.ReadValue<Vector2>()}");
        // bool dropdownOpen = Utilities.AnyDropdownOpen();
        if (Utilities.ShouldMoveCamera(clickAction)) {
            transform.localRotation = Utilities.CalcCamLocalRotation(transform, deltaAction, camData);
        }
        float scroll = scrollAction.action.ReadValue<Vector2>().y * (Utilities.AnyDropdownOpen() ? 0 : 1);
        TouchControl[] touches = Touchscreen.current.touches.Where(t => t.press.isPressed).ToArray();
        if (touches.Length >= 2) {
            TouchState t1 = touches[0].ReadValue();
            TouchState t2 = touches[1].ReadValue();
            Debug.Log(JsonUtility.ToJson(t1));
            Vector2 prevT1 = t1.startPosition + (t1.position - t1.delta);
            Vector2 prevT2 = t2.startPosition + (t2.position - t2.delta);
        
            float prevMag = (prevT1 - prevT2).magnitude;
            float currMag = (t1.position - t2.position).magnitude;
        
            scroll = (prevMag - currMag) * 0.01f;
            // usar scroll aqui
        }
        camData.distance = Mathf.Clamp(camData.distance - scroll * camData.zoomSpeed, camData.minDistance, camData.maxDistance);
        Vector3 negDistance = new(0, 0, -camData.distance);
        if (camData.isOrthographic) {
            foreach (Camera cam in cameras) {
                if (cam.name != "GalacticOverlayCamera") {
                    cam.orthographicSize = camData.distance; // zoom = tamanho ortográfico
                }
            }
            // Mantém a câmera a uma distância fixa do alvo
            //transform.position = rotation * new Vector3(0, 0, -camData.minDistance) + target.position;
            transform.position = Vector3.zero;
        } else {
            transform.position = transform.localRotation * negDistance + target.position;
        }
    }
}