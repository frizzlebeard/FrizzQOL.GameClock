using System;
using System.Collections.Generic;
using System.Globalization;

namespace GameClock
{
    public struct ClockSnapshot
    {
        public float DayFraction;
        public int Day;
        public string Weather;
        public float WindX;
        public float WindZ;
        public float WindIntensity;
        public bool IsCold;
        public bool IsWet;
        public bool IsFreezing;
    }

    public struct ClockOptions
    {
        public bool Use24Hour;
        public bool ShowAmPm;
        public bool ShowDay;
        public bool ShowWeather;
        public bool ShowWind;
        public bool ShowCold;
        public bool ShowWet;
        public bool ShowFreezing;

        public static ClockOptions TimeOnly(bool use24Hour, bool showAmPm)
        {
            return new ClockOptions
            {
                Use24Hour = use24Hour,
                ShowAmPm = showAmPm
            };
        }

        public static ClockOptions AllOn(bool use24Hour)
        {
            return new ClockOptions
            {
                Use24Hour = use24Hour,
                ShowAmPm = true,
                ShowDay = true,
                ShowWeather = true,
                ShowWind = true,
                ShowCold = true,
                ShowWet = true,
                ShowFreezing = true
            };
        }
    }

    public static class ClockText
    {
        public static bool Use24Hour(string clockFormat)
        {
            return clockFormat == "24h";
        }

        public static string NormalizeCorner(string corner)
        {
            if (corner == "TopLeft" || corner == "TopCenter" || corner == "TopRight")
            {
                return corner;
            }

            return "TopRight";
        }

        public static bool IsTimeReady(float dayFraction)
        {
            return !float.IsNaN(dayFraction) && !float.IsInfinity(dayFraction);
        }

        public static bool ShouldShow(bool mapOpen, bool smallMapActive, string corner)
        {
            if (mapOpen)
            {
                return false;
            }

            if (NormalizeCorner(corner) == "TopRight" && !smallMapActive)
            {
                return false;
            }

            return true;
        }

        public static float LabelTop(float mapBottom, float offsetY)
        {
            return mapBottom - 2f + offsetY;
        }

        public static string Format(ClockSnapshot snapshot, ClockOptions options)
        {
            List<string> when = new List<string>();
            string time = FormatTime(snapshot.DayFraction, options.Use24Hour, options.ShowAmPm);
            if (time.Length > 0)
            {
                when.Add(time);
            }

            if (options.ShowDay)
            {
                when.Add("Day " + snapshot.Day.ToString(CultureInfo.InvariantCulture));
            }

            List<string> place = new List<string>();
            if (options.ShowWeather && !string.IsNullOrEmpty(snapshot.Weather))
            {
                place.Add(snapshot.Weather);
            }

            if (options.ShowWind)
            {
                place.Add(FormatWind(snapshot.WindX, snapshot.WindZ, snapshot.WindIntensity));
            }

            List<string> lines = new List<string>();
            if (when.Count > 0)
            {
                lines.Add(Join(when));
            }

            if (place.Count > 0)
            {
                lines.Add(Join(place));
            }

            string status = FormatStatus(snapshot, options);
            if (status.Length > 0)
            {
                lines.Add(status);
            }

            return string.Join("\n", lines);
        }

        private static string Join(List<string> parts)
        {
            return string.Join(" \u00b7 ", parts);
        }

        private static string FormatTime(float dayFraction, bool use24Hour, bool showAmPm)
        {
            if (!IsTimeReady(dayFraction))
            {
                return "";
            }

            double totalMinutes = (double)dayFraction * 1440.0;
            if (totalMinutes < 0d || totalMinutes >= 1440d)
            {
                totalMinutes = 0d;
            }

            int minuteOfDay = (int)Math.Floor(totalMinutes + 1e-4);
            if (minuteOfDay >= 1440)
            {
                minuteOfDay = 0;
            }

            int hour24 = minuteOfDay / 60;
            int minute = minuteOfDay % 60;
            if (use24Hour)
            {
                return hour24.ToString("00", CultureInfo.InvariantCulture) + ":" + minute.ToString("00", CultureInfo.InvariantCulture);
            }

            int hour12 = hour24 % 12;
            if (hour12 == 0)
            {
                hour12 = 12;
            }

            string clock = hour12.ToString(CultureInfo.InvariantCulture) + ":" + minute.ToString("00", CultureInfo.InvariantCulture);
            if (!showAmPm)
            {
                return clock;
            }

            string suffix = hour24 < 12 ? "AM" : "PM";
            return clock + " " + suffix;
        }

        private static string FormatWind(float x, float z, float intensity)
        {
            if (intensity < 0.01f)
            {
                return "Wind 0%";
            }

            int percent = (int)Math.Round(intensity * 100d, MidpointRounding.AwayFromZero);
            if (percent < 0)
            {
                percent = 0;
            }

            if (percent > 100)
            {
                percent = 100;
            }

            return "Wind " + Compass(x, z) + " " + percent.ToString(CultureInfo.InvariantCulture) + "%";
        }

        private static string Compass(float x, float z)
        {
            double angle = Math.Atan2(x, z) * (180.0 / Math.PI);
            if (angle < 0d)
            {
                angle += 360d;
            }

            int index = (int)Math.Floor((angle + 22.5d) / 45d) % 8;
            if (index < 0)
            {
                index += 8;
            }

            string[] names = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            return names[index];
        }

        private static string FormatStatus(ClockSnapshot snapshot, ClockOptions options)
        {
            List<string> parts = new List<string>();
            if (options.ShowCold && snapshot.IsCold)
            {
                parts.Add("Cold");
            }

            if (options.ShowWet && snapshot.IsWet)
            {
                parts.Add("Wet");
            }

            if (options.ShowFreezing && snapshot.IsFreezing)
            {
                parts.Add("Freezing");
            }

            return string.Join(", ", parts);
        }
    }
}
