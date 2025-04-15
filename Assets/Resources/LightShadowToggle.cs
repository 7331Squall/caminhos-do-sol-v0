using UnityEngine;
using static UnityEngine.LightShadows;

public class LightShadowToggle : MonoBehaviour
{

    private Light lightComponent;

    void Awake()
    {
        // Pega o componente Light do mesmo GameObject
        lightComponent = GetComponent<Light>();
    }

    public void UseSoftShadows (bool softShadows) {
        if (lightComponent == null) return;
        lightComponent.shadows = softShadows ? LightShadows.Soft : LightShadows.Hard;
    }
}
