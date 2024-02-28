public class Base : Turret
{

    protected override void Awake()
    {
        base.Awake();

        Stats = new Stats
        {
            General = new General
            (
                id: 0,
                unitType: UnitType.Unique,
                role: RoleType.None,
                faction: Faction.None,
                name: "Base",
                hitBoxSize: 8,
                scanRange: 45,
                movementSpeed: 0f
            ),
            Defense = new Defense
            (
                maxHealth: 600,
                maxShield: 200,
                armor: 25,
                dodgeChance: 0,
                resistances: new Resistances
                (
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    poison: 0,
                    nature: 0,
                    holy: 0,
                    shadow: 0,
                    arcane: 0
                )
            ),
            Attack = new Attack
            (
                element: Element.None,
                altitude: AltitudeType.All,
                hitChance: 0,
                damage: 8,
                criticalChance: 0,
                criticalDamageMultiplier: 0,
                penetration: 75,
                speed: 0.3f,
                range: 45,
                isRanged: true,
                rangedCollisionDistance: 0.2f,
                isSplash: true,
                splashRadius: 3,
                travelSpeed: 0.5f,
                animationDelay: 0
            )
        };
    }

    protected override void Die(Entity killer)
    {
        MatchManager.S.OnGameOverServerRpc(Team.Value);
    }

}