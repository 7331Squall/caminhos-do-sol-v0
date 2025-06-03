using System.IO;
using UnityEngine;
using GLTFast;

public class GltfLoader : MonoBehaviour {
    GameObject _loadedModel;

    void Start() { Load3DModel("Models/Cristo.glb"); }

    async void Load3DModel(string fileName) {
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        GltfImport gltf = new();

        bool success = await gltf.Load(fullPath);

        if (success) {
            _loadedModel = new GameObject("LoadedModel");
            await gltf.InstantiateMainSceneAsync(_loadedModel.transform);
        } else {
            Debug.LogError($"Falha ao carregar o modelo {fileName}.");
        }
    }
}