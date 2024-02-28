using UnityEngine;

[CreateAssetMenu(fileName = "SpellSO", menuName = "Scriptable Objects/SpellSO")]
public class SpellSO : ScriptableObject
{

    public SpellID ID;
    public string Title;
    [TextArea]
    public string Description;
    public bool   Targetable;
    public float  Radius;
    public float  Power;
    public float  Duration;
    public Spell  Prefab;
}
