public class UnitEngageState : UnitBaseState
{

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) _m = manager;
        _m.CanMove = false;
    }

    public override void OnEndState()
    {
        _m.CanMove = true;
    }

    public override void UpdateState()
    {
        if (_m.IsScanStateCriteria())
        {
            _m.SwitchState(_m.ScanState);
            return;
        }

        if (_m.IsTargetOutOfAttackRange())
        {
            _m.SwitchState(_m.ChaseState);
            return;
        }

        if (_m.StatsAbility.Value.Cooldown > 0)
        {
            if (_m.AbilityTimer > _m.StatsAbility.Value.Cooldown)
            {
                _m.AbilityTimer = 0;
                _m.SwitchState(_m.AbilityState);
                return;
            }
        }

        if (_m.AttackTimer > _m.StatsAttack.Value.Speed)
        {
            _m.AttackTimer = 0;
            if (_m.Target) _m.Attack(_m.Target);
        }
    }

}