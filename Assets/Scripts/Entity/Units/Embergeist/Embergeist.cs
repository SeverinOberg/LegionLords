using System.Collections;
using UnityEngine;

public class Embergeist : UnitStateManager
{

    [Header("Embergeist")]
    [SerializeField] private GameObject _fireExplosionPrefab;

    

    private WaitForSeconds _attackWaitForSeconds;

    protected override void Awake()
    {
        base.Awake();
        _attackWaitForSeconds = new WaitForSeconds(StatsAttack.Value.TravelSpeed);
    }

    protected override IEnumerator DoAttack(Entity target)
    {
        Animator.SetTrigger("attack");
        yield return _attackWaitForSeconds;

        if (!IsSpawned) yield break;

        if (IsServer)
        {
            Die(target);
        }
    }

    protected override void Die(Entity killer)
    {
        base.Die(killer);
        Instantiate(_fireExplosionPrefab, transform.position, Quaternion.identity);
        Entity cacheEntity = null;

        Collider[] cacheHits = Physics.OverlapSphere(transform.position, StatsAttack.Value.SplashRadius, ReferenceManager.S.UnitMask | ReferenceManager.S.ObstacleMask);
        for (int i = 0; i < cacheHits.Length; i++)
        {
            if (cacheHits[i].isTrigger) continue;

            cacheEntity = cacheHits[i].GetComponent<Entity>();
            if (
                cacheEntity == null ||
                cacheEntity.Team.Value == Team.Value ||
                cacheEntity.IsFlying.Value == true
            ) continue;
   

            cacheEntity.TakeDamage(new DamagePayload(StatsAttack.Value.Damage));
        }
    }
}
