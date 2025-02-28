﻿using EFT;
using Newtonsoft.Json;
using SAIN.Attributes;
using SAIN.Helpers;
using SAIN.Preset.GlobalSettings.Categories;
using System.Collections.Generic;

namespace SAIN.Preset.GlobalSettings
{
    public class GeneralSettings
    {
        [JsonIgnore]
        [Hidden]
        public float SprintReachDistance = 1f;

        [JsonIgnore]
        [Hidden]
        public float BaseReachDistance = 0.5f;

        [Name("Global Difficulty Modifier")]
        [Description("Higher number = harder bots. Affects bot accuracy, recoil, fire-rate, full auto burst lenght, scatter, reaction-time")]
        [Default(1f)]
        [MinMax(0.1f, 5f, 100f)]
        public float GlobalDifficultyModifier = 1f;

        [Name("Random Speed Hacker AI")]
        [Description("Emulate the real Live-Like experience! 1% of bots will be a speed-hacker.")]
        [Default(false)]
        public bool RandomSpeedHacker = false;

        [Name("Performance Mode")]
        [Description("Limits the cover finder to maximize performance. If your PC is CPU limited, this might let you regain some frames lost while using SAIN. Can cause bots to take too long to find cover to go to.")]
        [Default(false)]
        public bool PerformanceMode = false;

        [Name("Bots Open Doors Fast")]
        [Description("WIP. Can cause bots to get stuck on doors sometimes.")]
        [Default(true)]
        public bool NewDoorOpening = true;

        [Name("Limit SAIN Function in AI vs AI")]
        [Description("Disables certains functions when ai are fighting other ai, and they aren't close to a human player. Turn off if you are spectating ai in free-cam.")]
        [Default(true)]
        public bool LimitAIvsAI = true;

        [Name("Max AI vs AI audio range for Distant Bots")]
        [Description("Bots will not hear gunshots from other bots past this distance (meters) if they are far away (around 250 meters) from the player")]
        [Default(125f)]
        [Advanced]
        public float LimitAIvsAIMaxAudioRange = 125f;

        [Name("Max AI vs AI audio range for Very Distant Bots")]
        [Description("Bots will not hear gunshots from other bots past this distance (meters) if they are VERY far away (around 400 meters) from the player")]
        [Default(80f)]
        [Advanced]
        public float LimitAIvsAIMaxAudioRangeVeryFar = 80f;

        [Name("Bot Grenades")]
        [Default(true)]
        public bool BotsUseGrenades = true;

        [Description("Requires Restart. Dont touch unless you know what this is")]
        [Advanced]
        [Default(24)]
        [MinMax(0, 100)]
        [Hidden]
        [JsonIgnore]
        public int SAINCombatSquadLayerPriority = 22;

        [Description("Requires Restart. Dont touch unless you know what this is")]
        [Advanced]
        [Default(22)]
        [MinMax(0, 100)]
        [Hidden]
        [JsonIgnore]
        public int SAINExtractLayerPriority = 24;

        [Description("Requires Restart. Dont touch unless you know what this is")]
        [Advanced]
        [Default(20)]
        [MinMax(0, 100)]
        [Hidden]
        [JsonIgnore]
        public int SAINCombatSoloLayerPriority = 20;
    }
}