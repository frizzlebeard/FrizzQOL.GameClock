using GameClock;
using Xunit;

public class ClockTextTests
{
    [Theory]
    [InlineData(0f, false, true, "12:00 AM")]
    [InlineData(0.25f, false, true, "6:00 AM")]
    [InlineData(0.5f, false, true, "12:00 PM")]
    [InlineData(0.75f, false, true, "6:00 PM")]
    [InlineData(1f, false, true, "12:00 AM")]
    [InlineData(0f, true, true, "00:00")]
    [InlineData(0.25f, true, true, "06:00")]
    [InlineData(0.5f, true, true, "12:00")]
    [InlineData(0.75f, true, true, "18:00")]
    [InlineData(1f, true, true, "00:00")]
    public void Clock_boundaries(float fraction, bool use24Hour, bool showAmPm, string expected)
    {
        ClockSnapshot snapshot = Sample(fraction);
        ClockOptions options = ClockOptions.TimeOnly(use24Hour, showAmPm);
        Assert.Equal(expected, ClockText.Format(snapshot, options));
    }

    [Fact]
    public void Just_under_one_stays_in_the_last_hour()
    {
        ClockSnapshot snapshot = Sample(1439f / 1440f);
        Assert.Equal("11:59 PM", ClockText.Format(snapshot, ClockOptions.TimeOnly(false, true)));
        Assert.Equal("23:59", ClockText.Format(snapshot, ClockOptions.TimeOnly(true, true)));
    }

    [Fact]
    public void Twelve_hour_without_am_pm_omits_the_suffix()
    {
        Assert.Equal("6:00", ClockText.Format(Sample(0.25f), ClockOptions.TimeOnly(false, false)));
    }

    [Theory]
    [InlineData("24h", true)]
    [InlineData("12h", false)]
    [InlineData("nope", false)]
    [InlineData("", false)]
    public void Format_string_resolves_to_24_hour_only_for_24h(string format, bool use24Hour)
    {
        Assert.Equal(use24Hour, ClockText.Use24Hour(format));
    }

    [Theory]
    [InlineData("TopRight", "TopRight")]
    [InlineData("TopLeft", "TopLeft")]
    [InlineData("TopCenter", "TopCenter")]
    [InlineData("Middle", "TopRight")]
    public void Corner_falls_back_to_top_right(string corner, string expected)
    {
        Assert.Equal(expected, ClockText.NormalizeCorner(corner));
    }

    [Fact]
    public void NaN_fraction_is_not_ready()
    {
        Assert.False(ClockText.IsTimeReady(float.NaN));
        Assert.True(ClockText.IsTimeReady(0f));
    }

    [Fact]
    public void Full_label_keeps_line_order()
    {
        string text = ClockText.Format(Sample(0.25f), ClockOptions.AllOn(false));
        Assert.Equal(
            "6:00 AM \u00b7 Day 12\nClear \u00b7 Wind NE 70%\nCold, Wet",
            text);
    }

    [Fact]
    public void Hidden_day_weather_biome_and_wind_are_omitted()
    {
        ClockOptions options = ClockOptions.AllOn(false);
        options.ShowDay = false;
        options.ShowWeather = false;
        options.ShowWind = false;
        Assert.Equal("6:00 AM\nCold, Wet", ClockText.Format(Sample(0.25f), options));
    }

    [Fact]
    public void Empty_weather_and_biome_are_omitted()
    {
        ClockSnapshot snapshot = Sample(0.25f);
        snapshot.Weather = "";
        Assert.Equal(
            "6:00 AM \u00b7 Day 12\nWind NE 70%\nCold, Wet",
            ClockText.Format(snapshot, ClockOptions.AllOn(false)));
    }

