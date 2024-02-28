public class UnitChaseState : UnitBaseState
{

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) _m = manager;
    }

    public override void UpdateState()
    {
        Chase();
    }

    private void Chase()
    {
        if (_m.IsScanStateCriteria())
        {
            _m.SwitchState(_m.ScanState);
            return;
        }

        if (_m.IsTargetWithinAttackRange())
        {
            _m.SwitchState(_m.EngageState);
            return;
        }
    }

}