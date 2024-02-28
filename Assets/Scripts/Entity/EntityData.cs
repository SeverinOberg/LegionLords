using System;
using UnityEngine;
using Unity.Netcode;

public class Stats
{
    public General General;
    public Defense Defense;
    public Attack  Attack;
    public Ability Ability;
}

[Serializable]
public struct General : INetworkSerializable
{
    public int ID;
    public UnitType UnitType;
    public RoleType Role;
    public Faction  Faction;
    public string   Name;
    public byte     HitBoxSize;
    public byte     ScanRange;
    [Tooltip("The higher the faster the unit moves")]
    public float    MovementSpeed;
    
    public General(int id, UnitType unitType, RoleType role, Faction faction, string name, byte hitBoxSize, byte scanRange, float movementSpeed)
    {
        ID            = id;
        UnitType      = unitType;
        Role          = role;
        Faction       = faction;
        Name          = name;
        HitBoxSize    = hitBoxSize;
        ScanRange     = scanRange;
        MovementSpeed = movementSpeed;
    }

    public General(General data)
    {
        ID            = data.ID;
        UnitType      = data.UnitType;
        Role          = data.Role;
        Faction       = data.Faction;
        Name          = data.Name;
        HitBoxSize    = data.HitBoxSize;
        ScanRange     = data.ScanRange;
        MovementSpeed = data.MovementSpeed;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ID);
            reader.ReadValueSafe(out UnitType);
            reader.ReadValueSafe(out Role);
            reader.ReadValueSafe(out Faction);
            reader.ReadValueSafe(out Name);
            reader.ReadValueSafe(out HitBoxSize);
            reader.ReadValueSafe(out ScanRange);
            reader.ReadValueSafe(out MovementSpeed);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ID);
            writer.WriteValueSafe(UnitType);
            writer.WriteValueSafe(Role);
            writer.WriteValueSafe(Faction);
            writer.WriteValueSafe(Name == null ? "" : Name);
            writer.WriteValueSafe(HitBoxSize);
            writer.WriteValueSafe(ScanRange);
            writer.WriteValueSafe(MovementSpeed);
        }
    }
}

[Serializable]
public struct Defense : INetworkSerializable
{
    public int MaxHealth;
    public int MaxShield;
    [Range(-100, 100)] [Tooltip("Physical damage reduction percentage, from -100% to 100%")]
    public sbyte Armor;
    [Range(-100, 100)] [Tooltip("Dodge chance, from 0% to 100%")]
    public byte DodgeChance;
    public Resistances Resistances;

    public Defense(int maxHealth, int maxShield, sbyte armor, byte dodgeChance, Resistances resistances)
    {
        MaxHealth   = maxHealth;
        MaxShield   = maxShield;
        Armor       = armor;
        DodgeChance = dodgeChance;
        Resistances = resistances;
    }

    public Defense(Defense data)
    {
        MaxHealth   = data.MaxHealth;
        MaxShield   = data.MaxShield;
        Armor       = data.Armor;
        DodgeChance = data.DodgeChance;
        Resistances = data.Resistances;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out MaxHealth);
            reader.ReadValueSafe(out MaxShield);
            reader.ReadValueSafe(out Armor);
            reader.ReadValueSafe(out DodgeChance);
            reader.ReadValueSafe(out Resistances);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(MaxHealth);
            writer.WriteValueSafe(MaxShield);
            writer.WriteValueSafe(Armor);
            writer.WriteValueSafe(DodgeChance);
            writer.WriteValueSafe(Resistances);
        }
    }
}

[Serializable][Tooltip("Element damage reduction percentages, from -100% to 100% per element")] 
public struct Resistances  : INetworkSerializable
{


    [Range(-100, 100)][Tooltip("Fire element damage reduction percentage, from -100% to 100%")] 
    public sbyte Fire;

    [Range(-100, 100)][Tooltip("Frost element damage reduction percentage, from -100% to 100%")] 
    public sbyte Frost;

    [Range(-100, 100)][Tooltip("Lightning element damage reduction percentage, from -100% to 100%")] 
    public sbyte Lightning;

    [Range(-100, 100)][Tooltip("Poison element damage reduction percentage, from -100% to 100%")] 
    public sbyte Poison;

    [Range(-100, 100)][Tooltip("Nature element damage reduction percentage, from -100% to 100%")] 
    public sbyte Nature;

    [Range(-100, 100)][Tooltip("Holy element damage reduction percentage, from -100% to 100%")] 
    public sbyte Holy;

    [Range(-100, 100)][Tooltip("Shadow element damage reduction percentage, from -100% to 100%")] 
    public sbyte Shadow;

    [Range(-100, 100)][Tooltip("Arcane element damage reduction percentage, from -100% to 100%")] 
    public sbyte Arcane;

    public Resistances(sbyte fire, sbyte frost, sbyte lightning, sbyte poison, sbyte nature, sbyte holy, sbyte shadow, sbyte arcane)
    {
        Fire        = fire;
        Frost       = frost;
        Lightning   = lightning;
        Poison      = poison;
        Nature      = nature;
        Holy        = holy;
        Shadow      = shadow;
        Arcane      = arcane;
    }

