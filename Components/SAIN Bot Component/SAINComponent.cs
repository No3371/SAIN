﻿using BepInEx.Logging;
using Comfort.Common;
using EFT;
using SAIN.Classes;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components
{
    public class SAINComponent : MonoBehaviour
    {
        private void Awake()
        {
            BotOwner = GetComponent<BotOwner>();
            Init(BotOwner);
        }

        private void Init(BotOwner bot)
        {
            BotColor = RandomColor;

            Info = new BotInfoClass(bot);

            Enemies = bot.GetOrAddComponent<EnemiesComponent>();
            HearingSensor = bot.GetOrAddComponent<AudioComponent>();

            BotSquad = new SquadClass(bot);
            Talk = bot.GetOrAddComponent<BotTalkComponent>();

            Lean = bot.GetOrAddComponent<LeanComponent>();
            Decisions = bot.GetOrAddComponent<DecisionComponent>();
            Cover = bot.GetOrAddComponent<CoverComponent>();
            FlashLight = bot.GetPlayer.gameObject.AddComponent<FlashLightComponent>();

            BotStatus = new StatusClass(bot);
            SelfActions = new SelfActionClass(bot);
            Movement = new MovementClass(bot);
            Dodge = new DodgeClass(bot);
            Steering = new SteeringClass(bot);
            Grenade = new BotGrenadeClass(bot);
            DebugDrawList = new DebugGizmos.DrawLists(BotColor, BotColor);

            Logger = BepInEx.Logging.Logger.CreateLogSource(GetType().Name);
        }

        private const float CheckSelfFreq = 0.1f;
        private const float CheckMoveFreq = 0.2f;

        private void Update()
        {
            if (BotActive && !GameIsEnding)
            {
                if (CheckMoveTimer < Time.time)
                {
                    CheckMoveTimer = Time.time + CheckMoveFreq;
                    BotIsMoving = Vector3.Distance(LastPos, BotOwner.Position) > 0.05f;
                    LastPos = BotOwner.Position;
                }

                if (SelfCheckTimer < Time.time)
                {
                    SelfCheckTimer = Time.time + CheckSelfFreq;
                    SelfActions.Activate();
                    BotOwner.WeaponManager.UpdateWeaponsList();
                }

                BotSquad.ManualUpdate();

                Info.ManualUpdate();
            }
        }

        private float ShiftTimer = 0f;

        public bool ShiftAwayFromCloseWall(Vector3 target)
        {
            if (ShiftTimer < Time.time && CheckTooCloseToWall(target))
            {
                ShiftTimer = Time.time + 0.5f;

                var direction = (BotOwner.Position - target).normalized;
                direction.y = 0f;
                var movePoint = BotOwner.Position + direction;
                if (NavMesh.SamplePosition(movePoint, out var hit, 0.1f, -1))
                {
                    BotOwner.GoToPoint(hit.position, true, -1, false, false);
                    return true;
                }
            }
            return false;
        }

        public bool CheckTooCloseToWall(Vector3 target)
        {
            target.y = BotOwner.Position.y;
            var direction = target - BotOwner.Position;
            return Physics.Raycast(BotOwner.Position, direction, 0.5f, LayerMaskClass.HighPolyWithTerrainMask);
        }

        public bool GoToPointRetreat(Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(BotOwner.Position, point, -1, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                var corners = path.corners;

                if (HasGoalEnemy)
                {
                    int max = corners.Length - 2;
                    int i = 0;
                    while (i < max)
                    {
                        Vector3 directionEnemy = GoalEnemyPos.Value - corners[i];
                        Vector3 directionCorner = corners[i + 1] - corners[i];
                        float dotProduct = Vector3.Dot(directionEnemy, directionCorner);
                        if (dotProduct > 0.85f || (corners[i] - GoalEnemyPos.Value).magnitude < 1f)
                        {
                            return false;
                        }
                        i++;
                    }
                }

                BotOwner.GoToByWay(corners);
                return true;
            }
            return false;
        }

        public bool IsPointInVisibleSector(Vector3 position)
        {
            Vector3 direction = position - BotOwner.Position;

            float cos;

            if (BotOwner.NightVision.UsingNow)
            {
                cos = BotOwner.LookSensor.VISIBLE_ANGLE_NIGHTVISION;
            }
            else
            {
                cos = (BotOwner.BotLight.IsEnable ? BotOwner.LookSensor.VISIBLE_ANGLE_LIGHT : 180f);
            }

            return VectorHelpers.IsAngLessNormalized(BotOwner.LookDirection, VectorHelpers.NormalizeFastSelf(direction), cos);
        }

        public void Dispose()
        {
            StopAllCoroutines();

            Talk.Dispose();
            HearingSensor.Dispose();
            Lean.Dispose();
            Cover.Dispose();
            Decisions.Dispose();
            Enemies.Dispose();
            FlashLight.Dispose();

            Destroy(this);
        }

        public SAINLogicDecision CurrentDecision => Decisions.CurrentDecision;

        public Vector3 HeadPosition => BotOwner.LookSensor._headPoint;

        public Vector3? CurrentTargetPosition => HasGoalEnemy ? GoalEnemyPos : GoalTargetPos;

        public bool HasAnyTarget => HasGoalEnemy || HasGoalTarget;
        public bool HasGoalTarget => BotOwner.Memory.GoalTarget?.GoalTarget != null;
        public bool HasGoalEnemy => BotOwner.Memory.GoalEnemy != null;
        public Vector3? GoalTargetPos => BotOwner.Memory.GoalTarget?.GoalTarget?.Position;
        public Vector3? GoalEnemyPos => BotOwner.Memory.GoalEnemy?.CurrPosition;

        public Vector3 MidPoint(Vector3 target, float lerpVal = 0.5f)
        {
            return Vector3.Lerp(BotOwner.Position, target, lerpVal);
        }

        public bool BotHasStamina => BotOwner.GetPlayer.Physical.Stamina.NormalValue > 0f;

        public Vector3 UnderFireFromPosition { get; set; }

        public Vector3 LastSoundHeardPosition { get; set; }

        public float LastSoundHeardTime { get; set; }

        public bool BotIsMoving { get; private set; }

        public bool HasEnemyAndCanShoot => BotOwner.Memory.GoalEnemy?.CanShoot == true && BotOwner.Memory.GoalEnemy?.IsVisible == true;

        public FlashLightComponent FlashLight { get; private set; }

        public AudioComponent HearingSensor { get; private set; }

        public BotTalkComponent Talk { get; private set; }

        public DecisionComponent Decisions { get; private set; }

        public CoverComponent Cover { get; private set; }

        public EnemiesComponent Enemies { get; private set; }

        public BotInfoClass Info { get; private set; }

        public SquadClass BotSquad { get; private set; }

        public StatusClass BotStatus { get; private set; }

        public SelfActionClass SelfActions { get; private set; }

        public LeanComponent Lean { get; private set; }

        public BotGrenadeClass Grenade { get; private set; }

        public MovementClass Movement { get; private set; }

        public DodgeClass Dodge { get; private set; }

        public SteeringClass Steering { get; private set; }

        public DebugGizmos.DrawLists DebugDrawList { get; private set; }

        public bool BotActive => BotOwner.BotState == EBotState.Active && !BotOwner.IsDead;

        public bool GameIsEnding
        {
            get
            {
                var game = Singleton<IBotGame>.Instance;
                if (game == null)
                {
                    return false;
                }

                return game.Status == GameStatus.Stopping;
            }
        }

        public Color BotColor { get; private set; }

        public BotOwner BotOwner { get; private set; }

        public static LayerMask SightMask => LayerMaskClass.HighPolyWithTerrainMaskAI;

        public static LayerMask ShootMask => LayerMaskClass.HighPolyWithTerrainMask;

        public static LayerMask CoverMask => LayerMaskClass.HighPolyWithTerrainMask;

        public static LayerMask FoliageMask => LayerMaskClass.AI;

        private static Color RandomColor => new Color(Random.value, Random.value, Random.value);

        private float CheckMoveTimer = 0f;

        private Vector3 LastPos = Vector3.zero;

        private float SelfCheckTimer = 0f;

        private ManualLogSource Logger;
    }
}