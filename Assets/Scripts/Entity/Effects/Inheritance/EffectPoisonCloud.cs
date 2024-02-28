using Unity.Netcode;
using UnityEngine;

public class EffectPoisonCloud : Effect
{

    public EffectPoisonCloud(Effects id) : base(id) {}

    private ParticleSystem _poisonTickPS;

    protected override void OnAwake()
    {
        duration = 1000;
        tickCooldown = 2;
        _poisonTickPS = GameObject.Instantiate(ReferenceManager.S.PoisonTickPS, entity.transform);
    }

    protected override void Tick()
    {
        base.Tick();

        _poisonTickPS.Play();

        if (entity.IsServer)
        {
            entity.TakeDamage(new DamagePayload(2));
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        if (entity)
        {
            GameObject.Destroy(_poisonTickPS);
        }
    }

}
