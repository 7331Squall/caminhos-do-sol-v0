using System;

public static class UtilsJs
{
    /*
     * Force a value into a number.
     */
    public static double ForceNumber(double n) {
        if (double.IsNaN(n)) { n = 0; }
        return n;
    }

    public static double roundToOnePlace(double n) { return Math.Round(n * 10) / 10; }

    /**
     * https://en.wikipedia.org/wiki/Solar_zenith_angle
     * 
     * Units are in radians for everything here.
     * 
     * If altitude is true, returns altitude instead of zenith (the
     * complement of this angle).
     */
    public static double GetSolarZenith(double latitude, double declination, double hourAngle, bool altitude = false) {
        double angle =
            Math.Sin(latitude) * Math.Sin(declination)
          + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hourAngle);
        if (altitude) { return Math.Asin(angle); }
        double zenith = Math.Acos(Math.Min(Math.Max(angle, -1), 1));
        return zenith;
    }

    /**
     * Get the sun's azimuth angle, given sun zenith,
     * hour angle, declination, and observer latitude.
     * 
     * Units are in radians for everything here.
     * 
     * https://en.wikipedia.org/wiki/Solar_azimuth_angle
     */
    public static double GetSolarAzimuth(double zenithAngle, double hourAngle, double declination, double latitude) {
        double cosPhi =
            (Math.Sin(declination) * Math.Cos(latitude)
              - Math.Cos(hourAngle) * Math.Cos(declination) * Math.Sin(latitude))
          / Math.Sin(zenithAngle);
        double az = Math.Acos(Math.Min(Math.Max(cosPhi, -1), 1));

        // The angle offset needs to be adjusted based on whether the hour angle
        // is in the morning or the evening.
        //   https://en.wikipedia.org/wiki/Solar_azimuth_angle#Formulas
        if (hourAngle is < 0 or > Math.PI) { return az; }
        return Math.PI * 2 - az;
    }

    /**
     * Convert the solar hour angle (not a clock's hour angle) from hours
     * to radians.
     */
    public static double HourAngleToRadians(double hours) { return hours / 24 * (Math.PI * 2); }

    /**
     * Convert angle for the hour hand of a clock to the hour.
     */
    public static double HourAngleToTime(double angle) {
        double hour = angle / (Math.PI * 2) * 24;
        return hour;
    }

    /**
     * Convert angle for the minute hand of a clock to the minute.
     */
    public static double MinuteAngleToTime(double angle) {
        double minute = angle / (Math.PI * 2) * 60;
        return minute;
    }

    public static double DegToRad(double degrees) { return degrees * Math.PI / 180; }
    public static double RadToDeg(double radians) { return radians * 180 / Math.PI; }

    /**
     * Calculate sidereal time.
     */
    public static double GetSiderealTime(double day) {
        // From the original source code
        return 24 * (((0.280464857844662 + 1.0027397260274 * day) % 1 + 1) % 1);
    }

    /**
     * Returns the hour angle as an hour decimal number.
     */
    public static double GetHourAngle(double siderealTime, double rightAscension) {
        // Calculation from original (actionscript) source
        double hourAngle = (siderealTime - rightAscension) % 24 % 24;

        // The hour angle shouldn't return values more than 12.
        // "18h 3m" should be displayed as "-5h 57m".
        if (hourAngle > 12) { hourAngle -= 24; }
        if (hourAngle < -12) { hourAngle += 24; }
        return hourAngle;
    }

    /**
     * Given a Date object, return the day of year, and the date's time as
     * a fraction.
     * 
     * Taken from: https://stackoverflow.com/a/8619946
     */
    public static double GetDayOfYear(DateTimeOffset d) {
        DateTimeOffset start = new(d.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
        int diff =
            (int)(d - start).TotalMilliseconds
          + (int)((start.ToLocalTime().Offset - d.ToLocalTime().Offset).TotalMinutes * 60 * 1000);
        const int oneDay = 1000 * 60 * 60 * 24;
        double day = Math.Floor((float)diff / oneDay);

        // Add the time of day as a fraction of 1.
        int time = GetTime(d);
        day += time;
        return day;
    }

    /**
     * Given a JS Date object, return its time of day as a number between
     * 0 and 1.
     */
    public static int GetTime(DateTimeOffset d) {
        int time = 0;
        DateTime dt = d.UtcDateTime;
        time += dt.Hour / 24;
        time += dt.Minute / 60 / 24;
        time += dt.Second / 60 / 60 / 24;
        return time;
    }

    /**
     * Format a decimal of minutes as: minutes:seconds
     */
    public static string FormatMinutes(double n) {
        bool isNegative = n < 0;
        n = Math.Abs(n);
        double minutes = Math.Floor(n);
        double r = n - minutes;
        double seconds = Math.Round(r * 60);
        char negDisplay = isNegative ? '-' : ' ';
        double secDisplay = seconds < 10 ? '0' + seconds : seconds;
        return $"{negDisplay}${minutes}:${secDisplay}";
    }

    /**
     * Format a decimal of hours as: Hh Mm
     * 
     * For example: 2.25 -> 2h 15m
     */
    public static string FormatHours(double n) {
        bool isNegative = n < 0;
        n = Math.Abs(n);
        double hours = Math.Floor(n);
        double r = n - hours;
        double minutes = ForceNumber(Math.Floor(r * 60));
        char negDisplay = isNegative ? '-' : ' ';
        return $"{negDisplay}${hours}h ${minutes}m";
    }

    // https://gist.github.com/chris-siedell/b5de8dae41cfa8a5ad67a1501aeeab47
    public static double GetEqnOfTime(double day) {
        // this function returns the equation of time in radians
        return -4.3796019e-6
          + 0.001830724 * Math.Cos(0.017214206 * day)
          - 0.032070267 * Math.Sin(0.017214206 * day)
          - 0.015952904 * Math.Cos(0.034428413 * day)
          - 0.04026479 * Math.Sin(0.034428413 * day)
          - 0.00044373354 * Math.Cos(0.051642619 * day)
          - 0.0013114725 * Math.Sin(0.051642619 * day)
          - 0.00064591583 * Math.Cos(0.068856825 * day)
          - 0.00070547099 * Math.Sin(0.068856825 * day);
    }

    // https://gist.github.com/chris-siedell/b5de8dae41cfa8a5ad67a1501aeeab47
    public static Position GetPosition(double day) {
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
        Position obj =
            new((12 / Math.PI * ra % 24 + 24) % 24, 180 / Math.PI * Math.Atan2(Math.Sin(ra), 2.30644456403329));
        return obj;
    }

    public struct Position
    {
        public Position(double ra, double dec) {
            _ra = ra;
            _dec = dec;
        }

        double _ra;
        double _dec;
    }
}