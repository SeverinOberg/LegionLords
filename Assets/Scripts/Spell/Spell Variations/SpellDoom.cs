using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpellDoom : Spell
{

    [SerializeField]
    private NetworkObject _doomExplosion;

    private Entity _entity;

    private DamagePayload _damagePayload = new DamagePayload(9999);

    public override IEnumerator Use(byte clientID, SpellSO spellData, Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, spellData.Radius, ReferenceManager.S.UnitMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out _entity))
            {
                if (!_entity.IsSpawned) continue;
                _entity.TakeDamage(_damagePayload);
            }
        }

        Instantiate(_doomExplosion, position, Quaternion.identity).Spawn();

        yield return null;
    }


}