    public Resistances(Resistances data)
    {
        Fire        = data.Fire;
        Frost       = data.Frost;
        Lightning   = data.Lightning;
        Poison      = data.Poison;
        Nature      = data.Nature;
        Holy        = data.Holy;
        Shadow      = data.Shadow;
        Arcane      = data.Arcane;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Fire);
            reader.ReadValueSafe(out Frost);
            reader.ReadValueSafe(out Lightning);
            reader.ReadValueSafe(out Poison);
            reader.ReadValueSafe(out Nature);
            reader.ReadValueSafe(out Holy);
            reader.ReadValueSafe(out Shadow);
            reader.ReadValueSafe(out Arcane);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Fire);
            writer.WriteValueSafe(Frost);
            writer.WriteValueSafe(Lightning);
            writer.WriteValueSafe(Poison);
            writer.WriteValueSafe(Nature);
            writer.WriteValueSafe(Holy);
            writer.WriteValueSafe(Shadow);
            writer.WriteValueSafe(Arcane);
        }
    }
}

[Serializable]
public struct Attack : INetworkSerializable
{
    public Element Element;
    public AltitudeType Altitude;
    [Range(-100, 100)][Tooltip("Hit chance percentage, from -100% to 100%. 0 is normal")]
    public sbyte HitChance;
    public byte Damage;
    [Range(0, 100)][Tooltip("Critical chance percentage, from 0% to 100%")]
    public byte CriticalChance;
    public byte CriticalDamageMultiplier;
    [Range(0, 100)][Tooltip("Damage penetration percentage, from 0% to 100%")]
    public byte Penetration;
    [Tooltip("The amount of seconds per attack. If speed is 0.5 an attack will occur every 0.5 second(s).")]
    public float Speed;
    public byte Range;
    public bool IsRanged;
    public float RangedCollisionDistance;
    public bool IsSplash;
    public byte SplashRadius;
    [Tooltip("The travel speed of a projectile")]
    public float TravelSpeed;
    [Tooltip("The animation delay until an expected attack happens")]
    public float AnimationDelay;

    public Attack(Element element, AltitudeType altitude, sbyte hitChance, byte damage, byte criticalChance, byte criticalDamageMultiplier, byte penetration, float speed, byte range, bool isRanged, float rangedCollisionDistance, bool isSplash, byte splashRadius, float travelSpeed, float animationDelay)
    {
        Element = element;
        Altitude = altitude;
        HitChance = hitChance;
        Damage = damage;
        CriticalChance = criticalChance;
        CriticalDamageMultiplier = criticalDamageMultiplier;
        Penetration = penetration;
        Speed = speed;
        Range = range;
        IsRanged = isRanged;
        RangedCollisionDistance = rangedCollisionDistance;
        IsSplash = isSplash;
        SplashRadius = splashRadius;
        TravelSpeed = travelSpeed;
        AnimationDelay = animationDelay;
    }

    public Attack(Attack data)
    {
        Element                  = data.Element;
        Altitude                 = data.Altitude;
        HitChance                = data.HitChance;
        Damage                   = data.Damage;
        CriticalChance           = data.CriticalChance;
        CriticalDamageMultiplier = data.CriticalDamageMultiplier;
        Penetration              = data.Penetration;
        Speed                    = data.Speed;
        Range                    = data.Range;
        IsRanged                 = data.IsRanged;
        RangedCollisionDistance  = data.RangedCollisionDistance;
        IsSplash                 = data.IsSplash;
        SplashRadius             = data.SplashRadius;
        TravelSpeed              = data.TravelSpeed;
        AnimationDelay           = data.AnimationDelay;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Element);
            reader.ReadValueSafe(out Altitude);
            reader.ReadValueSafe(out HitChance);
            reader.ReadValueSafe(out Damage);
            reader.ReadValueSafe(out CriticalChance);
            reader.ReadValueSafe(out CriticalDamageMultiplier);
            reader.ReadValueSafe(out Penetration);
            reader.ReadValueSafe(out Speed);
            reader.ReadValueSafe(out Range);
            reader.ReadValueSafe(out IsRanged);
            reader.ReadValueSafe(out RangedCollisionDistance);
            reader.ReadValueSafe(out IsSplash);
            reader.ReadValueSafe(out SplashRadius);
            reader.ReadValueSafe(out TravelSpeed);

        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Element);
            writer.WriteValueSafe(Altitude);
            writer.WriteValueSafe(HitChance);
            writer.WriteValueSafe(Damage);
            writer.WriteValueSafe(CriticalChance);
            writer.WriteValueSafe(CriticalDamageMultiplier);
            writer.WriteValueSafe(Penetration);
            writer.WriteValueSafe(Speed);
            writer.WriteValueSafe(Range);
            writer.WriteValueSafe(IsRanged);
            writer.WriteValueSafe(RangedCollisionDistance);
            writer.WriteValueSafe(IsSplash);
            writer.WriteValueSafe(SplashRadius);
            writer.WriteValueSafe(TravelSpeed);
        }
    }
}