    [Theory]
    [InlineData(0f, 1f, "N")]
    [InlineData(1f, 0f, "E")]
    [InlineData(0f, -1f, "S")]
    [InlineData(-1f, 0f, "W")]
    [InlineData(1f, 1f, "NE")]
    public void Wind_compass(float x, float z, string compass)
    {
        ClockSnapshot snapshot = Sample(0f);
        snapshot.WindX = x;
        snapshot.WindZ = z;
        snapshot.WindIntensity = 1f;
        snapshot.IsCold = false;
        snapshot.IsWet = false;
        ClockOptions options = ClockOptions.TimeOnly(false, true);
        options.ShowWind = true;
        Assert.Equal("12:00 AM\nWind " + compass + " 100%", ClockText.Format(snapshot, options));
    }

    [Fact]
    public void Calm_wind_has_no_compass_point()
    {
        ClockSnapshot snapshot = Sample(0f);
        snapshot.WindIntensity = 0.009f;
        snapshot.IsCold = false;
        snapshot.IsWet = false;
        ClockOptions options = ClockOptions.TimeOnly(false, true);
        options.ShowWind = true;
        Assert.Equal("12:00 AM\nWind 0%", ClockText.Format(snapshot, options));
    }

    [Fact]
    public void Wind_intensity_clamps_to_100()
    {
        ClockSnapshot snapshot = Sample(0f);
        snapshot.WindX = 0f;
        snapshot.WindZ = 1f;
        snapshot.WindIntensity = 1.4f;
        snapshot.IsCold = false;
        snapshot.IsWet = false;
        ClockOptions options = ClockOptions.TimeOnly(false, true);
        options.ShowWind = true;
        Assert.Equal("12:00 AM\nWind N 100%", ClockText.Format(snapshot, options));
    }

    [Fact]
    public void Status_words_follow_flags_and_settings()
    {
        ClockSnapshot snapshot = Sample(0f);
        snapshot.IsCold = true;
        snapshot.IsWet = true;
        snapshot.IsFreezing = true;
        ClockOptions options = ClockOptions.TimeOnly(false, false);
        options.ShowCold = true;
        options.ShowWet = true;
        options.ShowFreezing = true;
        Assert.Equal("12:00\nCold, Wet, Freezing", ClockText.Format(snapshot, options));

        options.ShowCold = false;
        snapshot.IsWet = false;
        Assert.Equal("12:00\nFreezing", ClockText.Format(snapshot, options));
    }

    [Fact]
    public void Status_line_is_omitted_when_every_enabled_flag_is_false()
    {
        ClockSnapshot snapshot = Sample(0.5f);
        snapshot.IsCold = false;
        snapshot.IsWet = false;
        snapshot.IsFreezing = false;
        Assert.Equal(
            "12:00 PM \u00b7 Day 12\nClear \u00b7 Wind NE 70%",
            ClockText.Format(snapshot, ClockOptions.AllOn(false)));
    }

    [Theory]
    [InlineData(false, true, "TopRight", true)]
    [InlineData(false, true, "TopLeft", true)]
    [InlineData(false, true, "TopCenter", true)]
    [InlineData(true, false, "TopRight", false)]
    [InlineData(true, true, "TopLeft", false)]
    [InlineData(true, true, "TopCenter", false)]
    [InlineData(false, false, "TopRight", false)]
    [InlineData(false, false, "TopLeft", true)]
    [InlineData(false, false, "TopCenter", true)]
    [InlineData(false, false, "Middle", false)]
    public void Clock_hides_on_the_big_map_and_with_a_missing_minimap(bool mapOpen, bool smallMapActive, string corner, bool shown)
    {
        Assert.Equal(shown, ClockText.ShouldShow(mapOpen, smallMapActive, corner));
    }

    [Fact]
    public void Label_sits_two_pixels_under_the_map()
    {
        Assert.Equal(-226f, ClockText.LabelTop(-224f, 0f));
        Assert.Equal(-216f, ClockText.LabelTop(-224f, 10f));
    }

    private static ClockSnapshot Sample(float fraction)
    {
        return new ClockSnapshot
        {
            DayFraction = fraction,
            Day = 12,
            Weather = "Clear",
            WindX = 1f,
            WindZ = 1f,
            WindIntensity = 0.7f,
            IsCold = true,
            IsWet = true,
            IsFreezing = false
        };
    }
}
