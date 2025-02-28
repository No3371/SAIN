﻿using BepInEx.Logging;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.SAINComponent.Classes.Decision;
using SAIN.SAINComponent.Classes.Talk;
using SAIN.SAINComponent.Classes.WeaponFunction;
using SAIN.SAINComponent.Classes.Mover;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.SubComponents;
using SAIN.SAINComponent;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using SAIN.SAINComponent.Classes.Enemy;

namespace SAIN.Layers.Combat.Solo
{
    internal class FlankAction : SAINAction
    {
        public FlankAction(BotOwner bot) : base(bot, nameof(FlankAction))
        {
        }

        public override void Update()
        {
            SAINEnemy enemy = SAINBot.Enemy;
            if (enemy == null)
            {
                return;
            }
        }

        private FlankRoute FindFlankRoute()
        {
            SAINEnemy enemy = SAINBot.Enemy;
            if (enemy == null)
            {
                return null;
            }

            FlankRoute flankRoute = null;

            Vector3 enemyPosition = SAINBot.Enemy.EnemyPosition;
            Vector3 botPosition = SAINBot.Position;

            Vector3? middleNode = FindMiddlePoint(enemy.Path.PathToEnemy, enemy.Path.PathDistance, out int index);

            if (middleNode != null)
            {
                Vector3 directionFromMiddle = enemyPosition - middleNode.Value;

                flankRoute = FindFlank(
                    middleNode.Value, 
                    directionFromMiddle, 
                    botPosition, 
                    enemy, 
                    SideTurn.right);

                if (flankRoute != null)
                {
                    return flankRoute;
                }

                flankRoute = FindFlank(
                    middleNode.Value, 
                    directionFromMiddle, 
                    botPosition, 
                    enemy, 
                    SideTurn.left);

                if (flankRoute != null)
                {
                    return flankRoute;
                }
            }
            return null;
        }

        private FlankRoute FindFlank(Vector3 middleNode, Vector3 directionFromMiddle, Vector3 botPosition, SAINEnemy enemy, SideTurn sideTurn)
        {
            Vector3 flank = Vector.Rotate90(directionFromMiddle, sideTurn);
            if (SamplePointAndCheckPath(flank, middleNode, out NavMeshPath path))
            {
                flank = path.corners[path.corners.Length - 1];
                NavMeshPath pathToEnemy = enemy.Path.PathToEnemy;
                NavMeshPath flankPath = new NavMeshPath();
                if (NavMesh.CalculatePath(botPosition, flank, -1, flankPath)
                    && ArePathsDifferent(pathToEnemy, flankPath))
                {
                    NavMeshPath flankPath2 = new NavMeshPath();
                    if (NavMesh.CalculatePath(flank, enemy.EnemyPosition, -1, flankPath2) 
                        && ArePathsDifferent(pathToEnemy, flankPath2)
                        && ArePathsDifferent(flankPath, flankPath2))
                    {
                        return new FlankRoute
                        {
                            FlankPoint = flank,
                            FirstPath = flankPath,
                            SecondPath = flankPath2,
                        };
                    }
                }
            }
            return null;
        }

        private bool ArePathsDifferent(NavMeshPath path1, NavMeshPath path2, float minRatio = 0.25f)
        {
            int sameCount = 0;
            int differentCount = 0;

            for (int i = 0; i < path1.corners.Length; i++)
            {
                Vector3 node = path1.corners[i];
                bool sameNode = false;
                for (int j = 0; j < path2.corners.Length; j++)
                {
                    Vector3 node2 = path2.corners[j];
                    if (node2 == node)
                    {
                        sameNode = true;
                        break;
                    }
                }
                if (sameNode)
                {
                    sameCount++;
                }
                else
                {
                    differentCount++;
                }
            }
            Logger.NotifyDebug(sameCount / path1.corners.Length);
            return sameCount / path1.corners.Length <= minRatio;
        }

        private bool SamplePointAndCheckPath(Vector3 point, Vector3 origin, out NavMeshPath path)
        {
            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                path = new NavMeshPath();
                return NavMesh.CalculatePath(origin, hit.position, NavMesh.AllAreas, path);
            }
            path = null;
            return false;
        }

        private Vector3? FindMiddlePoint(NavMeshPath path, float pathLength, out int index)
        {
            float currentLength = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 cornerA = path.corners[i];
                Vector3 cornerB = path.corners[i + 1];
                currentLength += (cornerA - cornerB).magnitude;
                if (currentLength >= pathLength / 2)
                {
                    index = i;
                    return new Vector3?(cornerA);
                }
            }
            index = 0;
            return null;
        }

        private FlankRoute _flankRoute;

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}