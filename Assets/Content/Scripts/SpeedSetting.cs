using UnityEngine.Rendering;
public enum SpeedSettings {
    TwoMinutes,
    FiveMinutes,
    FifteenMinutes,
    ThirtyMinutes,
    OneHour,
    TwoHours,
    ThreeHours,
    SixHours,
    TwelveHours,
    OneDay
}

public enum IntervalSettings {
    Continuous,
    OneWeek,
    TwoWeeks,
    OneMonth,
    ThreeMonths
}

public static class SpeedSetting {
    public static string ToDisplayString(SpeedSettings value) {
        return value switch {
            SpeedSettings.TwoMinutes     => "2 min/s",
            SpeedSettings.FiveMinutes    => "5 min/s",
            SpeedSettings.FifteenMinutes => "15 min/s",
            SpeedSettings.ThirtyMinutes  => "30 min/s",
            SpeedSettings.OneHour        => "1 hora/s",
            SpeedSettings.TwoHours       => "2 horas/s",
            SpeedSettings.ThreeHours     => "3 horas/s",
            SpeedSettings.SixHours       => "6 horas/s",
            SpeedSettings.TwelveHours    => "12 horas/s",
            SpeedSettings.OneDay         => "1 dia/s",
            var _                        => value.ToString()
        };
    }

    public static int SimSecondsPerSecond(int value) {
        return SimSecondsPerSecond((SpeedSettings) value);
    }

    public static int SimSecondsPerSecond(SpeedSettings value) {
        return value switch {
            SpeedSettings.TwoMinutes     => 120,
            SpeedSettings.FiveMinutes    => 300,
            SpeedSettings.FifteenMinutes => 900,
            SpeedSettings.ThirtyMinutes  => 1800,
            SpeedSettings.OneHour        => 3600,
            SpeedSettings.TwoHours       => 7200,
            SpeedSettings.ThreeHours     => 10800,
            SpeedSettings.SixHours       => 21600,
            SpeedSettings.TwelveHours    => 43200,
            SpeedSettings.OneDay         => 86400,
            // SpeedSettings.OneWeek      => 604800,
            // SpeedSettings.TwoWeeks       => 1209600,
            // SpeedSettings.OneMonth       => 2592000,
            // SpeedSettings.ThreeMonths    => 7776000,
            var _ => (int) value
        };
    }
}


public static class IntervalSetting {
    public static string ToDisplayString(IntervalSettings value) {
        return value switch {
            IntervalSettings.Continuous  => "∞",
            IntervalSettings.OneWeek     => "7 dias",
            IntervalSettings.TwoWeeks    => "14 dias",
            IntervalSettings.OneMonth    => "1 mês",
            IntervalSettings.ThreeMonths => "3 meses",
            var _                        => value.ToString()
        };
    }

    public static int SimSecondsPerSecond(int value) {
        return SimSecondsPerSecond((IntervalSettings) value);
    }

    public static int SimSecondsPerSecond(IntervalSettings value) {
        return value switch {
            IntervalSettings.OneWeek     => 604800,
            IntervalSettings.TwoWeeks    => 1209600,
            IntervalSettings.OneMonth    => 2592000,
            IntervalSettings.ThreeMonths => 7776000,
            var _                        => (int) value
        };
    }
}