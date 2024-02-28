using UnityEngine;

public class UnitStateManager : Unit
{

    [Header("Unit State Manager")]
    [HideInInspector] public UnitBaseState CurrentState;
    [HideInInspector] public UnitBaseState LastState;

    [HideInInspector] public UnitScanState    ScanState    = new UnitScanState();
    [HideInInspector] public UnitChaseState   ChaseState   = new UnitChaseState();
    [HideInInspector] public UnitEngageState  EngageState  = new UnitEngageState();
    [HideInInspector] public UnitAbilityState AbilityState = null;
    [HideInInspector] public UnitStunnedState StunnedState = new();

    private void Start()
    {
        CurrentState = ScanState;
        LastState    = CurrentState;

        CurrentState.StartState(this);
    }

    protected override void Update()
    {
        base.Update();

        CurrentState.UpdateState();
    }

    public void SwitchState(UnitBaseState state)
    {
        CurrentState.OnEndState();

        LastState = CurrentState;

        CurrentState = state;

        state.StartState(this);
    }

}