using Unity.Netcode;
using UnityEngine;

public class EffectWrath : Effect
{

    public EffectWrath(Effects id) : base(id) {}

    private Attack _payload = new();

    protected override void OnAwake()
    {
        isSingleUpdate = true;
        duration = 12;

        GameObject _particleSystem = GameObject.Instantiate(ReferenceManager.S.SpellWrathPS, entity.transform);
        _particleSystem.transform.localPosition = new Vector3(0, entity.transform.localScale.y * 0.5f, 0);
        GameObject.Destroy(_particleSystem, duration);
        if (NetworkManager.Singleton.IsServer)
        {
            _payload = entity.StatsAttack.Value;
            _payload.Speed -= entity.StatsAttack.Value.Speed * 0.5f;
            entity.StatsAttack.Value = _payload;
        }
    }

    protected override void OnEnd()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            _payload.Speed += _payload.Speed;
            entity.StatsAttack.Value = _payload;
        }

    }

}
