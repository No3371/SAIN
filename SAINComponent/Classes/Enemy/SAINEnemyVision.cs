﻿using EFT;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Enemy
{
    public class SAINEnemyVision : EnemyBase
    {
        public SAINEnemyVision(SAINEnemy enemy) : base(enemy)
        {
            GainSight = new GainSightClass(enemy);
            VisionDist = new EnemyVisionDistanceClass(enemy);
        }

        public void Update(bool isCurrentEnemy)
        {
            UpdateVisible(false);
            UpdateCanShoot(false);
        }

        public float EnemyVelocity
        {
            get
            {
                float rawVelocity = EnemyPlayer.Velocity.magnitude;
                if (rawVelocity <= 0.01f)
                {
                    return 0f;
                }
                else
                {
                    const float max = 5f;
                    rawVelocity = Mathf.Clamp(rawVelocity, 0f, max);
                    return rawVelocity / max;
                }
            }
        }

        public bool FirstContactOccured { get; private set; }

        public bool ShallReportRepeatContact { get; set; }

        public bool ShallReportLostVisual { get; set; }

        private const float _repeatContactMinSeenTime = 12f;

        private const float _lostContactMinSeenTime = 12f;

        private float _realLostVisionTime;

        private bool isEnemyInVisibleSector()
        {
            return AngleToEnemy <= MaxVisionAngle;
        }

        private float getAngleToEnemy(bool setYto0)
        {
            Vector3 direction = EnemyPosition - Enemy.SAINBot.Position;
            Vector3 lookDir = BotOwner.LookDirection;
            if (setYto0)
            {
                direction.y = 0;
                lookDir.y = 0;
            }
            return Vector3.Angle(direction, lookDir);
        }

        public float MaxVisionAngle => Enemy.SAINBot.Info.FileSettings.Core.VisibleAngle / 2f;

        public float AngleToEnemy
        {
            get
            {
                getAngles();
                return _angleToEnemy;
            }
        }

        public float AngleToEnemyHorizontal
        {
            get
            {
                getAngles();
                return _angleToEnemyHoriz;
            }
        }

        private void getAngles()
        {
            if (_calcAngleTime < Time.time)
            {
                _calcAngleTime = Time.time + _calcAngleFreq;
                _angleToEnemy = getAngleToEnemy(false);
                _angleToEnemyHoriz = getAngleToEnemy(true);
            }
        }

        private float _angleToEnemyHoriz;
        private float _angleToEnemy;
        private float _calcAngleTime;
        private float _calcAngleFreq = 0.1f;

        public void UpdateVisible(bool forceOff)
        {
            bool wasVisible = IsVisible;
            bool lineOfSight = InLineOfSight || SAIN.Memory.VisiblePlayers.Contains(EnemyPlayer);

            if (forceOff)
            {
                IsVisible = false;
            }
            else
            {
                IsVisible =
                    EnemyInfo?.IsVisible == true &&
                    lineOfSight &&
                    isEnemyInVisibleSector();
            }

            if (IsVisible)
            {
                if (!wasVisible)
                {
                    VisibleStartTime = Time.time;
                    if (Seen && TimeSinceSeen >= _repeatContactMinSeenTime)
                    {
                        ShallReportRepeatContact = true;
                    }
                }
                if (!Seen)
                {
                    FirstContactOccured = true;
                    TimeFirstSeen = Time.time;
                    Seen = true;
                }
                _realLostVisionTime = Time.time;
                TimeLastSeen = Time.time;
                LastSeenPosition = EnemyPerson.Position;
            }

            if (Time.time - _realLostVisionTime < 1f)
            {
                Enemy.UpdateSeenPosition(EnemyPerson.Position);
            }

            if (!IsVisible)
            {
                if (Seen
                    && TimeSinceSeen > _lostContactMinSeenTime
                    && _nextReportLostVisualTime < Time.time)
                {
                    _nextReportLostVisualTime = Time.time + 20f;
                    ShallReportLostVisual = true;
                }
                VisibleStartTime = -1f;
            }

            if (IsVisible != wasVisible)
            {
                LastChangeVisionTime = Time.time;
            }
        }

        private float _nextReportLostVisualTime;

        public void UpdateCanShoot(bool forceOff)
        {
            if (forceOff)
            {
                CanShoot = false;
                return;
            }
            CanShoot = EnemyInfo?.CanShoot == true;
        }

        public bool InLineOfSight { get; set; }
        public bool IsVisible { get; private set; }
        public bool CanShoot { get; private set; }
        public Vector3? LastSeenPosition { get; set; }
        public float VisibleStartTime { get; private set; }
        public float TimeSinceSeen => Seen ? Time.time - TimeLastSeen : -1f;
        public bool Seen { get; private set; }
        public float TimeFirstSeen { get; private set; }
        public float TimeLastSeen { get; private set; }
        public float LastChangeVisionTime { get; private set; }
        public float GainSightCoef => GainSight.GainSightCoef;
        public float VisionDistance => VisionDist.VisionDistance;

        private readonly GainSightClass GainSight;
        private readonly EnemyVisionDistanceClass VisionDist;
    }
}