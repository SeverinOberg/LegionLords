using System.Collections.Generic;
using UnityEngine;

public class EntityDatabase : MonoBehaviour
{

    public static EntityDatabase S { get; private set; }

    [field: SerializeField]
    public List<Entity> Data { get; private set; }

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Entity Get(int id)
    {
        return Data.Find((e) => e.ID == id);
    }

    public List<Entity> GetBatch(int[] idArray)
    {
        List<Entity> result = new();
        for (int i = 0; i < idArray.Length; i++)
        {
            var d = Data.Find((d) => d.ID == idArray[i]);
            if (d)
            {
                result.Add(d);
            }
        }
        return result;
    }

    public List<Entity> GetUnits()
    {
        List<Entity> payload = new();

        for (int i = 0; i < Data.Count; i++)
        {
            if (Data[i].IsUnit)
            {
                payload.Add(Data[i]);
            }
        }

        return payload;
    }

}
