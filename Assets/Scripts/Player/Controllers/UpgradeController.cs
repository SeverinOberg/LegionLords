using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UpgradeController : NetworkBehaviour
{

    private ResourceController _resourceController;

    private Dictionary<int, List<UpgradeSO>>  _upgrades = new();

    private void Awake()
    {
        _resourceController = GetComponent<ResourceController>();
    }

    public bool Buy(int entityID, UpgradeSO upgrade)
    {
        if (_upgrades.TryGetValue(entityID, out List<UpgradeSO> upgradeList))
        {
            if (upgradeList.Contains(upgrade))
            {
                AnnouncementManager.S.Announce("ALREADY OWN THIS UPGRADE");
                return false;
            }
        }

        if (!_resourceController.CanBuy(upgrade.Cost))
        {
            return false;
        }

        _resourceController.DecrGold(upgrade.Cost);

        if (!_upgrades.ContainsKey(entityID))
        {
            _upgrades.Add(entityID, new List<UpgradeSO>() { upgrade });
        }
        else
        {
            _upgrades[entityID].Add(upgrade);
        }
        
        //ServerStatsManager.S.UpgradeEntityStatsServerRpc((byte)OwnerClientId, upgrade.EntityID, upgrade.ID);

        SelectionManager.S.Refresh();

        return true;
    }

    public List<UpgradeSO> GetUpgrades(int entityID)
    {
        if (!_upgrades.TryGetValue(entityID, out List<UpgradeSO> value))
        {
            return new List<UpgradeSO>();
        }
        return value;
    }

}
