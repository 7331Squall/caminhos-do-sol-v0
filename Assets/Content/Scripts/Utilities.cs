using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public static class Utilities
{
    public static List<string> PopulateList(int amount, int offset = 0)
    {
        List<string> options = new();
        for (int i = 0; i < amount; i++)
        {
            options.Add((i + offset).ToString("D2"));
        }

        return options;
    }

    public static HashSet<string> ConstLongNames = new()
    {
        "Aquarius",
        "Aries",
        "Capricornus",
        "Cancer",
        "Gemini",
        "Leo",
        "Libra",
        "Pisces",
        "Scorpius",
        "Sagittarius",
        "Taurus",
        "Virgo"
    };
    
    public static HashSet<string> ConstShortNames = new()
    {
        "Aqr", 
        "Ari", 
        "Cap", 
        "Cnc", 
        "Gem", 
        "Leo", 
        "Lib", 
        "Psc", 
        "Sco", 
        "Sgr", 
        "Tau", 
        "Vir", 
    };
    
    public static HashSet<int> ZodiacStars = new()
    {
        102618, 106278, 109074, 109139, 110003, 110395, 110960, 111123, 111497, 112716, 112961, 113136, 114341, 114855, 115438,	// Aqr
        8832, 8903, 9884, 13209,	// Ari
        100064, 100345, 102485, 102978, 104139, 105515, 105881, 106985, 107556,	// Cap
        40526, 40843, 42806, 42911, 43103, 44066,	// Cnc
        28734, 29655, 30343, 30883, 31681, 32246, 32362, 33018, 34088, 34693, 35350, 35550, 36046, 36850, 36962, 37740, 37826,	// Gem
        47908, 48455, 49583, 49669, 50335, 50583, 54872, 54879, 57632,	// Leo
        72622, 73714, 74785, 76333, 77853,	// Lib
        1645, 3760, 4889, 4906, 5742, 6193, 7007, 7097, 7884, 8198, 8833, 9487, 114971, 115738, 115830, 116771, 116928, 118268,	// Psc
        78265, 78401, 78820, 80763, 81266, 82396, 82514, 82671, 84143, 85927, 86228, 86670, 87073,	// Sco
        87072, 88635, 89341, 89642, 89931, 90185, 90496, 92041, 92855, 93085, 93506, 93683, 93864, 94820, 95168, 95294, 95347, 96406, 98032, 98412, 98688,	// Sgr
        15900, 17847, 18724, 20205, 20455, 20648, 20889, 20894, 21421, 21881, 25428, 26451,	// Tau
        57380, 60030, 61941, 63090, 63608, 65474, 66249, 68520, 69427, 69701, 71957, 72220 // Vir
    };
    
    public static readonly HashSet<int> VirgoStars = new() { 57380, 60030, 61941, 63090, 63608, 65474, 66249, 68520, 69427, 69701, 71957, 72220 };

    // ✅ Verifica se qualquer TMP_Dropdown está expandido
    public static bool AnyDropdownOpen()
    {
        TMP_Dropdown[] dropdowns = Object.FindObjectsByType<TMP_Dropdown>(); // FindObjectsOfType<TMP_Dropdown>();
        return dropdowns.Any(dd => dd.IsExpanded);
    }

    public static List<GameObject> GetObjectsUnderPointer()
    {
        if (EventSystem.current == null)
        {
            return new List<GameObject>();
        }

        int pointerId = PointerId.mousePointerId;
        PointerEventData pointerData = new(EventSystem.current)
        {
            pointerId = pointerId,
            position = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Select(r => r.gameObject).ToList();
    }

    public static Vector2 GetCameraMovementValues(InputActionReference pointAction, Vector2 camSpeed)
    {
        Vector2 coords = pointAction.action.ReadValue<Vector2>();
        Vector2 mousePos = new();
        mousePos.x += coords.x * camSpeed.x * Time.deltaTime;
        mousePos.y += coords.y * camSpeed.y * Time.deltaTime;
        return mousePos;
    }

    public static Quaternion CalcCamLocalRotation(Transform transform, InputActionReference deltaAction,
        CameraData camData)
    {
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