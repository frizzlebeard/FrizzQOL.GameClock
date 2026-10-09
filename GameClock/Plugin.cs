using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace GameClock
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.frizzqol.gameclock";
        public const string PluginName = "FrizzQOL Game Clock";
        public const string PluginVersion = "0.1.1";

        internal static Plugin Instance { get; private set; }

        internal static ConfigEntry<string> ClockFormat;
        internal static ConfigEntry<bool> ShowAmPm;
        internal static ConfigEntry<bool> ShowDay;
        internal static ConfigEntry<bool> ShowWeather;
        internal static ConfigEntry<bool> ShowWind;
        internal static ConfigEntry<bool> ShowCold;
        internal static ConfigEntry<bool> ShowWet;
        internal static ConfigEntry<bool> ShowFreezing;
        internal static ConfigEntry<string> Corner;
        internal static ConfigEntry<float> OffsetX;
        internal static ConfigEntry<float> OffsetY;

        private static string _loggedFormat;
        private static string _loggedCorner;

        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            AcceptableValueList<string> formats = new AcceptableValueList<string>("12h", "24h");
            AcceptableValueList<string> corners = new AcceptableValueList<string>("TopRight", "TopLeft", "TopCenter");
            ClockFormat = Bind("ClockFormat", "12h", "12h or 24h. Example: 6:42 PM or 18:42.", formats);
            ShowAmPm = Bind("ShowAmPm", true, "Adds AM or PM in 12-hour mode. Unused in 24-hour mode.");
            ShowDay = Bind("ShowDay", true, "Shows the day number.");
            ShowWeather = Bind("ShowWeather", true, "Shows the current weather name.");
            ShowWind = Bind("ShowWind", true, "Shows wind direction and strength.");
            ShowCold = Bind("ShowCold", true, "Shows Cold while the world is cold.");
            ShowWet = Bind("ShowWet", true, "Shows Wet while the world is wet.");
            ShowFreezing = Bind("ShowFreezing", true, "Shows Freezing while the world is freezing.");
            Corner = Bind("Corner", "TopRight", "TopRight sits under the small minimap. TopLeft and TopCenter use that corner of the HUD.", corners);
            OffsetX = Bind("OffsetX", 0f, "Pixels. Positive moves the label right.");
            OffsetY = Bind("OffsetY", 0f, "Pixels. Positive moves the label up.");

            _harmony = new Harmony(PluginGuid);
            try
            {
                _harmony.PatchAll();
                Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
            }
            catch (System.Exception ex)
            {
                Logger.LogError("Harmony patch failed: " + ex.Message);
            }
        }

        internal static ClockOptions CurrentOptions()
        {
            string format = ClockFormat.Value;
            if (format != "12h" && format != "24h" && _loggedFormat != format)
            {
                _loggedFormat = format;
                LogWarning("ClockFormat '" + format + "' is not 12h or 24h. Using 12h.");
            }

            string corner = Corner.Value;
            if (ClockText.NormalizeCorner(corner) != corner && _loggedCorner != corner)
            {
                _loggedCorner = corner;
                LogWarning("Corner '" + corner + "' is not TopRight, TopLeft, or TopCenter. Using TopRight.");
            }

            return new ClockOptions
            {
                Use24Hour = ClockText.Use24Hour(format),
                ShowAmPm = ShowAmPm.Value,
                ShowDay = ShowDay.Value,
                ShowWeather = ShowWeather.Value,
                ShowWind = ShowWind.Value,
                ShowCold = ShowCold.Value,
                ShowWet = ShowWet.Value,
                ShowFreezing = ShowFreezing.Value
            };
        }

        internal static void LogWarning(string message)
        {
            if (Instance != null)
            {
                Instance.Logger.LogWarning(message);
            }
        }

        private ConfigEntry<T> Bind<T>(string key, T defaultValue, string description, AcceptableValueBase acceptable = null)
        {
            ConfigEntry<T> entry = acceptable == null
                ? Config.Bind("Display", key, defaultValue, description)
                : Config.Bind("Display", key, defaultValue, new ConfigDescription(description, acceptable));
            entry.SettingChanged += (_, __) => ClockView.RequestRefresh();
            return entry;
        }
    }

    [HarmonyPatch(typeof(Hud), "Awake")]
    internal static class HudAwakePatch
    {
        private static void Postfix(Hud __instance)
        {
            ClockView.Attach(__instance);
        }
    }
}
