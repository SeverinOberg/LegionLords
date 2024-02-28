using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegionUIManager : MonoBehaviour
{

    public static LegionUIManager S { get; private set; }

    [SerializeField]
    private Transform _galleryContent;

    [SerializeField]
    private UILegionUnitButton _legionUnitButtonPrefab;

    [SerializeField]
    private Scrollbar _galleryScrollbar;

    [SerializeField]
    private Button _activateLegionButton;

    [SerializeField]
    private List<Button> _tabs;

    [SerializeField]
    private List<UILegionUnitButton> _tier1Buttons;

    [SerializeField]
    private List<UILegionUnitButton> _tier2Buttons;

    [SerializeField]
    private List<UILegionUnitButton> _tier3Buttons;

    private int _activeTab = 0;

    private List<Entity> _galleryUnits;

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

        _galleryUnits = EntityDatabase.S.GetUnits();

        for (int i = 0; i < _galleryUnits.Count; i++)
        {
            UILegionUnitButton btn = Instantiate(_legionUnitButtonPrefab, _galleryContent);
            btn.Init(_galleryUnits[i]);
        }

        _galleryScrollbar.SetValueWithoutNotify(0);
    }

    private void OnEnable()
    {
        Client.S.LegionManager.OnLegionDataUpdate += OnLegionDataUpdateCallback;
    }

    private void OnDisable()
    {
        Client.S.LegionManager.OnLegionDataUpdate -= OnLegionDataUpdateCallback;
    }

    private void OnLegionDataUpdateCallback(Legion[] legionData)
    {
        UpdateUI(legionData);
    }

    public void ChangeActiveTab(int index)
    {
        _activeTab = index;
        UpdateUI(Client.S.LegionManager.GetLegionData());
    }

    private void UpdateUI(Legion[] legionData)
    {
        if (legionData == null || legionData.Length == 0) return;

        Entity entity = null;

        foreach (var data in legionData[_activeTab].Data)
        {
            int btnIndex = 0;

            foreach (var unit in data.Units)
            {
                if (unit != null)
                {
                    entity = EntityDatabase.S.Get(unit.ID);
                }
                else
                {
                    entity = null;
                }

                if (data.Tier == 1)
                {
                    _tier1Buttons[btnIndex].Set(entity);
                }
                else if (data.Tier == 2)
                {
                   _tier2Buttons[btnIndex].Set(entity);
                }
                else if (data.Tier == 3)
                {
                    _tier3Buttons[btnIndex].Set(entity);
                }
                btnIndex++;
            }
        }
    }

}
