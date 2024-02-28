
public class GoldDigger : UnitStateManager
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
                id: 7,
                unitType: UnitType.Melee,
                role: RoleType.Unique,
                faction: Faction.Wildhide,
                name: "Gold Digger",
                hitBoxSize: 2,
                scanRange: 12,
                movementSpeed: 4f
            ),
            Defense = new Defense
            (
                maxHealth: 10,
                armor: 0,
                dodgeChance: 0,
                maxShield: 0,
                resistances: new Resistances
                (
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    poison: 0,
                    nature: 25,
                    holy: 0,
                    shadow: 0,
                    arcane: 0
                )
            ),
            Attack = new Attack
            (
                element: Element.Physical,
                altitude: AltitudeType.Ground,
                hitChance: 0,
                damage: 3,
                criticalChance: 0,
                criticalDamageMultiplier: 0,
                penetration: 0,
                speed: 2f,
                range: 1,
                isRanged: false,
                rangedCollisionDistance: 0,
                isSplash: false,
                splashRadius: 0,
                travelSpeed: 0.3f,
                animationDelay: 0
            )
        };

    }

}