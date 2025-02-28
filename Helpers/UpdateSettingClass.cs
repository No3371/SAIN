﻿using EFT;
using SAIN.Preset.BotSettings.SAINSettings;
using SAIN.Preset.GlobalSettings;
using System.Text;
using UnityEngine;

namespace SAIN.Helpers
{
    internal class UpdateSettingClass
    {
        public static readonly string[] AimMultiplierNames =
        {
                nameof(BotSettingsComponents.Aiming.BASE_SHIEF),
                nameof(BotSettingsComponents.Aiming.BOTTOM_COEF)
        };

        public static readonly string[] ScatterMultiplierNames =
        {
                nameof(BotSettingsComponents.Aiming.XZ_COEF),
                nameof(BotSettingsComponents.Aiming.XZ_COEF_STATIONARY_BULLET),
                nameof(BotSettingsComponents.Aiming.XZ_COEF_STATIONARY_GRENADE),
                nameof(BotSettingsComponents.Scattering.MinScatter),
                nameof(BotSettingsComponents.Scattering.MaxScatter),
                nameof(BotSettingsComponents.Scattering.WorkingScatter)
        };

        public static string VisibleDistance = nameof(BotSettingsComponents.Core.VisibleDistance);
        public static string GainSightCoef = nameof(BotSettingsComponents.Core.GainSightCoef);

        private static GlobalSettingsClass GlobalSettings => SAINPlugin.LoadedPreset.GlobalSettings;

        public static void ManualSettingsUpdate(WildSpawnType WildSpawnType, BotDifficulty botDifficulty, BotSettingsComponents eftSettings, BotSettingsComponents defaultSettings = null, SAINSettingsClass sainSettings = null)
        {
            if (sainSettings == null)
            {
                sainSettings = SAINPlugin.LoadedPreset.BotSettings.GetSAINSettings(WildSpawnType, botDifficulty);
            }
            if (defaultSettings == null)
            {
                defaultSettings = HelpersGClass.GetEFTSettings(WildSpawnType, botDifficulty);
            }

            float multiplier = ScatterMulti(sainSettings);

            StringBuilder debugString = new StringBuilder();
            if (SAINPlugin.DebugMode)
            {
                debugString.AppendLine($"Applied Multipliers for [{WildSpawnType}, {botDifficulty}]");
            }

            eftSettings.Aiming.BASE_SHIEF = MultiplySetting(
                defaultSettings.Aiming.BASE_SHIEF,
                multiplier,
                "BASE_SHIEF",
                debugString);

            eftSettings.Aiming.BOTTOM_COEF = MultiplySetting(
                defaultSettings.Aiming.BOTTOM_COEF,
                multiplier,
                "BOTTOM_COEF",
                debugString);

            multiplier = AimMulti(sainSettings);

            eftSettings.Aiming.XZ_COEF = MultiplySetting(
                defaultSettings.Aiming.XZ_COEF,
                multiplier,
                "XZ_COEF",
                debugString);

            eftSettings.Aiming.XZ_COEF_STATIONARY_BULLET = MultiplySetting(
                defaultSettings.Aiming.XZ_COEF_STATIONARY_BULLET,
                multiplier,
                "XZ_COEF_STATIONARY_BULLET",
                debugString);

            eftSettings.Aiming.XZ_COEF_STATIONARY_GRENADE = MultiplySetting(
                defaultSettings.Aiming.XZ_COEF_STATIONARY_GRENADE,
                multiplier,
                "XZ_COEF_STATIONARY_GRENADE",
                debugString);

            eftSettings.Scattering.MinScatter = MultiplySetting(
                defaultSettings.Scattering.MinScatter,
                multiplier,
                "MinScatter",
                debugString);

            eftSettings.Scattering.MaxScatter = MultiplySetting(
                defaultSettings.Scattering.MaxScatter,
                multiplier,
                "MaxScatter",
                debugString);

            eftSettings.Scattering.WorkingScatter = MultiplySetting(
                defaultSettings.Scattering.WorkingScatter,
                multiplier,
                "WorkingScatter",
                debugString);

            eftSettings.Core.VisibleDistance = MultiplySetting(
                sainSettings.Core.VisibleDistance,
                VisionDistanceMulti,
                "VisibleDistance",
                debugString);

            eftSettings.Core.GainSightCoef = MultiplySetting(
                sainSettings.Core.GainSightCoef,
                VisionSpeedMulti(sainSettings),
                "GainSightCoef",
                debugString);

            if (SAINPlugin.DebugMode)
            {
                Logger.LogDebug(debugString);
                //Logger.NotifyDebug(debugString, EFT.Communications.ENotificationDurationType.Long);
            }
        }

        public static void ManualSettingsUpdate(WildSpawnType WildSpawnType, BotDifficulty botDifficulty, BotOwner BotOwner, SAINSettingsClass sainSettings)
        {
            var eftSettings = BotOwner.Settings.FileSettings;

            ManualSettingsUpdate(WildSpawnType, botDifficulty, eftSettings, null, sainSettings);

            if (BotOwner.WeaponManager?.WeaponAIPreset != null)
            {
                BotOwner.WeaponManager.WeaponAIPreset.XZ_COEF = eftSettings.Aiming.XZ_COEF;
                BotOwner.WeaponManager.WeaponAIPreset.BaseShift = eftSettings.Aiming.BASE_SHIEF;
            }
        }

        private static float MultiplySetting(float defaultValue, float multiplier, string name, StringBuilder debugString)
        {
            float result = Mathf.Round(defaultValue * multiplier * 100f) / 100f;
            if (SAINPlugin.DebugMode)
            {
                debugString.AppendLabeledValue($"Multiplied [{name}]", $"Default Value: [{defaultValue}] Multiplier: [{multiplier}] Result: [{result}]", Color.white, Color.white);
            }
            return result;
        }

        public static float AimMulti(SAINSettingsClass SAINSettings) 
            => Round(SAINSettings.Aiming.AccuracySpreadMulti * GlobalSettings.Aiming.AccuracySpreadMultiGlobal / GlobalSettings.General.GlobalDifficultyModifier);

        public static float ScatterMulti(SAINSettingsClass SAINSettings) 
            => Round(SAINSettings.Scattering.ScatterMultiplier * GlobalSettings.Shoot.GlobalScatterMultiplier / GlobalSettings.General.GlobalDifficultyModifier);

        public static float VisionSpeedMulti(SAINSettingsClass SAINSettings) 
            => Round(SAINSettings.Look.VisionSpeedModifier * GlobalSettings.Look.GlobalVisionSpeedModifier / GlobalSettings.General.GlobalDifficultyModifier);

        public static float VisionDistanceMulti => GlobalSettings.Look.GlobalVisionDistanceMultiplier;

        private static float Round(float value)
        {
            return Mathf.Round(value * 100f) / 100f;
        }
    }
}