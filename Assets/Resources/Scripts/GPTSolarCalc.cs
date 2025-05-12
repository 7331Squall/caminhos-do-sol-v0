using System;
using UnityEngine;

// ReSharper disable InconsistentNaming
public static class GPTSolarCalc
{
    // [Header("Inputs")]
    // [Range(-90f, 90f)]
    // public float latitude;      // graus (positivo = norte)
    // [Range(-180f, 180f)]
    // public float longitude;     // graus (positivo = leste)
    // public int day;             // 1-31
    // public int month;           // 1-12
    // [Range(0,23)]
    // public int hour;            // 0-23
    // [Range(0,59)]
    // public int minute;          // 0-59
    // public float timeZone = 0;  // horas de UTC (ex: -3 para Brasil)
    //
    // [Header("Light")]
    // public Light sunLight;

    public static (Vector3 position, Quaternion rotation) GetPositionNOAA(
        float latitude,
        // float longitude, 
        DateTime dateTime
        // , float timeZone
    ) {
        int day = dateTime.Day;
        int month = dateTime.Month;
        int hour = dateTime.Hour;
        int minute = dateTime.Minute;

        // 1) Cálculo do dia do ano (N)
        DateTime dt = new DateTime(2000, month, day, hour, minute, 0);
        int N = dt.DayOfYear;

        // 2) Ângulo do ano (γ), em radianos
        double gamma = 2.0 * Math.PI / 365.0 * (N - 1 + ((hour - 12.0) / 24.0) + minute / 1440.0);

        // 3) Declinação solar δ, em radianos
        double delta =
            0.006918
          - 0.399912 * Math.Cos(gamma)
          + 0.070257 * Math.Sin(gamma)
          - 0.006758 * Math.Cos(2 * gamma)
          + 0.000907 * Math.Sin(2 * gamma)
          - 0.002697 * Math.Cos(3 * gamma)
          + 0.001480 * Math.Sin(3 * gamma);

        // 4) Equação do tempo (EoT), em minutos
        // double eqTime =
        //     229.18
        //   * (0.000075
        //       + 0.001868 * Math.Cos(gamma)
        //       - 0.032077 * Math.Sin(gamma)
        //       - 0.014615 * Math.Cos(2 * gamma)
        //       - 0.040849 * Math.Sin(2 * gamma));

        // 5) Tempo Solar Verdadeiro (TST), em minutos
        // double timeOffset = eqTime + 4.0 * longitude - 60.0 * timeZone;
        // double tst = hour * 60.0 + minute + timeOffset;
        // tst = ((tst % 1440) + 1440) % 1440; // garantir [0,1440)
        double tst = hour * 60.0 + minute; // em minutos

        // 6) Ângulo horário H, em radianos
        double hourAngle = (tst / 4.0 - 180.0) * Mathf.Deg2Rad;

        // 7) Elevação (h) e azimute (A)
        double latRad = latitude * Mathf.Deg2Rad;
        double cosZenith =
            Math.Sin(latRad) * Math.Sin(delta) + Math.Cos(latRad) * Math.Cos(delta) * Math.Cos(hourAngle);
        double zenith = Math.Acos(cosZenith);
        double elevation = Math.PI / 2.0 - zenith;
        double sinAz = -Math.Cos(delta) * Math.Sin(hourAngle) / Math.Sin(zenith);
        double cosAz = (Math.Sin(delta) - Math.Sin(latRad) * Math.Cos(zenith)) / (Math.Cos(latRad) * Math.Sin(zenith));
        double azimuth = Math.Atan2(sinAz, cosAz);
        if (azimuth < 0) azimuth += 2 * Math.PI;

        // 8) Vetor direção no sistema Unity (X=leste, Y=cima, Z=norte)
        float x = (float)(Math.Cos(elevation) * Math.Sin(azimuth));
        float y = (float)(Math.Sin(elevation));
        float z = (float)(Math.Cos(elevation) * Math.Cos(azimuth));

        // 9) Aplica rotação na luz direcional para “apontar” para a origem
        Vector3 sunDir = new Vector3(x, y, z).normalized;
        Quaternion rotation = Quaternion.LookRotation(-sunDir, Vector3.up);
        return (sunDir, rotation);
    }
}