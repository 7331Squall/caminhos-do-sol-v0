using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class Utilities {
    public static List<string> PopulateList(int amount, int offset = 0) {
        List<string> options = new();
        for (int i = 0; i < amount; i++) { options.Add((i + offset).ToString("D2")); }
        return options;
    }

    // ✅ Verifica se qualquer TMP_Dropdown está expandido
    public static bool AnyDropdownOpen() {
        TMP_Dropdown[] dropdowns = Object.FindObjectsByType<TMP_Dropdown>(FindObjectsSortMode.None); // FindObjectsOfType<TMP_Dropdown>();
        return dropdowns.Any(dd => dd.IsExpanded);
    }

    public static GameObject IsPointerOverUI() {
        if (EventSystem.current == null)
            return null;

        PointerEventData pointerData = new(EventSystem.current);

        Vector2 pointerPos = Vector2.zero;

        // Mouse
        if (Mouse.current != null)
            pointerPos = Mouse.current.position.ReadValue();

        // Touch
        else if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            pointerPos = Touchscreen.current.touches[0].position.ReadValue();

        pointerData.position = pointerPos;

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
            return results[0].gameObject;

        return null;
    }

    public static Vector2 GetCameraMovementValues(InputActionReference pointAction, Vector2 camSpeed) {
        Vector2 coords = pointAction.action.ReadValue<Vector2>();
        Vector2 mousePos = new();
        mousePos.x += coords.x * camSpeed.x * Time.deltaTime;
        mousePos.y += coords.y * camSpeed.y * Time.deltaTime;
        return mousePos;
    }

    public static bool ShouldMoveCamera(InputActionReference clickAction) =>
        clickAction.action.IsPressed() && !IsPointerOverUI() && !AnyDropdownOpen();

    public static Quaternion CalcCamLocalRotation(Transform transform, InputActionReference deltaAction, CameraData camData) {
        Vector2 mv = GetCameraMovementValues(deltaAction, new(camData.xSpeed, -camData.ySpeed));
        // Pega a rotação atual
        Vector3 euler = transform.localEulerAngles;
        // Converte X de 0–360 para -180–180
        if (euler.x > 180f) euler.x -= 360f;
        // Aplica delta vertical do input e clamp
        float newRotX = Mathf.Clamp(euler.x + mv.y, camData.yMinLimit, camData.yMaxLimit);
        // Mantém horizontal (Y) atual
        float newRotY = euler.y + mv.x;
        // Aplica a rotação final
        return Quaternion.Euler(newRotX, newRotY, 0f);
    }
}