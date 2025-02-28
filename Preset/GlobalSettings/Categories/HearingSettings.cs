﻿using Newtonsoft.Json;
using SAIN.Attributes;
using System.Collections.Generic;

namespace SAIN.Preset.GlobalSettings
{
    public class HearingSettings
    {
        [Name("Max Footstep Audio Distance")]
        [Description("The Maximum Range that a bot can hear footsteps, sprinting, and jumping, turning, gear sounds, and any movement related sounds, in meters.")]
        [Default(70f)]
        [MinMax(10f, 150f, 1f)]
        public float MaxFootstepAudioDistance = 70f;

        [Name("Max Footstep Audio Distance without Headphones")]
        [Description("The Maximum Range that a bot can hear footsteps, sprinting, and jumping, turning, gear sounds, and any movement related sounds, in meters when not wearing headphones.")]
        [Default(50f)]
        [MinMax(10f, 150f, 1f)]
        public float MaxFootstepAudioDistanceNoHeadphones = 50f;

        [MinMax(1f, 150f, 100f)]
        [Default(60f)]
        [Advanced]
        public float BaseSoundRange_Looting = 60f;

        [MinMax(1f, 150f, 100f)]
        [Default(30f)]
        [Advanced]
        public float BaseSoundRange_MovementTurnSkid = 30f;

        [MinMax(1f, 150f, 100f)]
        [Default(35f)]
        [Advanced]
        public float BaseSoundRange_GrenadePinDraw = 35f;

        [MinMax(1f, 150f, 100f)]
        [Default(50f)]
        [Advanced]
        public float BaseSoundRange_Prone = 50f;

        [MinMax(1f, 150f, 100f)]
        [Default(40f)]
        [Advanced]
        public float BaseSoundRange_Healing = 40f;

        [MinMax(1f, 150f, 100f)]
        [Default(30f)]
        [Advanced]
        public float BaseSoundRange_Reload = 30f;

        [MinMax(1f, 150f, 100f)]
        [Default(55f)]
        [Advanced]
        public float BaseSoundRange_Surgery = 55f;

        [MinMax(1f, 150f, 100f)]
        [Default(10f)]
        [Advanced]
        public float BaseSoundRange_DryFire = 10f;

        [MinMax(1f, 150f, 100f)]
        [Default(50f)]
        [Advanced]
        public float MaxSoundRange_FallLanding = 70;

        [MinMax(1f, 150f, 100f)]
        [Default(35f)]
        [Advanced]
        public float BaseSoundRange_AimingandGearRattle = 35f;

        [MinMax(1f, 150f, 100f)]
        [Default(40f)]
        [Advanced]
        public float BaseSoundRange_EatDrink = 40f;

        [MinMax(1f, 150f, 100f)]
        [Default(50f)]
        public float MaxRangeToReportEnemyActionNoHeadset = 50f;

        [Name("Hearing Delay / Reaction Time with Active Enemy")]
        [MinMax(0.0f, 1f, 100f)]
        [Default(0.2f)]
        public float BaseHearingDelayWithEnemy = 0.2f;

        [Name("Hearing Delay / Reaction Time while At Peace")]
        [MinMax(0.0f, 1f, 100f)]
        [Default(0.35f)]
        public float BaseHearingDelayAtPeace = 0.35f;

        [Name("Global Gunshot Audible Range Multiplier")]
        [Default(1f)]
        [MinMax(0.1f, 2f, 100f)]
        public float GunshotAudioMultiplier = 1f;

        [Name("Global Footstep Audible Range Multiplier")]
        [Default(1f)]
        [MinMax(0.1f, 2f, 100f)]
        public float FootstepAudioMultiplier = 1f;

        [Name("Suppressed Sound Modifier")]
        [Description("Audible Gun Range is multiplied by this number when using a suppressor")]
        [Default(0.6f)]
        [MinMax(0.1f, 0.95f, 100f)]
        public float SuppressorModifier = 0.6f;

        [Name("Subsonic Sound Modifier")]
        [Description("Audible Gun Range is multiplied by this number when using a suppressor and subsonic ammo")]
        [Default(0.33f)]
        [MinMax(0.1f, 0.95f, 100f)]
        public float SubsonicModifier = 0.33f;

        [Name("Hearing Distances by Ammo Type")]
        [Description("How far a bot can hear a gunshot when fired from each specific caliber listed here.")]
        [MinMax(30f, 400f, 10f)]
        [Advanced]
        [DefaultDictionary(nameof(HearingDistancesDefaults))]
        public Dictionary<ICaliber, float> HearingDistances = new Dictionary<ICaliber, float>(HearingDistancesDefaults);

        [JsonIgnore]
        [Hidden]
        public static readonly Dictionary<ICaliber, float> HearingDistancesDefaults = new Dictionary<ICaliber, float>()
        {
            { ICaliber.Caliber9x18PM, 110f },
            { ICaliber.Caliber9x19PARA, 110f },
            { ICaliber.Caliber46x30, 120f },
            { ICaliber.Caliber9x21, 120f },
            { ICaliber.Caliber57x28, 120f },
            { ICaliber.Caliber762x25TT, 120f },
            { ICaliber.Caliber1143x23ACP, 115f },
            { ICaliber.Caliber9x33R, 125 },
            { ICaliber.Caliber545x39, 160 },
            { ICaliber.Caliber556x45NATO, 160 },
            { ICaliber.Caliber9x39, 160 },
            { ICaliber.Caliber762x35, 175 },
            { ICaliber.Caliber762x39, 175 },
            { ICaliber.Caliber366TKM, 175 },
            { ICaliber.Caliber762x51, 200f },
            { ICaliber.Caliber127x55, 200f },
            { ICaliber.Caliber762x54R, 225f },
            { ICaliber.Caliber86x70, 250f },
            { ICaliber.Caliber20g, 185 },
            { ICaliber.Caliber12g, 185 },
            { ICaliber.Caliber23x75, 210 },
            { ICaliber.Caliber26x75, 50 },
            { ICaliber.Caliber30x29, 50 },
            { ICaliber.Caliber40x46, 50 },
            { ICaliber.Caliber40mmRU, 50 },
            { ICaliber.Caliber127x108, 300 },
            { ICaliber.Caliber68x51, 200f },
            { ICaliber.Default, 125 },
        };
    }
}