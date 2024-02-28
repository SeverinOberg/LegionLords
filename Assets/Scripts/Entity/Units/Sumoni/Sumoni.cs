using System.Collections;
using UnityEngine;

public class Sumoni : UnitStateManager
{
    [Header("Sumoni")]
    [SerializeField] private Projectile _projectilePrefab;
    private Transform _projectileSpawnPoint;

    protected override void Awake()
    {
        base.Awake();
        _projectileSpawnPoint = transform.Find("Projectile Spawn Point");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        AbilityState = new SumoniGripAbilityState();
    }

    protected override IEnumerator DoAttack(Entity target)
    {
        Animator.SetTrigger("attack");
        _projectilePrefab.Spawn(
            instigator:        this,
            target:            target,
            spawnPosition:     _projectileSpawnPoint.position,
            damagePayload:     new DamagePayload(StatsAttack.Value),
            team:              Team.Value,
            isThrow:           false
        );

        yield return null;
    }

}
