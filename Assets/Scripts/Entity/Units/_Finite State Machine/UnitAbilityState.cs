public class UnitAbilityState : UnitBaseState
{

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) { _m = manager; }
    }

}