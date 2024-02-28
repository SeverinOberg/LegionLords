using System;

[Serializable]
public abstract class UnitBaseState
{

    protected UnitStateManager _m;
    public virtual void StartState(UnitStateManager manager) { }

    public virtual void OnEndState() { }

    public virtual void UpdateState() { }

    

}