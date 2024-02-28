using UnityEngine;
using DG.Tweening;

public class SumoniGripAbilityState : UnitAbilityState
{

    private float _castDurationTimer;

    public override void StartState(UnitStateManager manager)
    {
        base.StartState(manager);

        if (_m.Target.IsStunned.Value || _m.Target.IsDead.Value || _m.Target.CompareTag("Spawner"))
        {
            _m.SwitchState(_m.EngageState);
            return;
        }

        _castDurationTimer = 0;
        _m.Animator.SetBool("casting", true);
    }

    public override void OnEndState()
    {
        base.OnEndState();
        _m.Animator.SetBool("casting", false);
        _m.AbilityTimer = 0;
    }

    public override void UpdateState()
    {
        if (!_m.Target || _m.Target.IsDead.Value || !_m.AIDestinationSetter.target)
        {
            _m.SwitchState(_m.ScanState);
            return;
        }

        if (_m.transform.position.DistanceXZ(_m.Target.transform.position) > _m.StatsGeneral.Value.ScanRange)
        {
            _m.SwitchState(_m.ChaseState);
            return;
        }

        _castDurationTimer += Time.deltaTime;

        if (_castDurationTimer > _m.StatsAbility.Value.CastDuration)
        {
            _m.Animator.SetBool("casting", false);

            if (_m.Target.IsDead.Value || !_m.Target.TryGetComponent(out Unit unit))
            {
                _m.SwitchState(_m.EngageState);
                return;
            }

            //unit.Stun(_m.StatsAbility.Value.Duration);

            if (_m.Target.gameObject != null)
            {
                _m.Target.transform.DOLocalMoveY(2, _m.StatsAbility.Value.Duration * 0.25f).OnComplete(() => { _m.Target.transform.DOLocalMoveY(-1, _m.StatsAbility.Value.Duration * 0.75f); });
                _m.Target.transform.DOLocalRotate(new Vector3(0, 360), _m.StatsAbility.Value.Duration);
            }

            _m.SwitchState(_m.EngageState);
            return;
        }
    }

}