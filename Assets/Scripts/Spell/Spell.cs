using System.Collections;
using UnityEngine;

public enum SpellID
{
    Wrath,
    Doom,
    PoisonCloud,
}

public class Spell : MonoBehaviour
{

    protected SpellController _owner;

    protected virtual void Awake()
    {
        _owner = GetComponent<SpellController>();
    }


    public virtual IEnumerator Use(byte clientID, SpellSO spellData, Vector3 position = new Vector3())
    {
        yield return null;
    }

}