using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GLTFast;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ModelSelectField : MonoBehaviour {
    TMP_Dropdown _modelSelectDropdown;

    List<ModelData> _modelList = new();

    GameObject _loadedModel;

    [SerializeField]
    Mesh fallbackMesh;

    SceneManager _sceneManager;

    void PopulateModelSelectDropdown() {
        // TODO: Tirar o Hard-Code e Refatorar essa classe toda
        List<string> options = new() { "- Cilindro -" };
        options.AddRange(_modelList.Select(modelProp => modelProp.ModelName));
        _modelSelectDropdown.ClearOptions();
        _modelSelectDropdown.AddOptions(options);
        _modelSelectDropdown.onValueChanged.AddListener(ChangeModel);
    }

    void Awake() {
        _sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        _modelSelectDropdown = GetComponent<TMP_Dropdown>();
        // ReSharper disable once IteratorMethodResultIsIgnored
    }

    void Start() { StartCoroutine(LoadList()); }

    IEnumerator LoadList() {
        _sceneManager.DoLoading("Carregando lista de modelos...");
        string fullPath = Path.Combine(Application.streamingAssetsPath, "models.json");
        UnityWebRequest www = UnityWebRequest.Get(fullPath);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success) {
            _sceneManager.DisplayMessage($"Erro ao carregar JSON: {www.error}");
        } else {
            string json = www.downloadHandler.text;
            _modelList = JsonConvert.DeserializeObject<List<ModelData>>(json);
            PopulateModelSelectDropdown();
        }
        _sceneManager.TryToFinishLoading();
    }

    async void ChangeModel(int index) {
        try {
            int actualIdx = index - 1;
            if (Math.Clamp(actualIdx, -1, _modelList.Count) != actualIdx) {
                return;
            }
            if (actualIdx == -1) {
                DisplayFallbackModel();
                return;
            }
            _sceneManager.DoLoading($"Carregando {_modelList[actualIdx].ModelName}");
            await Load3DModel(actualIdx);
        } catch (Exception ex) {
            _sceneManager.DisplayMessage($"Erro ao trocar modelo: {ex.Message}");
            DisplayFallbackModel();
        }
    }

    async Task Load3DModel(int index) {
        ModelData prop = _modelList[index];
        string fullPath = Path.Combine(Application.streamingAssetsPath, prop.ModelPath);
        GltfImport gltf = new();
        bool success = await gltf.Load(fullPath);
        if (success) {
            Destroy(_loadedModel);
            SetFallbackModelActive(false);

            _loadedModel = null; //Needed to ensure I won't do stuff on the destroyed object
            _loadedModel = new GameObject("LoadedModel") {
                transform = { rotation = Quaternion.Euler(0f, prop.RotationDegrees, 0f), localScale = new Vector3(0.1f, 0.1f, 0.1f) }
            };
            await gltf.InstantiateMainSceneAsync(_loadedModel.transform);
            _sceneManager.TryToFinishLoading();
        } else {
            _sceneManager.DisplayMessage($"Falha ao carregar o modelo {fullPath}.");
        }
    }

    void SetFallbackModelActive(bool isActive) => _sceneManager.FallbackModel.gameObject.SetActive(isActive);

    void DisplayFallbackModel() {
        Destroy(_loadedModel);
        _sceneManager.UpdateProps(null);
        SetFallbackModelActive(true);
        _sceneManager.TryToFinishLoading();
    }
}

// void DestroyModel() {
//     if (!_loadedModel) {
//         return;
//     }
//     foreach (AssetReference mesh in meshes) {
//         try {
//             mesh?.ReleaseInstance(_loadedModel);
//         } catch (Exception e) {
//             Debug.LogError(JsonUtility.ToJson(e));
//         }
//     }
// }

// void MeshLoadingCompleted(AsyncOperationHandle<GameObject> obj) {
//     if (obj.Status == AsyncOperationStatus.Succeeded) {
//         Destroy(_loadedModel);
//         _loadedModel = null; //Needed to ensure I won't do stuff on the destroyed object
//         _loadedModel = obj.Result;
//         _sceneManager.UpdateProps(_loadedModel.GetComponent<OrbitalCameraProperties>());
//         _sceneManager.TryToFinishLoading();
//     } else {
//         _sceneManager.DisplayMessage(obj.OperationException.Message);
//         DisplayFallbackModel();
//     }
// }