﻿using BepInEx.Logging;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using System.Text;
using SAIN.SAINComponent;
using SAIN.Layers.Combat.Solo.Cover;
using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Layers.Combat.Solo
{
    internal class CombatSoloLayer : SAINLayer
    {
        public CombatSoloLayer(BotOwner bot, int priority) : base(bot, priority, Name)
        {
        }

        public static readonly string Name = BuildLayerName<CombatSoloLayer>();

        public override Action GetNextAction()
        {
            SoloDecision Decision = CurrentDecision;
            var SelfDecision = SAINBot.Decision.CurrentSelfDecision;
            LastActionDecision = Decision;

            if (_doSurgeryAction)
            {
                _doSurgeryAction = false;
                return new Action(typeof(DoSurgeryAction), $"Do Surgery");
            }

            switch (Decision)
            {
                case SoloDecision.MoveToEngage:
                    return new Action(typeof(MoveToEngageAction), $"{Decision}");

                case SoloDecision.RushEnemy:
                    return new Action(typeof(RushEnemyAction), $"{Decision}");

                case SoloDecision.ThrowGrenade:
                    return new Action(typeof(ThrowGrenadeAction), $"{Decision}");

                case SoloDecision.ShiftCover:
                    return new Action(typeof(ShiftCoverAction), $"{Decision}");

                case SoloDecision.RunToCover:
                    if (SAINBot.Cover.CoverPoints.Count > 0)
                    {
                        return new Action(typeof(RunToCoverAction), $"{Decision}");
                    }
                    else
                    {
                        NoCoverUseDogFight = true;
                        return new Action(typeof(DogFightAction), $"{Decision} : No Cover Found Yet! Using Dogfight");
                    }

                case SoloDecision.Retreat:
                    if (SAINBot.Cover.CoverPoints.Count > 0)
                    {
                        return new Action(typeof(RunToCoverAction), $"{Decision} + {SelfDecision}");
                    }
                    else
                    {
                        NoCoverUseDogFight = true;
                        return new Action(typeof(DogFightAction), $"{Decision} : No Cover Found Yet! Using Dogfight");
                    }

                case SoloDecision.MoveToCover:
                case SoloDecision.UnstuckMoveToCover:
                    if (SAINBot.Cover.CoverPoints.Count > 0)
                    {
                        return new Action(typeof(WalkToCoverAction), $"{Decision}");
                    }
                    else
                    {
                        NoCoverUseDogFight = true;
                        return new Action(typeof(DogFightAction), $"{Decision} : No Cover Found Yet! Using Dogfight");
                    }

                case SoloDecision.DogFight:
                case SoloDecision.UnstuckDogFight:
                    return new Action(typeof(DogFightAction), $"{Decision}");

                case SoloDecision.ShootDistantEnemy:
                case SoloDecision.StandAndShoot:
                    return new Action(typeof(StandAndShootAction), $"{Decision}");

                case SoloDecision.HoldInCover:
                    if (SelfDecision != SelfDecision.None)
                    {
                        return new Action(typeof(HoldinCoverAction), $"{Decision} + {SelfDecision}");
                    }
                    else
                    {
                        return new Action(typeof(HoldinCoverAction), $"{Decision}");
                    }

                case SoloDecision.Shoot:
                    return new Action(typeof(ShootAction), $"{Decision}");

                case SoloDecision.Search:
                case SoloDecision.UnstuckSearch:
                    return new Action(typeof(SearchAction), $"{Decision}");

                default:
                    return new Action(typeof(StandAndShootAction), $"DEFAULT! {Decision}");
            }
        }

        bool NoCoverUseDogFight;

        public override bool IsActive()
        {
            bool active = SAINBot != null && CurrentDecision != SoloDecision.None;
            if (SAINBot != null && SAINBot.SAINSoloActive != active)
            {
                SAINBot.SAINSoloActive = active;
            }
            if (SAINBot == null)
            {
                if (_nextLogErrorTime < Time.time)
                {
                    _nextLogErrorTime = Time.time + 10f;
                    Logger.LogError($"SAIN Is null for {BotOwner?.name} : {BotOwner?.Profile?.Info?.Settings?.Role} : {BotOwner?.Profile?.Nickname}");
                }
                return false;
            }
            return active;
        }

        private static float _nextLogErrorTime;

        public override bool IsCurrentActionEnding()
        {
            if (NoCoverUseDogFight && SAINBot.Cover.CoverPoints.Count > 0)
            {
                NoCoverUseDogFight = false;
                return true;
            }

            // this is dumb im sorry
            if (!_doSurgeryAction
                && SAINBot.Decision.CurrentSelfDecision == SelfDecision.Surgery
                && SAINBot.Cover.BotIsAtCoverInUse())
            {
                _doSurgeryAction = true;
                return true;
            }

            return CurrentDecision != LastActionDecision;
        }

        bool _doSurgeryAction;

        private SoloDecision LastActionDecision = SoloDecision.None;
        public SoloDecision CurrentDecision => SAINBot.Decision.CurrentSoloDecision;
    }
}