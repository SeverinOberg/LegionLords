using System.Collections.Generic;

public class UnitsManager
{

    // ID, Unit
    public Dictionary<int, Unit> Units { get; private set; }

    public UnitsManager(Dictionary<int, Unit> unitData)
    {
        Units = unitData;
    }

    public Unit Get(int id)
    {
        if (Units.TryGetValue(id, out Unit unit))
            return unit;
        else
            return null;
    }

}
