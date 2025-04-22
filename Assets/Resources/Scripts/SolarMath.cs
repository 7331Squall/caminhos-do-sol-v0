using System;
using UnityEngine;

public static class SolarMath
{
    public static Vector3 GetSunPosition(DateTime currentTime, float latitude) {
        double centuryTime = Century(currentTime);
        double declination = SolarDeclination(centuryTime); // graus

        // Ângulo horário: diferença entre hora solar e meio-dia
        double decimalHour = currentTime.TimeOfDay.TotalHours;
        double hourDegrees = (decimalHour - 12) * 15; // graus (15° por hora)
        float phi = Mathf.Deg2Rad * latitude;
        float delta = Mathf.Deg2Rad * (float)declination;
        float hourRadian = Mathf.Deg2Rad * (float)hourDegrees;

        // Altitude
        float sinAlt = Mathf.Sin(phi) * Mathf.Sin(delta) + Mathf.Cos(phi) * Mathf.Cos(delta) * Mathf.Cos(hourRadian);
        float altitude = Mathf.Asin(sinAlt);

        // Azimute
        float cosAz =
            (Mathf.Sin(delta) - Mathf.Sin(altitude) * Mathf.Sin(phi)) / (Mathf.Cos(altitude) * Mathf.Cos(phi));
        float azimuth = Mathf.Acos(Mathf.Clamp(cosAz, -1f, 1f));

        // Corrige sentido leste/oeste
        if (Mathf.Sin(hourRadian) > 0) azimuth = 2 * Mathf.PI - azimuth;

        // Converte azimute e altitude para posição 3D (esfera ao redor da origem)
        Vector3 direction =
            new Vector3(
                Mathf.Sin(azimuth) * Mathf.Cos(altitude), Mathf.Sin(altitude), Mathf.Cos(azimuth) * Mathf.Cos(altitude)
            );
        return direction;
    }

    // Converte graus pra radianos
    public static double Radians(double graus) { return graus * Math.PI / 180.0; }

    // Converte radianos pra graus
    public static double Degrees(double rad) { return rad * 180.0 / Math.PI; }

    // Número de séculos desde J2000.0
    public static double Century(DateTime date) {
        DateTime epoch = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc); // J2000.0
        double elapsedSeconds = (date.ToUniversalTime() - epoch).TotalSeconds;
        return elapsedSeconds / 315576000.0; // 100 anos julianos em segundos
    }

    // Obliquidade da eclíptica em graus
    public static double ObliquityOfEcliptic(double t) {
        double e0 = 23 + (26 + (21.448 - t * (46.815 + t * (0.00059 - t * 0.001813))) / 60.0) / 60.0;
        double omega = 125.04 - 1934.136 * t;
        return e0 + 0.00256 * Math.Cos(Radians(omega));
    }

    // Longitude média do Sol
    public static double MeanLongitude(double time) {
        double longitude = (280.46646 + time * (36000.76983 + time * 0.0003032)) % 360.0;
        return longitude < 0 ? longitude + 360.0 : longitude;
    }

    // Anomalia média do Sol
    public static double MeanAnomaly(double time) { return 357.52911 + time * (35999.05029 - 0.0001537 * time); }

    // Equação do centro (em graus)
    public static double EquationOfCenter(double time) {
        double median = Radians(MeanAnomaly(time));
        double sinMedian = Math.Sin(median);
        double sin2Median = Math.Sin(2 * median);
        double sin3Median = Math.Sin(3 * median);
        return sinMedian * (1.914602 - time * (0.004817 + 0.000014 * time))
          + sin2Median * (0.019993 - 0.000101 * time)
          + sin3Median * 0.000289;
    }

    // Longitude verdadeira do Sol
    public static double TrueLongitude(double time) { return MeanLongitude(time) + EquationOfCenter(time); }

    // Longitude aparente do Sol
    public static double ApparentLongitude(double time) {
        return TrueLongitude(time) - 0.00569 - 0.00478 * Math.Sin(Radians(125.04 - 1934.136 * time));
    }

    // Declinação solar em graus
    public static double SolarDeclination(double time) {
        double epsilon = Radians(ObliquityOfEcliptic(time));
        double lambda = Radians(ApparentLongitude(time));
        return Degrees(Math.Asin(Math.Sin(epsilon) * Math.Sin(lambda)));
    }
}