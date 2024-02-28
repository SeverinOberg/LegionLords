using UnityEngine;
using UnityEngine.UI;

public class UISpellButton : MonoBehaviour
{

    public TooltipContent TooltipContent { get; private set; } = TooltipContent.Horizontal;

    [field:SerializeField] public SpellSO SpellSO { get; private set; }

    public void Set(SpellSO spellSO, Sprite icon)
    {
        SpellSO = spellSO;
        GetComponent<Image>().sprite = icon;
    }

    public void OnClick()
    {
        Debug.Log($"Clicked spell {SpellSO.Title}");
        Player.S.SpellController.InitiateSpell(SpellSO);
    }
    
}
