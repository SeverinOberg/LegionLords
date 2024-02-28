using System.Collections;
using UnityEngine;

public class Shaman : UnitStateManager
{
    [Header("Shaman")]
    [SerializeField] private ShamanHealingAbilityState _abilityState;
    [SerializeField] private Projectile                _projectilePrefab;
    [SerializeField] private Transform                 _projectileSpawnPoint;

    private WaitForSeconds _waitForAnimationDelay;

    protected override void Awake()
    {
        InitStats();

        AbilityState = _abilityState;

        if (_projectileSpawnPoint == null)
        {
             _projectileSpawnPoint = transform.Find("Projectile Spawn Point");
        }

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
                unitType: UnitType.Ranged,
                role: RoleType.Support,
                faction: Faction.Orc,
                name: "Shaman",
                hitBoxSize: 1,
                scanRange: 35,
                movementSpeed: 5f
            ),
            Defense = new Defense
            (
                maxHealth: 13,
                maxShield: 0,
                armor: 0,
                dodgeChance: 0,
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
                element: Element.Nature,
                altitude: AltitudeType.All,
                hitChance: 0,
                damage: 7,
                criticalChance: 0,
                criticalDamageMultiplier: 0,
                penetration: 0,
                speed: 2.25f,
                range: 25,
                isRanged: true,
                rangedCollisionDistance: 0.1f,
                isSplash: false,
                splashRadius: 0,
                travelSpeed: 0.3f,
                animationDelay: 0.3f
            ),
            Ability = new Ability
            (
                element: Element.Nature,
                power: 5,
                criticalChance: 0,
                criticalDamage: 5,
                penetration: 0,
                speed: 2,
                cooldown: 7,
                castDuration: 1,
                duration: 0,
                animationDelay: 1
            )
        };

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _waitForAnimationDelay = new WaitForSeconds(StatsAttack.Value.AnimationDelay);
    }

    //protected override IEnumerator DoAttack(Entity target)
    //{
    //    Animator.SetTrigger("attack");

    //    yield return _waitForAnimationDelay;

    //    if (target.IsDead.Value) yield break;

    //    target.Attack();

    //    //_projectilePrefab.Spawn(
    //    //    instigator:        this,
    //    //    target:            target,
    //    //    spawnPosition:     _projectileSpawnPoint.position,
    //    //    damagePayload:     new DamagePayload(StatsAttack.Value),
    //    //    team:              Team.Value,
    //    //    isThrow:           false
    //    //);

    //    yield return null;
    //}

}
