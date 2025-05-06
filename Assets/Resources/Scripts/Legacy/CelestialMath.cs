using System;

namespace Resources.Scripts
{
    public static class CelestialMath
    {
        // Epoch J2000.0: 2000-01-01 12:00:00 UTC
        public static readonly DateTime Epoch = new(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        public static double DegToRad(double degrees) { return degrees * Math.PI / 180.0; }
        public static double RadToDeg(double radians) { return radians * 180.0 / Math.PI; }

        public static double Century(DateTime date) {
            double deltaSeconds = (date.ToUniversalTime() - Epoch).TotalSeconds;
            return deltaSeconds / 31557600000.0;
        }

        public static double GetSiderealTime(int doy) {
            // From the original source code
            return 24 * (((0.280464857844662 + 1.0027397260274 * doy) % 1 + 1) % 1);
        }

        // https://gist.github.com/chris-siedell/b5de8dae41cfa8a5ad67a1501aeeab47
        public static (double ra, double dec) GetPosition(double day) {
            // this function returns the right ascension in decimal hours and
            // the declination in degrees
            double ra =
                0.01721421 * day
              - 1.3793756
              - 0.001830724 * Math.Cos(0.017214206 * day)
              + 0.032070267 * Math.Sin(0.017214206 * day)
              + 0.015952904 * Math.Cos(0.034428413 * day)
              + 0.04026479 * Math.Sin(0.034428413 * day)
              + 0.00044373354 * Math.Cos(0.051642619 * day)
              + 0.0013114725 * Math.Sin(0.051642619 * day)
              + 0.00064591583 * Math.Cos(0.068856825 * day)
              + 0.00070547099 * Math.Sin(0.068856825 * day);
            return (ra: (12 / Math.PI * ra % 24 + 24) % 24,
                dec: 180 / Math.PI * Math.Atan2(Math.Sin(ra), 2.30644456403329));
        }

        public static (double siderealTime, double hourAngle, double sunDeclination) DateUpdateData(
            DateTime currentTime
        ) {
            double siderealTime = GetSiderealTime(currentTime.DayOfYear);
            double hourAngle = GetHourAngle(siderealTime, GetPosition(currentTime.DayOfYear).ra);
            double centuryDate = Century(currentTime);
            double sunDeclination = DegToRad(SolarDeclination(centuryDate));
            return (siderealTime, hourAngle, sunDeclination);
        }

        /**
         * Given the sun declination in radians, return the radius of this
         * orbit on the sphere.
         */
        public static double GetSunDeclinationRadius(double sphereRadius, double sunDeclination) {
            return sphereRadius * (Math.Cos(sunDeclination) * 1.25);
        }

        public static double MeanLongitude(double t) {
            double l = 280.46646 + t * (36000.76983 + t * 0.0003032);
            l %= 360.0;
            return l < 0 ? l + 360.0 : l;
        }

        public static double MeanAnomaly(double t) { return 357.52911 + t * (35999.05029 - 0.0001537 * t); }

        public static double EquationOfCenter(double t) {
            double m = DegToRad(MeanAnomaly(t));
            double sinM = Math.Sin(m);
            double sin2M = Math.Sin(2 * m);
            double sin3M = Math.Sin(3 * m);
            return sinM * (1.914602 - t * (0.004817 + 0.000014 * t))
              + sin2M * (0.019993 - 0.000101 * t)
              + sin3M * 0.000289;
        }

        public static double TrueLongitude(double t) { return MeanLongitude(t) + EquationOfCenter(t); }

        public static double ApparentLongitude(double t) {
            double omega = 125.04 - 1934.136 * t;
            return TrueLongitude(t) - 0.00569 - 0.00478 * Math.Sin(DegToRad(omega));
        }

        public static double ObliquityOfEcliptic(double t) {
            double e0 = 23.0 + (26.0 + (21.448 - t * (46.815 + t * (0.00059 - t * 0.001813))) / 60.0) / 60.0;
            double omega = 125.04 - 1934.136 * t;
            return e0 + 0.00256 * Math.Cos(DegToRad(omega));
        }

        public static double SolarDeclination(double t) {
            double e = ObliquityOfEcliptic(t);    // graus
            double lambda = ApparentLongitude(t); // graus
            return RadToDeg(Math.Asin(Math.Sin(DegToRad(e)) * Math.Sin(DegToRad(lambda))));
        }

        public static double GetHourAngle(double siderealTime, double rightAscension) {
            // Calculation from original (actionscript) source
            double hourAngle = (siderealTime - rightAscension) % 24 % 24;

            // The hour angle shouldn't return values more than 12.
            // "18h 3m" should be displayed as "-5h 57m".
            if (hourAngle > 12) { hourAngle -= 24; }
            if (hourAngle < -12) { hourAngle += 24; }
            return hourAngle;
        }
    }
}