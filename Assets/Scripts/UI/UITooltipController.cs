using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	private enum Type
	{
		Upgrade,
		Sell,
        Spell,
        Building,
        Stats
	}

	[SerializeField] private Type _type;

    private UIUpgradeButton _upgradeButton;
    private UISpellButton    _spellButton;
    private UIBuildingButton  _buildingButton;
    private UIStatsButton  _statsButton;

    private Vector2 _horizontalSize = new Vector2(450, 200);
    private Vector2 _verticalSize = new Vector2(200, 250);

    private void Awake()
    {
        switch (_type)
        {
            case Type.Upgrade:
                _upgradeButton = GetComponent<UIUpgradeButton>();
                break;
            case Type.Sell:
                break;
            case Type.Spell:
                _spellButton = GetComponent<UISpellButton>();
                break;
            case Type.Building:
                _buildingButton = GetComponent<UIBuildingButton>();
                break;
            case Type.Stats:
                _statsButton = GetComponent<UIStatsButton>();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectionManager.S.Tooltip.UI.gameObject.SetActive(true);
        InjectData();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SelectionManager.S.Tooltip.UI.gameObject.SetActive(false);
    }

    private void InjectData()
    {
        switch (_type)
        {
            case Type.Upgrade:
                UpgradeInjection();
                break;
            case Type.Sell:
                SellInjection();
                break;
            case Type.Spell:
                SpellInjection();
                break;
            case Type.Building:
                BuildingInjection();
                break;
            case Type.Stats:
                StatsInjection();
                break;
        }
    }

    private void UpgradeInjection()
    {
        SelectionManager.S.Tooltip.UI.position = transform.position;
        SelectionManager.S.Tooltip.UI.sizeDelta = _horizontalSize;
        SelectionManager.S.Tooltip.Title.fontSize = 28;

        SelectionManager.S.Tooltip.Title.text = _upgradeButton.UpgradeData.Title;
        SelectionManager.S.Tooltip.Description.text = _upgradeButton.UpgradeData.Description;
        SelectionManager.S.Tooltip.Cost.text = $"{_upgradeButton.UpgradeData.Cost} Gold";
        string statsString = "";

        for (int i = 0; i < _upgradeButton.UpgradeData.Properties.Length; i++)
        {
            statsString += $"{_upgradeButton.UpgradeData.Properties[i].Key}: {_upgradeButton.UpgradeData.Properties[i].Value} ";
        }

        SelectionManager.S.Tooltip.Stats.text = statsString;
    }

    private void SellInjection()
    {
        SelectionManager.S.Tooltip.UI.position = transform.position;
        SelectionManager.S.Tooltip.UI.sizeDelta = _horizontalSize;
        SelectionManager.S.Tooltip.Title.fontSize = 28;

        SelectionManager.S.Tooltip.Title.text = "Sell Spawner [G]";
        SelectionManager.S.Tooltip.Description.text = "Sell your building and get 50% of your gold back. " +
            "If you regret the purchase of a building, you have 10 seconds to regret your decision and get the full amount back.";
        SelectionManager.S.Tooltip.Stats.text = "";
        SelectionManager.S.Tooltip.Cost.text = "";
    }

    private void SpellInjection()
    {
        SelectionManager.S.Tooltip.UI.position = transform.position;
        SelectionManager.S.Tooltip.UI.sizeDelta = _horizontalSize;
        SelectionManager.S.Tooltip.Title.fontSize = 28;

        SelectionManager.S.Tooltip.Title.text = _spellButton.SpellSO.Title;
        SelectionManager.S.Tooltip.Description.text = _spellButton.SpellSO.Description;
        SelectionManager.S.Tooltip.Stats.text = "";
        SelectionManager.S.Tooltip.Cost.text = "";
    }

    private void BuildingInjection()
    {
        SelectionManager.S.Tooltip.UI.position = transform.position;
        SelectionManager.S.Tooltip.UI.sizeDelta = _horizontalSize;
        SelectionManager.S.Tooltip.Title.fontSize = 28;

        SelectionManager.S.Tooltip.Title.text       = _buildingButton.BuildingSO.Label;
        SelectionManager.S.Tooltip.Description.text = _buildingButton.BuildingSO.Description;
        SelectionManager.S.Tooltip.Cost.text        = $"{_buildingButton.BuildingSO.Cost} Gold";
        if (_buildingButton.BuildingSO.SpawnInterval != 0)
        {
            SelectionManager.S.Tooltip.Stats.text = $"Spawns Every {_buildingButton.BuildingSO.SpawnInterval} Seconds";
        }
        else
        {
            SelectionManager.S.Tooltip.Stats.text = $"{_buildingButton.BuildingSO.Income} Gold Per Second";
        }
        
    }

    private void StatsInjection()
    {
        SelectionManager.S.Tooltip.UI.position = transform.position;
        SelectionManager.S.Tooltip.UI.sizeDelta = _verticalSize;
        SelectionManager.S.Tooltip.Title.fontSize = 18;

        SelectionManager.S.Tooltip.Title.text = _statsButton.DataString;
        SelectionManager.S.Tooltip.Description.text = "";
        SelectionManager.S.Tooltip.Stats.text = "";
        SelectionManager.S.Tooltip.Cost.text = "";
    }

}

public enum TooltipContent
{
    Horizontal,
    Vertical,
}
