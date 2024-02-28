using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpellPoisonCloud : Spell
{           
    
    [SerializeField]
    private NetworkObject _poisonCloud;

    public override IEnumerator Use(byte clientID, SpellSO spellData, Vector3 position)
    {
        Instantiate(_poisonCloud, position, Quaternion.identity).Spawn();
        yield return null;
    }

}
