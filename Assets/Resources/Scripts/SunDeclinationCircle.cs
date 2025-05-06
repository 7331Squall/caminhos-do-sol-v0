using System;
using UnityEngine;

public class SunDeclinationCircle : MonoBehaviour
{
    DateTime _sunDeclination;
    public DateTime SunDeclination {
        get => _sunDeclination;
        set {
            _sunDeclination = value;
            //this.transform.rotation = Quaternion.Euler(0, 0, (float)value);
            Debug.Log(SolarMath.SolarDeclination(SolarMath.Century(value)));
        }
    }
}