﻿using BepInEx.Logging;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent;
using SAIN.SAINComponent.Classes.Enemy;
using SAIN.SAINComponent.Classes.Info;
using System.Collections.Generic;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Decision
{
    public class SquadDecisionClass : SAINBase, ISAINClass
    {
        public SquadDecisionClass(BotComponent sain) : base(sain)
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

        private SAINSquadClass Squad => SAINBot.Squad;

        float SquaDDecision_DontDoSquadDecision_EnemySeenRecentTime = 3f;

        public bool GetDecision(out SquadDecision Decision)
        {
            Decision = SquadDecision.None;
            if (!Squad.BotInGroup || SAINBot.Squad.SquadInfo?.LeaderComponent == null || Squad.LeaderComponent?.IsDead == true)
            {
                return false;
            }

            if (EnemyDecision(out Decision))
            {
                return true;
            }

            //if (shallRegroup())
            //{
            //    Decision = SquadDecision.Regroup;
            //    return true;
            //}

            return false;
        }

        float SquaDecision_RadioCom_MaxDistSq = 1200f;
        float SquadDecision_MyEnemySeenRecentTime = 10f;

        private bool EnemyDecision(out SquadDecision Decision)
        {
            Decision = SquadDecision.None;
            SAINEnemy myEnemy = SAINBot.Enemy;

            if (shallPushSuppressedEnemy(myEnemy))
            {
                Decision = SquadDecision.PushSuppressedEnemy;
                return true;
            }
            if (myEnemy != null)
            {
                if (myEnemy.IsVisible || myEnemy.TimeSinceSeen < SquadDecision_MyEnemySeenRecentTime)
                {
                    return false;
                }
            }
            if (SAINBot.Squad.LeaderComponent != null && 
                shallGroupSearch())
            {
                Decision = SquadDecision.GroupSearch;
                return true;
            }

            foreach (var member in SAINBot.Squad.Members.Values)
            {
                if (member == null || member.BotOwner == BotOwner || member.BotOwner.IsDead)
                {
                    continue;
                }
                if (!HasRadioComms && (SAINBot.Transform.Position - member.Transform.Position).sqrMagnitude > SquaDecision_RadioCom_MaxDistSq)
                {
                    continue;
                }
                if (myEnemy != null 
                    && member.HasEnemy)
                {
                    if (myEnemy.EnemyIPlayer == member.Enemy.EnemyIPlayer)
                    {
                        if (shallSuppressEnemy(member))
                        {
                            Decision = SquadDecision.Suppress;
                            return true;
                        }
                        if (shallHelp(member))
                        {
                            Decision = SquadDecision.Help;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static readonly float PushSuppressedEnemyMaxPathDistance = 75f;
        private static readonly float PushSuppressedEnemyMaxPathDistanceSprint = 100f;
        private static readonly float PushSuppressedEnemyLowAmmoRatio = 0.5f;

        private bool shallPushSuppressedEnemy(SAINEnemy enemy)
        {
            if (enemy != null
                && !SAINBot.Decision.SelfActionDecisions.LowOnAmmo(PushSuppressedEnemyLowAmmoRatio) 
                && SAINBot.Info.PersonalitySettings.Rush.CanRushEnemyReloadHeal)
            {
                bool inRange = false;
                float modifier = enemy.EnemyStatus.VulnerableAction == EEnemyAction.UsingSurgery ? 1.25f : 1f;
                if (enemy.Path.PathDistance < PushSuppressedEnemyMaxPathDistanceSprint * modifier
                    && BotOwner?.CanSprintPlayer == true)
                {
                    inRange = true;
                }
                else if (enemy.Path.PathDistance < PushSuppressedEnemyMaxPathDistance * modifier)
                {
                    inRange = true;
                }

                if (inRange
                    && (SAINBot.Memory.Health.HealthStatus == ETagStatus.Healthy || SAINBot.Memory.Health.HealthStatus == ETagStatus.Injured)
                    && SAINBot.Squad.SquadInfo.SquadIsSuppressEnemy(enemy.EnemyPlayer.ProfileId, out var suppressingMember) 
                    && suppressingMember != SAINBot)
                {
                    var enemyStatus = enemy.EnemyStatus;
                    if (enemy.EnemyStatus.VulnerableAction != EEnemyAction.None)
                    {
                        return true;
                    }
                    ETagStatus enemyHealth = enemy.EnemyPlayer.HealthStatus;
                    if (enemyHealth == ETagStatus.Dying || enemyHealth == ETagStatus.BadlyInjured)
                    {
                        return true;
                    }
                    else if (enemy.EnemyPlayer.IsInPronePose)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasRadioComms => SAINBot.Equipment.HasEarPiece;

        float SquadDecision_SuppressFriendlyDistStart = 30f;
        float SquadDecision_SuppressFriendlyDistEnd = 50f;

        private bool shallSuppressEnemy(BotComponent member)
        {
            if (SAINBot.Enemy?.SuppressionTarget == null)
            {
                return false;
            }
            if (SAINBot.Enemy?.IsVisible == true)
            {
                return false;
            }
            if (member.Decision.CurrentSoloDecision != SoloDecision.Retreat)
            {
                return false;
            }

            float memberDistance = (member.Transform.Position - BotOwner.Position).magnitude;
            float ammo = SAINBot.Decision.SelfActionDecisions.AmmoRatio;

            if (SAINBot.Decision.CurrentSquadDecision == SquadDecision.Suppress)
            {
                return memberDistance <= SquadDecision_SuppressFriendlyDistEnd && ammo >= 0.1f;
            }
            return memberDistance <= SquadDecision_SuppressFriendlyDistStart && ammo >= 0.5f;
        }

        private bool shallGroupSearch(BotComponent member)
        {
            bool squadSearching = member.Decision.CurrentSoloDecision == SoloDecision.Search || member.Decision.CurrentSquadDecision == SquadDecision.Search;
            if (squadSearching)
            {
                return true;
            }
            return false;
        }

        private bool shallGroupSearch()
        {
            foreach (var member in SAINBot.Squad.Members.Values)
            {
                if (member.Decision.CurrentSoloDecision == SoloDecision.Search)
                {
                    if (SAINBot.Enemy != null
                        && doesMemberShareEnemy(member))
                    {
                        return true;
                    }
                    if (SAINBot.Enemy == null
                        && SAINBot.CurrentTargetPosition != null
                        && doesMemberShareTarget(member, SAINBot.CurrentTargetPosition.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool doesMemberShareTarget(BotComponent member, Vector3 targetPosition, float maxDist = 20f)
        {
            if (member == null || member.ProfileId == SAINBot.ProfileId || member.BotOwner?.IsDead == true)
            {
                return false;
            }

            return member.CurrentTargetPosition != null 
                && (member.CurrentTargetPosition.Value - targetPosition).sqrMagnitude < maxDist;
        }
        private bool doesMemberShareEnemy(BotComponent member)
        {
            if (member == null || member.ProfileId == SAINBot.ProfileId || member.BotOwner?.IsDead == true)
            {
                return false;
            }

            return member.Enemy != null
                && member.Enemy.EnemyPlayer.ProfileId == SAINBot.Enemy.EnemyPlayer.ProfileId;
        }

        float SquadDecision_StartHelpFriendDist = 30f;
        float SquadDecision_EndHelpFriendDist = 45f;
        float SquadDecision_EndHelp_FriendsEnemySeenRecentTime = 8f;

        private bool shallHelp(BotComponent member)
        {
            float distance = member.Enemy.Path.PathDistance;
            bool visible = member.Enemy.IsVisible;

            if (SAINBot.Decision.CurrentSquadDecision == SquadDecision.Help 
                && member.Enemy.Seen)
            {
                return distance < SquadDecision_EndHelpFriendDist
                    && member.Enemy.TimeSinceSeen < SquadDecision_EndHelp_FriendsEnemySeenRecentTime;
            }
            return distance < SquadDecision_StartHelpFriendDist && visible;
        }

        float SquadDecision_Regroup_NoEnemy_StartDist = 125f;
        float SquadDecision_Regroup_NoEnemy_EndDistance = 50f;
        float SquadDecision_Regroup_Enemy_StartDist = 50f;
        float SquadDecision_Regroup_Enemy_EndDistance = 15f;
        float SquadDecision_Regroup_EnemySeenRecentTime = 60f;

        public bool shallRegroup()
        {
            var squad = SAINBot.Squad;
            if (squad.IAmLeader)
            {
                return false;
            }

            float maxDist = SquadDecision_Regroup_NoEnemy_StartDist;
            float minDist = SquadDecision_Regroup_NoEnemy_EndDistance;

            var enemy = SAINBot.Enemy;
            if (enemy != null)
            {
                if (enemy.IsVisible || (enemy.Seen && enemy.TimeSinceSeen < SquadDecision_Regroup_EnemySeenRecentTime))
                {
                    return false;
                }
                maxDist = SquadDecision_Regroup_Enemy_StartDist;
                minDist = SquadDecision_Regroup_Enemy_EndDistance;
            }

            var lead = squad.LeaderComponent;
            if (lead != null)
            {
                Vector3 BotPos = BotOwner.Position;
                Vector3 leadPos = lead.Transform.Position;
                Vector3 directionToLead = leadPos - BotPos;
                float leadDistance = directionToLead.magnitude;
                if (enemy != null)
                {
                    Vector3 EnemyPos = enemy.EnemyPosition;
                    Vector3 directionToEnemy = EnemyPos - BotPos;
                    float EnemyDistance = directionToEnemy.magnitude;
                    if (EnemyDistance < leadDistance)
                    {
                        if (EnemyDistance < 30f && Vector3.Dot(directionToEnemy.normalized, directionToLead.normalized) > 0.25f)
                        {
                            return false;
                        }
                    }
                }
                if (SAINBot.Decision.CurrentSquadDecision == SquadDecision.Regroup)
                {
                    return leadDistance > minDist;
                }
                else
                {
                    return leadDistance > maxDist;
                }
            }
            return false;
        }
    }
}