[Serializable]
public struct Ability : INetworkSerializable
{
    public Element Element;
    public byte Power;
    [Range(0, 100)][Tooltip("Critical chance percentage, from 0% to 100%")]
    public byte CriticalChance;
    public byte CriticalDamage;
    [Range(0, 100)][Tooltip("Ability power damage penetration percentage, from 0% to 100%")]
    public byte Penetration;
    public byte Speed;
    public byte Cooldown;
    public byte CastDuration;
    public byte Duration;
    public float AnimationDelay;

    public Ability(Element element, byte power, byte criticalChance, byte criticalDamage, byte penetration, byte speed, byte cooldown, byte castDuration, byte duration, byte animationDelay)
    {
        Element        = element;
        Power          = power;
        CriticalChance = criticalChance;
        CriticalDamage = criticalDamage;
        Penetration    = penetration;
        Speed          = speed;
        Cooldown       = cooldown;
        CastDuration   = castDuration;
        Duration       = duration;
        AnimationDelay = animationDelay;
    }

    public Ability(Ability data)
    {
        Element        = data.Element;
        Power          = data.Power;
        CriticalChance = data.CriticalChance;
        CriticalDamage = data.CriticalDamage;
        Penetration    = data.Penetration;
        Speed          = data.Speed;
        Cooldown       = data.Cooldown;
        CastDuration   = data.CastDuration;
        Duration       = data.Duration;
        AnimationDelay = data.AnimationDelay;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Element);
            reader.ReadValueSafe(out Power);
            reader.ReadValueSafe(out CriticalChance);
            reader.ReadValueSafe(out CriticalDamage);
            reader.ReadValueSafe(out Penetration);
            reader.ReadValueSafe(out Speed);
            reader.ReadValueSafe(out Cooldown);
            reader.ReadValueSafe(out CastDuration);
            reader.ReadValueSafe(out Duration);
            reader.ReadValueSafe(out AnimationDelay);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Element);
            writer.WriteValueSafe(Power);
            writer.WriteValueSafe(CriticalChance);
            writer.WriteValueSafe(CriticalDamage);
            writer.WriteValueSafe(Penetration);
            writer.WriteValueSafe(Speed);
            writer.WriteValueSafe(Cooldown);
            writer.WriteValueSafe(CastDuration);
            writer.WriteValueSafe(Duration); 
            writer.WriteValueSafe(AnimationDelay); 
        }
    }
}

public enum EntityType
{
    Building,
    Spawner,
    Unit,
    Defense
}

public enum UnitType
{
    Melee,
    Ranged,
    Flying,
    Unique
}

public enum StealthState
{
    None,
    Stealth,
    Revealed,
}

public enum Element
{
    None,
    Physical,
    Fire,
    Frost,
    Lightning,
    Poison,
    Nature,
    Holy,
    Shadow,
    Arcane,
}

public enum Faction
{
    None,
    Human,
    Elf,
    Dwarf,
    Orc,
    Troll,
    Goblin,
    Demon,
    Elemental,
    Sylvan,      // Nature born beings
    Beast,
    Insect,
    Necrotic,    // Undead, zombies, etc.
    Abyssal,     // Underwater beings
    Runeborn,    // Born from Arcane magic
    Phantom,     // Ghosts
    Venogon,     // Aliens
    Inanimor,    // A spirit that posseses inanimate object(s): Toys, Masks, Weapons, Things, etc.
    Steamweaver, // A steampunk-like robot faction that gained true AI from a magical ore and now live in their own societies.
    Wildhide     // Anthropomorphic, human-like with animal features.
}

public enum AltitudeType
{
    Ground,
    Flying,
    All
}

public enum RoleType
{
    None,
    Damage,
    Tank,
    Support,
    Unique,
}
    
public struct DamagePayload
{
    public float Damage;
    public byte CriticalChance;
    public byte CriticalDamage;
    public byte Penetration;
    public Element Element;
    public bool IsSplash;
    public float SplashRadius;

    public DamagePayload(float damage, byte criticalChance = default, byte criticalDamage = default, byte penetration = default, Element element = Element.Physical, bool isSplash = false, float splashRadius = default)
    {
        Damage         = damage;
        CriticalChance = criticalChance;
        CriticalDamage = criticalDamage;
        Penetration    = penetration;
        Element        = element;
        IsSplash       = isSplash;
        SplashRadius   = splashRadius;
    }

    public DamagePayload(Attack value)
    {
        Damage         = value.Damage;
        CriticalChance = value.CriticalChance;
        CriticalDamage = value.CriticalDamageMultiplier;
        Penetration    = value.Penetration;
        Element        = value.Element;
        IsSplash       = value.IsSplash;
        SplashRadius   = value.SplashRadius;
    }

}