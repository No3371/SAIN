﻿using Comfort.Common;
using EFT;
using EFT.Game.Spawning;
using SAIN.Components.BotController;
using SAIN.Helpers;
using SAIN.SAINComponent.SubComponents.CoverFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SAIN.Components
{
    public class SAINGameworldComponent : MonoBehaviour
    {
        public static SAINGameworldComponent Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            SAINBotController = this.GetOrAddComponent<SAINBotController>();
            ExtractFinder = this.GetOrAddComponent<Extract.ExtractFinderComponent>();
        }

        private void Update()
        {
            if (SAINMainPlayer == null)
            {
                SAINMainPlayer = ComponentHelpers.AddOrDestroyComponent(SAINMainPlayer, GameWorld?.MainPlayer);
            }

            findLocation();
            findSpawnPointMarkers();
        }

        private void findLocation()
        {
            if (Location == ELocation.None)
            {
                Location = GameWorldHandler.FindLocation();
            }
        }

        public ELocation Location { get; private set; }

        private void OnDestroy()
        {
            try
            {
                ComponentHelpers.DestroyComponent(SAINBotController);
                ComponentHelpers.DestroyComponent(SAINMainPlayer);
            }
            catch (Exception e)
            {
                Logger.LogError("Dispose Component Error: " + e.ToString());
            }
        }

        public Player FindClosestPlayer(out float closestPlayerSqrMag, Vector3 targetPosition)
        {
            var players = GameWorld?.AllAlivePlayersList;

            Player closestPlayer = null;
            closestPlayerSqrMag = float.MaxValue;

            if (players != null)
            {
                foreach (var player in players)
                {
                    if (player != null && player.AIData?.IsAI == false)
                    {
                        float sqrMag = (player.Position - targetPosition).sqrMagnitude;
                        if (sqrMag < closestPlayerSqrMag)
                        {
                            closestPlayer = player;
                            closestPlayerSqrMag = sqrMag;
                        }
                    }
                }
            }
            return closestPlayer;
        }

        private void findSpawnPointMarkers()
        {
            if ((SpawnPointMarkers != null) || (Camera.main == null))
            {
                return;
            }

            SpawnPointMarkers = UnityEngine.Object.FindObjectsOfType<SpawnPointMarker>();

            if (SAINPlugin.DebugMode)
                Logger.LogInfo($"Found {SpawnPointMarkers.Length} spawn point markers");
        }

        public IEnumerable<Vector3> GetAllSpawnPointPositionsOnNavMesh()
        {
            List<Vector3> spawnPointPositions = new List<Vector3>();
            foreach (SpawnPointMarker spawnPointMarker in SpawnPointMarkers)
            {
                // Try to find a point on the NavMesh nearby the spawn point
                Vector3? spawnPointPosition = NavMeshHelpers.GetNearbyNavMeshPoint(spawnPointMarker.Position, 2);
                if (spawnPointPosition.HasValue && !spawnPointPositions.Contains(spawnPointPosition.Value))
                {
                    spawnPointPositions.Add(spawnPointPosition.Value);
                }
            }

            return spawnPointPositions;
        }

        public GameWorld GameWorld => Singleton<GameWorld>.Instance;
        public SAINMainPlayerComponent SAINMainPlayer { get; private set; }
        public SAINBotController SAINBotController { get; private set; }
        public Extract.ExtractFinderComponent ExtractFinder { get; private set; }
        public SpawnPointMarker[] SpawnPointMarkers { get; private set; }
    }

}
