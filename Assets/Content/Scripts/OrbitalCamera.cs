using System.Collections.Generic;
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
    InputActionReference rightClickAction;
    [SerializeField]
    InputActionReference middleClickAction;
    [SerializeField]
    InputActionReference deltaAction;
    [SerializeField]
    InputActionReference scrollAction;

    [SerializeField]
    public Camera[] cameras;

    [SerializeField]
    public Camera GalacticOverlayCamera;

    enum States {
        Idle,
        MoveMainCam,
        OrbitMainCam,
        OrbitOverlay
    }

    States state = States.Idle;

    void Awake() {
        camData ??= new OrbitalCameraData();
        cameras = GetComponentsInChildren<Camera>();
    }

    void Start() {
        InitTarget();
        InitMouse();
        OrbitCamera();
    }

    void InitMouse() {
        // habilita todas as actions ligadas a esta câmera
        clickAction.action.Enable();
        rightClickAction.action.Enable();
        middleClickAction.action.Enable();
        deltaAction.action.Enable();
        scrollAction.action.Enable();
        clickAction.action.started += _ => DefineState(true, false);
        clickAction.action.canceled += _ => DefineState(false, false);
        rightClickAction.action.started += _ => DefineState(true, true);
        rightClickAction.action.canceled += _ => DefineState(false, true);
        middleClickAction.action.canceled += _ => ResetCameraPosition();
        scrollAction.action.performed += _ => ZoomCamera(scrollAction.action.ReadValue<Vector2>().y);
    }

    void InitTarget() {
        if (target) return;
        GameObject go = new("Camera Target") { transform = { position = Vector3.zero } };
        target = go.transform;
    }

    void LateUpdate() {
        switch (state) {
            case States.Idle: return;
            case States.MoveMainCam:
                MoveCamera();
                return;
            case States.OrbitMainCam:
                OrbitCamera();
                return;
            case States.OrbitOverlay:
                OrbitOverlay();
                return;
        }
    }

    void OrbitOverlay() {
        if (GalacticOverlayCamera == null) return;
        GalacticOverlayCamera.transform.localRotation = Utilities.CalcCamLocalRotation(GalacticOverlayCamera.transform, deltaAction, camData);
        // cam.transform.position = cam.transform.localRotation + target.position;
    }

    void OrbitCamera() {
        transform.localRotation = Utilities.CalcCamLocalRotation(transform, deltaAction, camData);
    }

    void MoveCamera()
    {
        transform.localPosition = Utilities.CalcCamLocalPosition(transform, deltaAction, camData);
    }

    void ResetCameraPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    void ZoomCamera(float zoom) {
        float scroll = zoom * (Utilities.AnyDropdownOpen() ? 0 : 1);
        TouchControl[] touches = Touchscreen.current.touches.Where(t => t.press.isPressed).ToArray();
        if (touches.Length >= 2) {
            TouchState t1 = touches[0].ReadValue();
            TouchState t2 = touches[1].ReadValue();
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
            transform.position = Vector3.zero;
        } else {
            transform.position = transform.localRotation * negDistance + target.position;
        }
    }

    void DefineState(bool mouseDown, bool isRight) {
        state = NewState(mouseDown, isRight);
        return;

        States NewState(bool mouseDown, bool isRight) {
            if (!mouseDown || Utilities.AnyDropdownOpen())
                return States.Idle;
            List<GameObject> overUI = Utilities.GetObjectsUnderPointer();
            if (overUI.Any(x => x.name == "SolarSystemDiagram"))
                return States.OrbitOverlay;
            if (overUI.Count <= 0)
                return isRight ? States.MoveMainCam : States.OrbitMainCam;
            return States.Idle;
        }
    }

}