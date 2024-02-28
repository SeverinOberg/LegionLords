public class UnitStunnedState : UnitBaseState
{

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) { _m = manager; }

        _m.CanMove = false;
    }

    public override void OnEndState()
    {
        _m.CanMove = true;
    }

}