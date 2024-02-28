using System;
using System.Threading.Tasks;
using UnityEngine;
using LegionLords.API;

public class LegionManager
{

    private Legion[] _legionData;

    public Action<Legion[]> OnLegionDataUpdate;

    public bool IsValid()
    {
        return _legionData.Length > 0;
    }

    public Legion[] GetLegionData()
    {
        return _legionData;
    }

    public Legion GetActiveLegion()
    {
        for (int i = 0; i < _legionData.Length; i++)
        {
            if (_legionData[i].IsActive)
            {
                return _legionData[i];
            }
        }

        return null;
    }

    public int? GetActiveLegionIndex()
    {
        for (int i = 0; i < _legionData.Length; i++)
        {
            if (_legionData[i].IsActive)
            {
                return i;
            }
        }

        return null;
    }

    public async void UpdateLegion()
    {
        await Set();
    }

    private async Task Set()
    {
        var payload = await API.S.GetLegions();

        if (payload.Data == null && payload.Data.Length <= 0)
        {
            Debug.Log("API Error: legions empty or not found");
            return;
        }

        _legionData = new Legion[payload.Data.Length];

        for (int i = 0; i < payload.Data.Length; i++)
        {
            _legionData[i] = new Legion(
                payload.Data[i].ID,
                payload.Data[i].Name,
                payload.Data[i].IsActive,
                GetLocalRefLegionDataFromUnitIDArray(payload.Data[i].UnitIDs)
            );
        }

        OnLegionDataUpdate?.Invoke(_legionData);
    }

    private LegionData[] GetLocalRefLegionDataFromUnitIDArray(int[] unitIDs)
    {
        Entity entity = null;
        LegionData[] result = new LegionData[3];

        int maxSize = 7;

        result[0] = new LegionData { Tier = 1, Units = new LegionUnit[maxSize] };
        result[1] = new LegionData { Tier = 2, Units = new LegionUnit[maxSize] };
        result[2] = new LegionData { Tier = 3, Units = new LegionUnit[maxSize] };

        for (int i = 0; i < result.Length; i++)
        {
            int unitIndex = 0;
            for (int j = 0; j < unitIDs.Length; j++)
            {
                entity = EntityDatabase.S.Get(unitIDs[j]);
                if (result[i].Tier == entity.Tier)
                {
                    result[i].Units[unitIndex] = new LegionUnit
                    {
                        ID = entity.ID,
                        Tier = entity.Tier,
                        Type = "unit",
                        Name = entity.name,
                        Icon = entity.Icon,
                    };
                    unitIndex++;
                }
            }
        }
  
        return result;
    }
}

public class Legion
{
    public int ID;
    public string Name;
    public bool IsActive;
    public LegionData[] Data;

    public Legion(int id, string name, bool isActive, LegionData[] data)
    {
        ID = id;
        Name = name;
        IsActive = isActive;
        Data = data;
    }
}

public class LegionData
{
    public byte Tier;
    public LegionUnit[] Units;
}

public class LegionUnit
{
    public int ID;
    public byte Tier;
    public string Type;
    public string Name;
    public Sprite Icon;
}