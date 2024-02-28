using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeButton : MonoBehaviour
{

    public TooltipContent TooltipContent { get; private set; } = TooltipContent.Horizontal;

    public UpgradeSO UpgradeData { get; private set; }
    private int _entityID;

    public void OnClick()
    {
        if (!Player.S.UpgradeController.Buy(_entityID, UpgradeData))
        {
            return;
        }

        SelectionManager.S.ClearTooltip();

        Destroy(gameObject);
    }

    public void Set(int entityID, UpgradeSO data)
    {
        _entityID = entityID;
        UpgradeData = data;
        transform.GetChild(0).GetComponent<Image>().sprite = data.Icon;
    }

}
