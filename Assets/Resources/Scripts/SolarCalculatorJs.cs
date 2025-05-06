using System;
using static UtilsJs;

public static class SolarCalculatorJs
{
    // var epoch = Date.UTC(2000, 0, 1, 12); // J2000.0
    const long Epoch = 946728000000;
    // https://github.com/mbostock/solar-calculator Version 0.3.0. Copyright 2017 Mike Bostock.
    // (function (global, factory) {
    // 	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
    // 	typeof define === 'function' && define.amd ? define(['exports'], factory) :
    // 	(factory((global.solar = global.solar || {})));
    // }(this, (function (exports) { 'use strict';

    // var acos = Math.acos;
    // var asin = Math.asin;
    // var cos = Math.cos;
    // var pi = Math.PI;
    // var pow = Math.pow;
    // var sin = Math.sin;
    // var tan = Math.tan;

    // Given t in J2000.0 centuries, returns the sun’s mean longitude in degrees.
    // https://en.wikipedia.org/wiki/Mean_longitude
    public static double MeanLongitude(double t) {
        double l = (280.46646 + t * (36000.76983 + t * 0.0003032)) % 360;
        return l < 0 ? l + 360 : l;
    }

    // Given t in J2000.0 centuries, returns the sun’s mean anomaly in degrees.
    // https://en.wikipedia.org/wiki/Mean_anomaly
    public static double MeanAnomaly(double t) { return 357.52911 + t * (35999.05029 - 0.0001537 * t); }

    // Given t in J2000.0 centuries, returns the sun’s equation of the center in degrees.
    // https://en.wikipedia.org/wiki/Equation_of_the_center
    public static double EquationOfCenter(double t) {
        double m = DegToRad(MeanAnomaly(t));
        double sin1M = Math.Sin(m);
        double sin2M = Math.Sin(m * 2);
        double sin3M = Math.Sin(m * 3);
        return sin1M * (1.914602 - t * (0.004817 + 0.000014 * t))
          + sin2M * (0.019993 - 0.000101 * t)
          + sin3M * 0.000289;
    }

    // Given t in J2000.0 centuries, returns the sun’s true longitude in degrees.
    // https://en.wikipedia.org/wiki/True_longitude
    public static double TrueLongitude(double t) { return MeanLongitude(t) + EquationOfCenter(t); }

    // Given t in J2000.0 centuries, returns the sun’s apparent longitude in degrees.
    // https://en.wikipedia.org/wiki/Apparent_longitude
    public static double ApparentLongitude(double t) {
        return TrueLongitude(t) - 0.00569 - 0.00478 * Math.Sin(DegToRad(125.04 - 1934.136 * t));
    }
    //LongFromDateTime(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));

    static long LongFromDateTime(DateTime dateTime) {
        return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds; // J2000.0
    }

    public static long Century(long date) { return (date - Epoch) / (long)315576e7; }
    public static long Century(DateTime date) { return Century(LongFromDateTime(date)); }
    public static long Century(double date) { return Century((long)date); }

    // Given t in J2000.0 centuries, returns the obliquity of the Earth’s ecliptic in degrees.
    public static double ObliquityOfEcliptic(double t) {
        double e0 = 23 + (26 + (21.448 - t * (46.815 + t * (0.00059 - t * 0.001813))) / 60) / 60;
        double omega = 125.04 - 1934.136 * t;
        double e = e0 + 0.00256 * Math.Cos(DegToRad(omega));
        return e;
    }

    // Given t in J2000.0 centuries, returns the solar declination in degrees.
    // https://en.wikipedia.org/wiki/Position_of_the_Sun#Declination_of_the_Sun_as_seen_from_Earth
    public static double Declination(double t) {
        return RadToDeg(
            Math.Asin(Math.Sin(DegToRad(ObliquityOfEcliptic(t))) * Math.Sin(DegToRad(ApparentLongitude(t))))
        );
    }

    // Given t in J2000.0 centuries, returns eccentricity.
    // https://en.wikipedia.org/wiki/Orbital_eccentricity
    public static double OrbitEccentricity(double t) { return 0.016708634 - t * (0.000042037 + 0.0000001267 * t); }

    // Given t in J2000.0 centuries, returns the equation of time in minutes.
    // https://en.wikipedia.org/wiki/Equation_of_time
    public static double EquationOfTime(double t) {
        double epsilon = ObliquityOfEcliptic(t);
        double l0 = MeanLongitude(t);
        double e = OrbitEccentricity(t);
        double m = MeanAnomaly(t);
        double y = Math.Pow(Math.Tan(DegToRad(epsilon) / 2), 2);
        double sin2L0 = Math.Sin(2 * DegToRad(l0));
        double sin1M = Math.Sin(DegToRad(m));
        double cos2L0 = Math.Cos(2 * DegToRad(l0));
        double sin4L0 = Math.Sin(4 * DegToRad(l0));
        double sin2M = Math.Sin(2 * DegToRad(m));
        double etime =
            y * sin2L0 - 2 * e * sin1M + 4 * e * y * sin1M * cos2L0 - 0.5 * y * y * sin4L0 - 1.25 * e * e * sin2M;
        return RadToDeg(etime) * 4;
    }

    public static double RiseHourAngle(long date, double latitude) {
        double phi = DegToRad(latitude);
        double theta = DegToRad(Declination(Century(date)));
        return -RadToDeg(
            Math.Acos(Math.Cos(DegToRad(90.833)) / (Math.Cos(phi) * Math.Cos(theta)) - Math.Tan(phi) * Math.Tan(theta))
        );
    }

    public static double Hours(long date, double latitude) {
        double delta = -RiseHourAngle(date, latitude);
        if (!double.IsNaN(delta)) return 8 * delta / 60;
        delta = Declination(Century(date));
        return (latitude < 0 ? delta < 0.833 ? 1 : 0 :
                delta > -0.833 ? 1 : 0)
          * 24;
    }

    static long Day(long date) {
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeMilliseconds(date);
        dto = new DateTimeOffset(dto.Year, dto.Month, dto.Day, 0, 0, 0, TimeSpan.Zero);
        return dto.ToUnixTimeMilliseconds();
    }

    public static long Noon(long date, double longitude) {
        // First approximation.
        long t = Century(Day(date) + (12 - longitude * 24 / 360) * 36e5);
        // First correction.
        double o1 = 720 - longitude * 4 - EquationOfTime(t - longitude / (360 * 36525));
        // Second correction.
        double o2 = 720 - longitude * 4 - EquationOfTime(t + o1 / (1440 * 36525));
        return (long)(Day(date) + o2 * 1000 * 60);
    }

    public static long Rise(long date, double latitude, double longitude) {
        date = Noon(date, longitude);
        return (long)(date + RiseHourAngle(date, latitude) * 4 * 1000 * 60);
    }

    public static long Set(long date, double latitude, double longitude) {
        date = Noon(date, longitude);
        return (long)(date - RiseHourAngle(date, latitude) * 4 * 1000 * 60);
    }
}