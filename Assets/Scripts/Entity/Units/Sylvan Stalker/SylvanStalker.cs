using System;
using UnityEngine;

public class SylvanStalker : UnitStateManager
{

    [Header("Sylvan Stalker")]
    [SerializeField]
    private Material _baseMat;
    [SerializeField]
    private Material _stealthMat;
    
    [SerializeField]
    private Renderer[] _renderers;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        EnterStealth();
        IsStealthed.OnValueChanged += OnIsStealthedChanged;

        if (IsServer)
        {
            IsStealthed.Value = true;
        } 
    }

    public override void TakeDamage(DamagePayload dmgPayload, Entity attacker = null)
    {
        base.TakeDamage(dmgPayload, attacker);

        IsStealthed.Value = false;
    }

    private void OnIsStealthedChanged(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            EnterStealth();
        }
        else
        {
            ExitStealth();
        }
    }

    private void EnterStealth()
    {
        foreach (var r in _renderers)
        {
            r.material = _stealthMat;
        }
    }

    private void ExitStealth()
    {
        foreach (var r in _renderers)
        {
            r.material = _baseMat;
        }
    }

}
