using System.Collections;
using UnityEngine;

public class ToadTosser : UnitStateManager
{

    [Header("Toad Tosser")]
    [SerializeField] private Projectile _projectilePrefab;
    private Transform _projectileSpawnPoint;

    protected override void Awake()
    {
        base.Awake();
        _projectileSpawnPoint = transform.Find("Projectile Spawn Point");
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
            isThrow:           true
        );

        yield return null;
    }
}
