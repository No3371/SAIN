﻿using EFT;
using System.Collections.Generic;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Enemy
{
    public class EnemyPlace
    {
        public EnemyPlace(string ownerID, Vector3 position, float expireTime, bool isDanger, IPlayer player)
        {
            OwnerID = ownerID;
            Position = position;
            ExpireTime = expireTime;
            IsDanger = isDanger;
            Player = player;
        }

        public readonly string OwnerID;
        public readonly IPlayer Player;
        public readonly bool IsDanger;

        public bool ShallClear
        {
            get
            {
                if (TimeSincePositionUpdated > ExpireTime)
                {
                    return true;
                }
                if (HasArrivedPersonal
                    && HasSeenPersonal
                    && Time.time - TimeArrived > 2f
                    && Time.time - TimeSeen > 2f)
                {
                    return true;
                }
                if (HasArrivedSquad
                    && HasSeenSquad
                    && Time.time - TimeSquadArrived > 2f
                    && Time.time - TimeSquadSeen > 2f)
                {
                    return true;
                }
                if (Player?.HealthController?.IsAlive == false)
                {
                    return true;
                }
                if (playerLeftArea)
                {
                    return true;
                }
                return false;
            }
        }

        private bool playerLeftArea
        {
            get
            {
                if (_nextCheckLeaveTime < Time.time)
                {
                    _nextCheckLeaveTime = Time.time + 5f;
                    // If the person this place was created for is AI and left the area, just forget it and move on.
                    if (Player != null)
                    {
                        float sqrMag = (Player.Position - Position).sqrMagnitude;
                        if (Player?.IsAI == true && sqrMag > 75f * 75f)
                        {
                            return true;
                        }
                        if (Player?.IsAI == false && sqrMag > 125f * 125f)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private float _nextCheckLeaveTime;

        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                TimePositionUpdated = Time.time;
                _position = value;
            }
        }

        private Vector3 _position;

        public float TimePositionUpdated;
        public float TimeSincePositionUpdated => Time.time - TimePositionUpdated;

        public float Distance(Vector3 toPoint)
        {
            return (_position - toPoint).magnitude;
        }

        public float DistanceSqr(Vector3 toPoint)
        {
            return (_position - toPoint).sqrMagnitude;
        }


        public bool HasArrivedPersonal
        {
            get
            {
                return _hasArrived;
            }
            set
            {
                if (value)
                {
                    TimeArrived = Time.time;
                    HasSeenPersonal = true;
                }
                _hasArrived = value;
            }
        }

        public readonly float ExpireTime;

        private bool _hasArrived;

        public float TimeArrived;

        public bool HasArrivedSquad
        {
            get
            {
                return _hasArrivedSquad;
            }
            set
            {
                if (value)
                {
                    TimeSquadArrived = Time.time;
                }
                _hasArrivedSquad = value;
            }
        }

        private bool _hasArrivedSquad;

        public float TimeSquadArrived;

        public bool HasSeenPersonal
        {
            get
            {
                return _hasSeen;
            }
            set
            {
                if (value)
                {
                    TimeSeen = Time.time;
                }
                _hasSeen = value;
            }
        }

        private bool _hasSeen;

        public float TimeSeen;

        public bool HasSeenSquad
        {
            get
            {
                return _hasSquadSeen;
            }
            set
            {
                if (value)
                {
                    TimeSquadSeen = Time.time;
                }
                _hasSquadSeen = value;
            }
        }

        private bool _hasSquadSeen;

        public float TimeSquadSeen;

        public bool PersonalClearLineOfSight(Vector3 origin, LayerMask mask)
        {
            if (_nextCheckSightTime < Time.time)
            {
                _nextCheckSightTime = Time.time + 0.33f;
                Vector3 pos = Position + Vector3.up;
                Vector3 direction = pos - origin;
                _inSightNow = !Physics.Raycast(pos, direction, direction.magnitude, mask);
                if (!HasSeenPersonal && _inSightNow)
                {
                    HasSeenPersonal = true;
                }
            }
            return _inSightNow;
        }

        public bool SquadClearLineOfSight(Vector3 origin, LayerMask mask)
        {
            if (_nextCheckSquadSightTime < Time.time)
            {
                _nextCheckSquadSightTime = Time.time + 0.5f;
                Vector3 pos = Position + Vector3.up;
                Vector3 direction = pos - origin;
                _inSightSquadNow = !Physics.Raycast(pos, direction, direction.magnitude, mask);
                if (!HasSeenSquad && _inSightSquadNow)
                {
                    HasSeenSquad = true;
                }
            }
            return _inSightSquadNow;
        }

        private bool _inSightNow;
        private bool _inSightSquadNow;
        private float _nextCheckSightTime;
        private float _nextCheckSquadSightTime;
    }
}