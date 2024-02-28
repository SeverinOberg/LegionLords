using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UpgradeManager : NetworkBehaviour
{

    public static UpgradeManager S;

    [field:SerializeField]
    public UpgradeSO[] AllUpgrades { get; private set; }
    private Dictionary<UpgradeID, UpgradeSO> _upgrades = new();

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

        foreach (var upgrade in AllUpgrades)
        {
            _upgrades.Add(upgrade.ID, upgrade);
        }
    }

    public UpgradeSO Get(UpgradeID id)
    {
        if (_upgrades.TryGetValue(id, out UpgradeSO upgrade))
        {
            return Instantiate(upgrade);
        }
        return null;
    }
}
