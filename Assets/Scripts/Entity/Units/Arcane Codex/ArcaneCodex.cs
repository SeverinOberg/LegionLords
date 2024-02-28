using System.Collections;
using UnityEngine;

public class ArcaneCodex : UnitStateManager
{

    [Header("Arcane Codex")]
    [SerializeField] private Projectile _projectilePrefab;
    private Transform _projectileSpawnPoint;
    private WaitForSeconds _attackWaitForSeconds;

    protected override void Awake()
    {
        base.Awake();
        _projectileSpawnPoint = transform.Find("Projectile Spawn Point");
        _attackWaitForSeconds = new WaitForSeconds(StatsAttack.Value.TravelSpeed);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            IsFlying.Value = true;
        }
    }

    protected override IEnumerator DoAttack(Entity target)
    {
        yield return _attackWaitForSeconds;
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
