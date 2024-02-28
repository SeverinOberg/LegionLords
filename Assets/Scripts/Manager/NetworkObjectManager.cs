using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Global NetworkObject holder. This is used for Clients to also see NetworkObjects. Consider removing this for security reasons(?)
/// </summary>
public class NetworkObjectManager : MonoBehaviour
{
    public static NetworkObjectManager S;

    private Dictionary<ulong, NetworkObject> Data = new();

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Add(NetworkObject netObj)
    {
        Data.Add(netObj.NetworkObjectId, netObj);
    }

    public void Remove(ulong netObjId)
    {
        Data.Remove(netObjId);
    }

    public Transform GetTransform(ulong netObjId)
    {
        if (Data[netObjId])
        {
            return Data[netObjId].transform;
        }
        return null;
    }

    public Entity GetEntity(ulong netObjId)
    {
        return Data[netObjId].GetComponent<Entity>();
    }

}
