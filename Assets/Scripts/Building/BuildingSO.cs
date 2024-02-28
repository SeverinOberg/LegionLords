using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSO", menuName = "Scriptable Objects/BuildingSO")]
public class BuildingSO : ScriptableObject
{
    [field:SerializeField] public string Label             { get; private set; }
    [field:TextArea()]
    [field:SerializeField] public string Description       { get; private set; }
    [field:SerializeField] public int    Cost              { get; private set; }
    [field:SerializeField] public NetworkObject UnitNetworkObject { get; private set; }
    [field:SerializeField] public int    EntityID { get; private set; }
    [field:SerializeField] public int    SpawnInterval     { get; private set; }
    [field:SerializeField] public int    Income            { get; private set; }
}
