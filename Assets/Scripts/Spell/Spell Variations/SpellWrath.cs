using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpellWrath : Spell
{

    [SerializeField] private GameObject _particleSystemPrefab;

    private Unit _cacheUnit;

    public override IEnumerator Use(byte clientID, SpellSO spellData, Vector3 _)
    {
        if (!NetworkManager.Singleton.IsServer) yield break;
        var clientOwnedObjects = NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(clientID);
        foreach (var obj in clientOwnedObjects)
        {
            if (!obj.IsSpawned || !obj.CompareTag("Unit")) continue;

            _cacheUnit = obj.GetComponent<Unit>();
            if (_cacheUnit != null)
            {
                _cacheUnit.AddEffectClientRpc((byte)Effects.Wrath);
            }
        }
        yield return null;
    }

}
