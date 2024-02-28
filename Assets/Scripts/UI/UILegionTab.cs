using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILegionTab : MonoBehaviour
{

    [field:SerializeField]
    public Image Image { get; private set; }

    [field:SerializeField]
    public Button Button { get; private set; }

    [field:SerializeField]
    public TextMeshProUGUI Text { get; private set; }

    [field:SerializeField]
    public int TabIndex { get; private set; }

    private void OnEnable()
    {
        Button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(OnClick);
    }

    public void OnClick()
    {
        LegionUIManager.S.ChangeActiveTab(TabIndex);
    }

}
