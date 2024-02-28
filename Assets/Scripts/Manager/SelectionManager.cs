using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager S { get; private set; }

    public class TooltipObject
    {
        public RectTransform UI;
        public Vector2 InitialTooltipPosition;
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Stats;
        public TextMeshProUGUI Cost;
        public bool IsInitialized;
    }

    [SerializeField] private GameObject _sellButtonPrefab;
    [SerializeField] private GameObject _upgradeButtonPrefab;

    public Entity Selected { get; private set; }
    public Building SelectedBuilding { get; private set; }

    private GameObject _selectionMenuUI;
    private Transform _selectionMenuLayoutGroup;
    private GameObject _selectionUpgrades;

    private Image _selectionAvatar;

    private TextMeshProUGUI _statsLabel;
    private TextMeshProUGUI _statsData;
    private UIStatsButton _statsAttackButton;
    private UIStatsButton _statsDefenseButton;
    private UIStatsButton _statsAbilityButton;

    public TooltipObject Tooltip { get; private set; } = new();

    private RaycastHit _hit;
    private Ray _ray;

    private List<GameObject> _selectionObjects = new();

    private EventSystem _eventSystem;
    private bool _isMouseOverUI;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _eventSystem = EventSystem.current;
        _selectionMenuUI = GameObject.Find("UI Selection Menu");
        _selectionMenuLayoutGroup = GameObject.Find("UI Selection Grid Layout Group")?.transform;
        _selectionUpgrades = GameObject.Find("Upgrades");

        _selectionAvatar = GameObject.Find("Selection Avatar")?.GetComponent<Image>();

        _statsLabel = GameObject.Find("Stats Label Text")?.GetComponent<TextMeshProUGUI>();
        _statsData = GameObject.Find("Stats Data Text")?.GetComponent<TextMeshProUGUI>();

        _statsAttackButton = GameObject.Find("Stats Attack")?.GetComponent<UIStatsButton>();
        _statsDefenseButton = GameObject.Find("Stats Defense")?.GetComponent<UIStatsButton>();
        _statsAbilityButton = GameObject.Find("Stats Ability")?.GetComponent<UIStatsButton>();

        _selectionMenuUI.SetActive(false);

        InitTooltipObject();
    }

    private void InitTooltipObject()
    {
        Tooltip.UI = GameObject.Find("UI Selection Tooltip").GetComponent<RectTransform>();
        Tooltip.InitialTooltipPosition = Tooltip.UI.position;
        TextMeshProUGUI[] tooltipTextObjects = Tooltip.UI.transform.GetComponentsInChildren<TextMeshProUGUI>();
        Tooltip.Title = tooltipTextObjects[0];
        Tooltip.Description = tooltipTextObjects[1];
        Tooltip.Stats = tooltipTextObjects[2];
        Tooltip.Cost = tooltipTextObjects[3];
        Tooltip.UI.gameObject.SetActive(false);
        
        Tooltip.IsInitialized = true;
    }

    private void Update()
    {
        _isMouseOverUI = _eventSystem.IsPointerOverGameObject();
    }

    public void TrySelect()
    {
        if (_isMouseOverUI || Player.S.BuildingController.BuildingToPlace != null) return;

        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, ReferenceManager.S.UnitMask | ReferenceManager.S.BuildingMask))
        {
            Entity entity = null;
            if (_hit.transform.TryGetComponent(out entity) || _hit.transform.parent != null && _hit.transform.parent.TryGetComponent(out entity))
            {
                if (entity.Team.Value != Player.S.Team.Value) return;

                // @TODO: Show something different if the enemy clicks your entity
                Selected = entity;
                if (entity.EntityType == EntityType.Spawner)
                {
                    SelectedBuilding = Selected.GetComponent<Building>();
                }
                DestroySelectionObjects();
                Select();
            }
            else
            {
                ClearSelection();
            }
        }
        else
        {
            ClearSelection();
        }
    }

    public void ClearSelection(InputAction.CallbackContext ctx)
    {
        if (!Selected || _isMouseOverUI) return;
        ClearSelection();
    }

    private void Select()
    {
        if (!_selectionMenuUI.activeSelf)
        {
            _selectionMenuUI.SetActive(true);
        }

        SetStats();
        _selectionAvatar.sprite = Selected.Icon;

        if (Selected.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;
        
        if (Selected.EntityType == EntityType.Spawner)
        {
            BuildingSelection();
        }
        else if (Selected.EntityType == EntityType.Unit)
        {
            UnitSelection();
        }
        else if (Selected.EntityType == EntityType.Defense)
        {
            _selectionUpgrades.SetActive(false);
        }
    }

    private void SetStats()
    {
        _statsLabel.text = Selected.StatsGeneral.Value.Name;
        _statsData.text = "";

        if (Selected.EntityType != EntityType.Defense)
        {
            _statsData.text = $"{Selected.StatsGeneral.Value.Faction} Faction\n";

            if (Selected.StatsGeneral.Value.Role != RoleType.None)
            {
                _statsData.text += $"{Selected.StatsGeneral.Value.Role} Role\n";
            }

            if (Selected.EntityType != EntityType.Spawner)
            {
                _statsData.text += $"{Selected.StatsGeneral.Value.UnitType} _type\n";
                _statsData.text += Selected.IsFlying.Value == true ? "Flying _type\n" : "Ground _type\n";   
            }
            else
            {
                _statsData.text += $"Spawns every {SelectedBuilding.BuildingSO.SpawnInterval} seconds\n";
            }
            
            if (Selected.StatsGeneral.Value.MovementSpeed > 0)
            {
                _statsData.text += $"{Selected.StatsGeneral.Value.MovementSpeed} Move Speed\n";
            }
        }

        _statsAttackButton.SetStatsString(Selected);
        _statsDefenseButton.SetStatsString(Selected);
        _statsAbilityButton.SetStatsString(Selected);
    }

    private void BuildingSelection()
    {
        _selectionUpgrades.SetActive(true);

        if (Selected.EntityType == EntityType.Spawner )
        {
            GameObject sellButtonGameObject = Instantiate(_sellButtonPrefab, _selectionMenuLayoutGroup);
            _selectionObjects.Add(sellButtonGameObject);
        }

        if (!Selected.TryGetComponent(out BuildingUpgradeData buildingUpgrades)
            || buildingUpgrades.Upgrades.Count == 0)
        {
            return;
        }

        var currentUpgrades = Player.S.UpgradeController.GetUpgrades(SelectedBuilding.BuildingSO.EntityID);

        foreach (UpgradeSO upgrade in buildingUpgrades.Upgrades)
        {
            if (currentUpgrades.Contains(upgrade))
            {
                continue;
            }

            if (upgrade.Requires != UpgradeID.None)
            {
                if (!currentUpgrades.Find((cu) => cu.ID == upgrade.Requires))
                {
                    continue;
                }
            }

            UIUpgradeButton upgradeButton = Instantiate(_upgradeButtonPrefab, _selectionMenuLayoutGroup).GetComponent<UIUpgradeButton>();
            upgradeButton.Set(SelectedBuilding.BuildingSO.EntityID, upgrade);
            _selectionObjects.Add(upgradeButton.gameObject);
        }
    }

    private void UnitSelection()
    {
        _selectionUpgrades.SetActive(false);
    }

    public void ClearSelection()
    {
        if (_selectionMenuUI.activeSelf)
        {
            _selectionMenuUI.SetActive(false);
        }

        if (Tooltip.UI.gameObject.activeSelf)
        {
            Tooltip.UI.gameObject.SetActive(false);
        }

        DestroySelectionObjects();

        if (Selected != null)
        {
            Selected = null;
        }
    }

    public void ClearTooltip()
    {
        if (Tooltip.UI.gameObject.activeSelf)
        {
            Tooltip.UI.gameObject.SetActive(false);
        }
    }

    private void DestroySelectionObjects()
    {
        foreach (var obj in _selectionObjects)
        {
            Destroy(obj);
        }
    }

    public void Refresh()
    {
        DestroySelectionObjects();

        if (Selected.CompareTag("Spawner"))
        {
            BuildingSelection();
        }
        else if (Selected.CompareTag("Unit"))
        {
            UnitSelection();
        }
    }

}
