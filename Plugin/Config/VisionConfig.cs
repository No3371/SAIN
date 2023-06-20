﻿using BepInEx.Configuration;

namespace SAIN.UserSettings
{
    internal class VisionConfig
    {
        public static ConfigEntry<bool> EnableVisionJobs { get; private set; }
        public static ConfigEntry<int> CheckFrameCount { get; private set; }
        public static ConfigEntry<float> AbsoluteMaxVisionDistance { get; private set; }
        public static ConfigEntry<bool> NoGlobalFog { get; private set; }
        public static ConfigEntry<bool> DebugWeather { get; private set; }
        public static ConfigEntry<bool> DebugVision { get; private set; }

        public static void Init(ConfigFile Config)
        {
            string debugmode = "Vision Settings";

            EnableVisionJobs = Config.Bind(debugmode, "Enable Vision Job System", true,
                new ConfigDescription("",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

            CheckFrameCount = Config.Bind(debugmode, "CheckFrameCount", 1,
                new ConfigDescription("",
                new AcceptableValueRange<int>(1, 30),
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

            NoGlobalFog = Config.Bind(debugmode, "Is Global Fog Disabled?", false,
                new ConfigDescription("This does not disable global fog. Only enable this if you have global fog disabled through mods like AmandsGraphics. This Allows AI to see much further to reflect the lack of any global fog, and scales weather modifier accordingly.",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

            AbsoluteMaxVisionDistance = Config.Bind(debugmode, "Maximum Vision Distance with No Fog", 300f,
                new ConfigDescription("The furthest possible distance a bot can see if all weather conditions are perfect and it is day-time. Value is capped at 180 if global fog is not disabled",
                new AcceptableValueRange<float>(100f, 800.0f),
                new ConfigurationManagerAttributes { IsAdvanced = true, Order = 2 }));

            DebugWeather = Config.Bind(debugmode, "Debug Weather", false,
                new ConfigDescription("",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = true, Order = 1 }));

            DebugVision = Config.Bind(debugmode, "Debug Vision", false,
                new ConfigDescription("",
                null,
                new ConfigurationManagerAttributes { IsAdvanced = true, Order = 1 }));
        }
    }
}