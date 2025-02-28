﻿
using SAIN.Attributes;

namespace SAIN
{
    public enum SoloDecision
    {
        None,
        Retreat,
        Fight,
        RunToCover,
        Search,
        HoldInCover,
        RunAway,
        DogFight,
        Shoot,
        MoveToCover,
        StandAndShoot,
        ThrowGrenade,
        ShiftCover,
        RushEnemy,
        MoveToEngage,
        Ambush,
        ReturnFire,
        ShootDistantEnemy,
        AvoidGrenade,
        Freeze,
        CreepOnEnemy,

        UnstuckSearch,
        UnstuckMoveToCover,
        UnstuckDogFight,
        DebugNoDecision
    }

    public enum SelfDecision
    {
        None = 0,
        Reload = 1,
        RunAway = 2,
        FirstAid = 3,
        RunAwayGrenade = 4,
        Stims = 5,
        Surgery = 6,
    }

    public enum FriendlyFireStatus
    {
        None,
        FriendlyBlock,
        FriendlyClose,
        Clear,
    }

    public enum IWeaponClass
    {
        Default,
        assaultRifle,
        assaultCarbine,
        machinegun,
        smg,
        pistol,
        marksmanRifle,
        sniperRifle,
        shotgun,
        grenadeLauncher,
        specialWeapon
    }

    public enum ICaliber
    {
        Default,
        Caliber9x18PM,
        Caliber9x19PARA,
        Caliber46x30,
        Caliber9x21,
        Caliber57x28,
        Caliber762x25TT,
        Caliber1143x23ACP,
        Caliber9x33R,
        Caliber545x39,
        Caliber556x45NATO,
        Caliber9x39,
        Caliber762x35,
        Caliber762x39,
        Caliber366TKM,
        Caliber762x51,
        Caliber127x55,
        Caliber762x54R,
        Caliber86x70,
        Caliber20g,
        Caliber12g,
        Caliber23x75,
        Caliber26x75,
        Caliber30x29,
        Caliber40x46,
        Caliber40mmRU,
        Caliber127x108,
        Caliber68x51
    }

    public enum CoverStatus
    {
        InCover = 0,
        CloseToCover = 1,
        MidRangeToCover = 2,
        FarFromCover = 3,
        None = 4,
    }

    public enum LeanSetting
    {
        None = 0,
        Left = 1,
        Right = 2,
    }

    public enum SideStepSetting
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    public enum EPersonality
    {
        Wreckless,
        SnappingTurtle,
        GigaChad,
        Chad,
        Rat,
        Timmy,
        Coward,
        Normal,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
        Custom5,
    }

    public enum GrenadeThrowDirection
    {
        None,
        Over,
        Around
    }

    public enum GrenadeThrowType
    {
        None,
        Close,
        Mid,
        Far
    }

    public enum SquadDecision
    {
        None,
        Surround,
        Retreat,
        Suppress,
        PushSuppressedEnemy,
        BoundingRetreat,
        Regroup,
        SpreadOut,
        HoldPositions,
        Help,
        Search,
        GroupSearch,
    }

    public enum SAINSoundType
    {
        None,
        FootStep,
        Prone,
        Looting,
        Reload,
        GearSound,
        GrenadePin,
        GrenadeExplosion,
        GrenadeDraw,
        Jump,
        Door,
        DoorBreach,
        Gunshot,
        SuppressedGunShot,
        Heal,
        Food,
        Conversation,
        Surgery,
        DryFire,
        TurnSound,
        Breathing,
        Pain,
    }

    public enum ELocation
    {
        None = 0,
        Factory = 1,
        FactoryNight = 2,
        Customs = 3,
        GroundZero = 4,
        Reserve = 5,
        Streets = 6,
        Lighthouse = 7,
        Shoreline = 8,
        Labs = 9,
        Woods = 10,
        Interchange = 11,
        Terminal = 12,
        Town = 13,
    }

    public enum EnemyPathDistance
    {
        VeryClose,
        Close,
        Mid,
        Far,
        VeryFar,
        NoEnemy,
    }

    public enum StyleState
    {
        normal,
        onNormal,
        active,
        onActive,
        hover,
        onHover,
        focused,
        onFocused,
    }

    public enum ESoundCleanupReason
    {
        None = 0,
        PlayerNull = 1,
        IPlayerNull = 2,
        TooFar = 3,
        TooOld = 4,
        SoundNull = 5,
        Forced = 6,
    }

    public enum AILimitSetting
    {
        Close = 0,
        Far = 1,
        VeryFar = 2,
        Narnia = 3,
    }
}
