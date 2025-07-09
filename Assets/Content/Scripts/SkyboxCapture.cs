using UnityEngine;

public class SkyboxCapture : MonoBehaviour {
    static readonly int MainTex = Shader.PropertyToID("_MainTex");
    public int resolution = 512;
    public Material sphereMaterial;

    GameObject _camGo;
    Camera _cam;
    RenderTexture _cubemap;

    void Start() {
        _camGo = new GameObject("SkyCaptureCamera") { transform = { position = Vector3.zero } };
        _cam = _camGo.AddComponent<Camera>();
        _cam.enabled = false;
        _cam.cullingMask = 0; // Bit Mask, cada bit é um Layer que renderiza.

        _cubemap = new(resolution, resolution, 24) {
            dimension = UnityEngine.Rendering.TextureDimension.Cube, hideFlags = HideFlags.HideAndDontSave
        };
        sphereMaterial.SetTexture(MainTex, _cubemap);
    }

    void Update() { _cam.RenderToCubemap(_cubemap); }
}