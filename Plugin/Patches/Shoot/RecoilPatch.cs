﻿using Aki.Reflection.Patching;
using EFT;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System;
using Aki.Reflection.Utils;
using System.Linq;
using SAIN.Helpers;
using SAIN.SAINComponent;
using SAIN.SAINComponent.Classes.Decision;
using SAIN.SAINComponent.Classes.Talk;
using SAIN.SAINComponent.Classes.WeaponFunction;
using SAIN.SAINComponent.Classes.Mover;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.SubComponents;
using SAIN.SAINComponent.Classes.Enemy;

namespace SAIN.Patches.Shoot
{
    public class AimOffsetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            _endTargetPointProp = AccessTools.Property(HelpersGClass.AimDataType, "EndTargetPoint");
            return AccessTools.Method(HelpersGClass.AimDataType, "method_13");
        }

        private static PropertyInfo _endTargetPointProp;

        private static float DebugTimer;

        [PatchPrefix]
        public static bool PatchPrefix(ref BotOwner ___botOwner_0, ref Vector3 ___vector3_5, ref Vector3 ___vector3_4, ref float ___float_13)
        {
            Vector3 badShootOffset = ___vector3_5;
            float aimUpgradeByTime = ___float_13;
            Vector3 aimOffset = ___vector3_4;
            Vector3 recoilOffset = ___botOwner_0.RecoilData.RecoilOffset;
            Vector3 realTargetPoint = ___botOwner_0.AimingData.RealTargetPoint;

            IPlayer person = ___botOwner_0?.Memory?.GoalEnemy?.Person;
            if (SAINEnableClass.GetSAIN(___botOwner_0, out var bot, nameof(AimOffsetPatch)))
            {
                SAINEnemy enemy = bot.EnemyController.CheckAddEnemy(person);
                if (enemy != null)
                {
                    aimUpgradeByTime /= enemy.EnemyAim.AimAndScatterMultiplier;
                }
                float distance = (realTargetPoint - ___botOwner_0.WeaponRoot.position).magnitude;
                float scaled = distance / 20f;
                recoilOffset = bot.Info.WeaponInfo.Recoil.CurrentRecoilOffset * scaled;
            }

            // Applies aiming offset, recoil offset, and scatter offsets
            // Default Setup :: Vector3 finalTarget = __instance.RealTargetPoint + badShootOffset + (AimUpgradeByTime * (AimOffset + ___botOwner_0.RecoilData.RecoilOffset));
            Vector3 finalOffset = badShootOffset + (aimUpgradeByTime * aimOffset) + recoilOffset;

            if (person != null && 
                !person.IsAI &&
                SAINPlugin.LoadedPreset.GlobalSettings.Look.NotLookingToggle)
            {
                finalOffset += NotLookingOffset(person, ___botOwner_0);
            }

            _endTargetPointProp.SetValue(___botOwner_0.AimingData, realTargetPoint + finalOffset);
            return false;
        }

        private static Vector3 NotLookingOffset(IPlayer person, BotOwner botOwner)
        {
            float ExtraSpread = SAINNotLooking.GetSpreadIncrease(person, botOwner);
            if (ExtraSpread > 0)
            {
                Vector3 vectorSpread = UnityEngine.Random.insideUnitSphere * ExtraSpread;
                vectorSpread.y = 0;
                if (SAINPlugin.DebugMode && DebugTimer < Time.time)
                {
                    DebugTimer = Time.time + 10f;
                    Logger.LogDebug($"Increasing Spread because Player isn't looking. Magnitude: [{vectorSpread.magnitude}]");
                }
                return vectorSpread;
            }
            return Vector3.zero;
        }
    }

    public class RecoilPatch : ModulePatch
    {
        private static PropertyInfo _RecoilDataPI;

        protected override MethodBase GetTargetMethod()
        {
            _RecoilDataPI = AccessTools.Property(typeof(BotOwner), "RecoilData");
            return AccessTools.Method(_RecoilDataPI.PropertyType, "Recoil");
        }

        [PatchPrefix]
        public static bool PatchPrefix(BotOwner ____owner)
        {
            return SAINPlugin.IsBotExluded(____owner);
        }
    }

    public class LoseRecoilPatch : ModulePatch
    {
        private static PropertyInfo _RecoilDataPI;

        protected override MethodBase GetTargetMethod()
        {
            _RecoilDataPI = AccessTools.Property(typeof(BotOwner), "RecoilData");
            return AccessTools.Method(_RecoilDataPI.PropertyType, "LosingRecoil");
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref Vector3 ____recoilOffset, BotOwner ____owner)
        {
            if (SAINPlugin.IsBotExluded(____owner))
            {
                return true;
            }
            if (SAINPlugin.BotController == null)
            {
                Logger.LogError($"Bot Controller Null in [{nameof(LoseRecoilPatch)}]");
                return true;
            }
            if (SAINPlugin.BotController.GetSAIN(____owner, out BotComponent sain))
            {
                var recoil = sain?.Info?.WeaponInfo?.Recoil;
                if (recoil != null)
                {
                    ____recoilOffset = recoil.CurrentRecoilOffset;
                    return false;
                }
            }
            return true;
        }
    }

    public class EndRecoilPatch : ModulePatch
    {
        private static PropertyInfo _RecoilDataPI;

        protected override MethodBase GetTargetMethod()
        {
            _RecoilDataPI = AccessTools.Property(typeof(BotOwner), "RecoilData");
            return AccessTools.Method(_RecoilDataPI.PropertyType, "CheckEndRecoil");
        }

        [PatchPrefix]
        public static bool PatchPrefix(BotOwner ____owner)
        {
            if (SAINPlugin.IsBotExluded(____owner))
            {
                return true;
            }
            if (SAINPlugin.BotController == null)
            {
                Logger.LogError($"Bot Controller Null in [{nameof(EndRecoilPatch)}]");
                return true;
            }
            if (SAINPlugin.BotController.GetSAIN(____owner, out BotComponent sain))
            {
                return false;
            }
            return true;
        }
    }
}