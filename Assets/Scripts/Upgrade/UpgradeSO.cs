using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade _legionData", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeSO : ScriptableObject
{
    [field: SerializeField] public UpgradeID ID                   { get; private set; }
    [field: SerializeField] public UpgradeID Requires             { get; private set; }
    [field: SerializeField] public int       EntityID              { get; private set; }
    [field: SerializeField] public Sprite    Icon                    { get; private set; }
    [field: SerializeField] public string    Title                   { get; private set; }
    [field:TextArea(5, 10)]
    [field: SerializeField] public string Description             { get; private set; }
    [field: SerializeField] public int    Cost                    { get; private set; }
    [field: SerializeField] public bool   ForAllUnits             { get; private set; }
    [field: SerializeField] public UpgradeKeyValue[] Properties   { get; private set; }
}

[Serializable]
public class UpgradeKeyValue
{
    [field: SerializeField] public UpgradeType Key  { get; private set; }
    [field: SerializeField] public float Value      { get; private set; }
}

public enum UpgradeType
{
    SpawnInterval,

    IsFlying,
    IsRanged,
    IsStealth,

    HitBoxSize,
    ScanRange,
    MovementSpeed,

    AbilityElement,
    AbilityPower,
    AbilityCriticalChance,
    AbilityCriticalDamage,
    AbilityPenetration,
    AbilityCooldown,
    AbilityCastDuration,
    AbilityDuration,

    AttackElement,
    AttackAltitude,
    AttackDamage,
    AttackCriticalChance,
    AttackCriticalDamage,
    AttackPenetration,
    AttackSpeed,
    AttackRange,
    AttackIsSplash,
    AttackSplash,

    DefenseMaxHealth,
    DefenseArmor,

    FireResistance,
    FrostResistance,
    LightningResistance,
    PoisonResistance,
    HolyResistance,
    ShadowResistance,
    ArcaneResistance,
}

public enum UpgradeID
{
    None,
    Sharpen,
    Sharpen2,
    Sharpen3,
    Headstrong,
    Headstrong2,
    Headstrong3,
    BonePicking,
    LetTheSkullsRoll,
    PaperThin,
}