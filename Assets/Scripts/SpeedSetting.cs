public enum SpeedPerSecond
{
    OneMinute,
    FiveMinutes,
    FifteenMinutes,
    ThirtyMinutes,
    OneHour,
    TwoHours,
    ThreeHours,
    SixHours,
    TwelveHours,
    OneDay,
    OneWeek,
    TwoWeeks,
    OneMonth,
    ThreeMonths
}

public static class SpeedSetting
{
    public static string ToDisplayString(SpeedPerSecond value) {
        return value switch {
            SpeedPerSecond.OneMinute      => "1 min/s",
            SpeedPerSecond.FiveMinutes    => "5 min/s",
            SpeedPerSecond.FifteenMinutes => "15 min/s",
            SpeedPerSecond.ThirtyMinutes  => "30 min/s",
            SpeedPerSecond.OneHour        => "1 hora/s",
            SpeedPerSecond.TwoHours       => "2 horas/s",
            SpeedPerSecond.ThreeHours     => "3 horas/s",
            SpeedPerSecond.SixHours       => "6 horas/s",
            SpeedPerSecond.TwelveHours    => "12 horas/s",
            SpeedPerSecond.OneDay         => "1 dia/s",
            SpeedPerSecond.OneWeek        => "7 dias/s",
            SpeedPerSecond.TwoWeeks       => "14 dias/s",
            SpeedPerSecond.OneMonth       => "30 dias/s",
            SpeedPerSecond.ThreeMonths    => "120 dias/s",
            var _                         => value.ToString()
        };
    }

    public static int SimSecondsPerSecond(SpeedPerSecond value) {
        return value switch {
            SpeedPerSecond.OneMinute      => 60,
            SpeedPerSecond.FiveMinutes    => 300,
            SpeedPerSecond.FifteenMinutes => 900,
            SpeedPerSecond.ThirtyMinutes  => 1800,
            SpeedPerSecond.OneHour        => 3600,
            SpeedPerSecond.TwoHours       => 7200,
            SpeedPerSecond.ThreeHours     => 10800,
            SpeedPerSecond.SixHours       => 21600,
            SpeedPerSecond.TwelveHours    => 43200,
            SpeedPerSecond.OneDay         => 86400,
            SpeedPerSecond.OneWeek        => 604800,
            SpeedPerSecond.TwoWeeks       => 1209600,
            SpeedPerSecond.OneMonth       => 2592000,
            SpeedPerSecond.ThreeMonths    => 7776000,
            var _                         => (int)value
        };
    }
}