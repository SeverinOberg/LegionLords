using UnityEngine;

public class UISellButton : MonoBehaviour
{

    public TooltipContent TooltipContent { get; private set; } = TooltipContent.Horizontal;

    public void OnClick()
    {
        Spawner selectedBuilding = SelectionManager.S.Selected.GetComponent<Spawner>();

        Player.S.BuildingController.Sell(selectedBuilding);
    }

}
