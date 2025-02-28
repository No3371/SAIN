﻿using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Layers.Combat.Solo;
using SAIN.SAINComponent;
using SAIN.SAINComponent.Classes.Enemy;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using UnityEngine;

namespace SAIN.Layers.Combat.Squad
{
    internal class AssaultAction : SAINAction
    {
        public AssaultAction(BotOwner bot) : base(bot, nameof(AssaultAction))
        {
        }

        public override void Update()
        {
            Shoot.Update();

            SAINEnemy enemy = SAINBot.Enemy;
            if (!SAINBot.Steering.SteerByPriority(false) && enemy != null)
            {
                SAINBot.Steering.LookToEnemy(enemy);
            }

            if (enemy != null)
            {
                if (PointDestination == null)
                {
                    PointDestination = SAINBot.Cover.FindPointInDirection(enemy.EnemyDirection);
                }
                if (PointDestination != null)
                {
                    Vector3 destination = PointDestination.Position;

                    if ((destination - SAINBot.Position).sqrMagnitude < 1f)
                    {
                        PointDestination = null;
                        return;
                    }
                    if (_recalcPathTime < Time.time)
                    {
                        bool sprint = !PointDestination.IsSafePath;

                        if (sprint && BotOwner.BotRun.Run(destination, false, SAINPlugin.LoadedPreset.GlobalSettings.General.SprintReachDistance))
                        {
                            SAINBot.Steering.LookToMovingDirection(500f, true);
                            _recalcPathTime = Time.time + 1f;
                        }
                        else if (SAINBot.Mover.GoToPoint(destination, out _))
                        {
                            _recalcPathTime = Time.time + 1f;
                        }
                        else
                        {
                            _recalcPathTime = Time.time + 0.5f;
                        }
                    }
                }
            }
        }


        private float _recalcPathTime;
        private CoverPoint PointDestination;

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}