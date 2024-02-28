

public class Skeleton : UnitStateManager
{
    protected override void Awake()
    {
        InitStats();

        base.Awake();
    }

    public override void InitStats()
    {
        base.InitStats();

        Stats = new Stats
        {
            General = new General
            (
                id: 14,
                unitType: UnitType.Melee,
                role: RoleType.Damage,
                faction: Faction.Necrotic,
                name: "Skeleton",
                hitBoxSize: 1,
                scanRange: 35,
                movementSpeed: 5.25f
            ),
            Defense = new Defense
            (
                maxHealth: 9,
                maxShield: 0,
                armor: 0,
                dodgeChance: 0,
                resistances: new Resistances
                (
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    poison: 0,
                    nature: 0,
                    holy: -25,
                    shadow: 25,
                    arcane: 0
                )
            ),
            Attack = new Attack
            (
                element: Element.Physical,
                altitude: AltitudeType.Ground,
                hitChance: 0,
                damage: 4,
                criticalChance: 0,
                criticalDamageMultiplier: 0,
                penetration: 0,
                speed: 1.75f,
                range: 2,
                isRanged: false,
                rangedCollisionDistance: 0,
                isSplash: false,
                splashRadius: 0,
                travelSpeed: 0.3f,
                animationDelay: 0.3f
            )
        };

    }
}
