﻿using EFT;
using EFT.InventoryLogic;
using SAIN.Components;
using SAIN.SAINComponent;
using SAIN.SAINComponent.Classes.Decision;
using SAIN.SAINComponent.Classes.Talk;
using SAIN.SAINComponent.Classes.WeaponFunction;
using SAIN.SAINComponent.Classes.Mover;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.SubComponents;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using static EFT.InventoryLogic.Weapon;
using SAIN.SAINComponent.Classes.Info;

namespace SAIN.SAINComponent.Classes.WeaponFunction
{
    public class Firerate : SAINBase, ISAINClass
    {
        public Firerate(BotComponent sain) : base(sain)
        {
        }

        public void Init()
        {
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }

        public float SemiAutoROF()
        {
            WeaponInfoClass weaponInfo = SAINBot.Info.WeaponInfo;
            if (weaponInfo == null)
            {
                return 1f;
            }
            float minTime = 0.1f; // minimum time per shot
            float maxTime = 4f; // maximum time per shot
            float EnemyDistance = (BotOwner.AimingData.RealTargetPoint - BotOwner.WeaponRoot.position).magnitude;

            float rate = EnemyDistance / (PerMeter / weaponInfo.FinalModifier);
            float final = Mathf.Clamp(rate, minTime, maxTime);

            // Sets a different time between shots if a weapon is full auto or burst and the enemy isn't close
            if (weaponInfo.IsFireModeSet(EFireMode.fullauto) || weaponInfo.IsFireModeSet(EFireMode.burst))
            {
                final = Mathf.Clamp(final, 0.1f, 3f);
            }

            final /= SAINBot.Info.FileSettings.Shoot.FireratMulti;

            // Final Result which is randomized +- 15%
            float finalTime = final * Random.Range(0.85f, 1.15f);

            finalTime = Mathf.Round(finalTime * 100f) / 100f;

            //Logger.LogDebug(finalTime);

            return finalTime;
        }

        public float PerMeter
        {
            get
            {
                var perMeterDictionary = GlobalSettings?.Shoot?.WeaponPerMeter;
                var weapInfo = SAINBot?.Info?.WeaponInfo;

                if (perMeterDictionary != null && weapInfo != null)
                {
                    if (perMeterDictionary.TryGetValue(weapInfo.IWeaponClass, out float perMeter))
                    {
                        return perMeter;
                    }
                    if (perMeterDictionary.TryGetValue(IWeaponClass.Default, out perMeter))
                    {
                        return perMeter;
                    }
                }
                return 80f;
            }
        }
    }
}
