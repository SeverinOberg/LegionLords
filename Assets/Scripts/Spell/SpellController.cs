using System;
using Unity.Netcode;
using UnityEngine;

public class SpellController : NetworkBehaviour
{

    [SerializeField]
    private SpellIndicator _spellIndicator;

    public NetworkVariable<bool> UsedSpell = new();

    private bool _isCasting;
    private SpellSO _currentCastingSpell;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        UsedSpell.OnValueChanged += OnUsedSpell;
    }

    private void Update()
    {
        if (!_isCasting) return;

        if (InputController.InputActions.Player.Primary.WasPressedThisFrame())
        {
            UseSpellServerRpc((byte)OwnerClientId, (byte)_currentCastingSpell.ID, _spellIndicator.transform.position);
            CleanUp();
        }
        else if (InputController.InputActions.Player.Secondary.WasPressedThisFrame())
        {
            CleanUp();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity,ReferenceManager.S.GroundMask))
        {
            _spellIndicator.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }
    }

    private void OnUsedSpell(bool previousValue, bool newValue)
    {
        if (!IsOwner) return;
        UIManager.Singleton.DeactivateSpellsMenuUI();
        SelectionManager.S.ClearTooltip();
    }

    public void InitiateSpell(SpellSO spellData)
    {
        if (UsedSpell.Value == true)
        {
            AnnouncementManager.S.Announce("You already used your spell");
            return;
        };

        CleanUp();

        if (spellData.Targetable)
        {
            if (_spellIndicator.gameObject.activeSelf == false)
            {
                _spellIndicator.gameObject.SetActive(true);
            }

            _spellIndicator.SetRadius(spellData.Radius);

            _isCasting = true;
            _currentCastingSpell = spellData;
        }
        else
        {
            UseSpellServerRpc((byte)OwnerClientId, (byte)spellData.ID);
        }
    }

    private void CleanUp()
    {
        if (_spellIndicator.gameObject.activeSelf)
        {
            _spellIndicator.gameObject.SetActive(false);
        }
        _currentCastingSpell = null;
        _isCasting = false;
    }

    [ServerRpc]
    public void UseSpellServerRpc(byte clientID, byte spellID, Vector3 position = new Vector3())
    {
        if (UsedSpell.Value == true) return;

        var spellSO = SpellManager.Singleton.Get((SpellID)spellID);
        StartCoroutine(spellSO.Prefab.Use(clientID, spellSO, position));

        UsedSpell.Value = true;

        Destroy(_spellIndicator.gameObject, 10);
        Destroy(this, 10);
    }

}
