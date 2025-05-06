using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LatitudeField : MonoBehaviour
{
    public TMP_InputField latitudeValue;
    public TMP_Text latitudeDirection;

    [FormerlySerializedAs("Latitude")]
    [SerializeField]
    float latitude;

    public float Latitude {
        get => latitude;
        set {
            latitude = value;
            latitudeValue.text = Mathf.Abs(latitude).ToString("F1");
            latitudeDirection.text = latitude > 0 ? "N" : "S";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { Latitude = latitude; }

    // public void UpdateLatitude()
    // {
    //     latitudeValue.text = Mathf.Abs(Latitude).ToString("F1");
    // }
    public void LatitudeButtonClicked() { Latitude = -latitude; }
    public float GetValue() { return latitude; }
    public void SetValue(string newValue) { Latitude = float.Parse(newValue); }

    // Update is called once per frame
    // void Update()
    // {
    // }
}