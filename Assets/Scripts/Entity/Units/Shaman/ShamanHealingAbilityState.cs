using System.Collections;
using UnityEngine;

[System.Serializable]
public class ShamanHealingAbilityState : UnitAbilityState
{

    [SerializeField]
    private GameObject _healingEffectPrefab;

    private Entity _healTarget;

    private Collider[] _hits;

    public override void StartState(UnitStateManager manager)
    {
        base.StartState(manager);

        _m.Animator.SetBool("casting", true);

        _healTarget = GetLowestHealthAlly();
        if (_healTarget == null)
        {
            _m.SwitchState(_m.EngageState);
            return;
        }

        _m.StartCoroutine(Ability());
    }

    public override void OnEndState()
    {
        base.OnEndState();

        _m.Animator.SetBool("casting", false);
        _m.AbilityTimer = 0;
        _healTarget = null;
    }

    private IEnumerator Ability()
    {

        yield return new WaitForSeconds(_m.StatsAbility.Value.CastDuration);

        if (!_healTarget)
        {
            _m.SwitchState(_m.ScanState);
            yield return null;
        }

        if (_m.transform.position.DistanceXZ(_healTarget.transform.position) > _m.StatsGeneral.Value.ScanRange)
        {
            _m.SwitchState(_m.ChaseState);
            yield return null;
        }

        if (_m.IsServer) _m.NetworkAnimator.SetTrigger("cast");
        _m.Animator.SetBool("casting", false);

        yield return new WaitForSeconds(_m.StatsAbility.Value.AnimationDelay);

        if (_healTarget && !_healTarget.IsDead.Value)
        {
            Object.Instantiate(_healingEffectPrefab, _healTarget.transform).transform.localScale = _healTarget.StatsGeneral.Value.HitBoxSize.MakeVector3();

            if (_m.IsServer)
            {
                _healTarget.Heal(_m.StatsAbility.Value.Power);
            }
        }

        _m.SwitchState(_m.EngageState);
    }

    private Entity GetLowestHealthAlly()
    {
        _hits = Physics.OverlapSphere(_m.transform.position, _m.StatsGeneral.Value.ScanRange, ReferenceManager.S.UnitMask);

        Entity lowestHealthAlly = null;

        for (int i = 0; i < _hits.Length; i++)
        {
            if (_hits[i].TryGetComponent(out Entity ally) && ally.Team.Value == _m.Team.Value)
            {
                if (ally.IsDead.Value) continue;

                if (lowestHealthAlly == null)
                {
                    lowestHealthAlly = ally;
                    continue;
                }

                if (ally.Health.Value < ally.StatsDefense.Value.MaxHealth && ally.Health.Value < lowestHealthAlly.Health.Value)
                {
                    lowestHealthAlly = ally;
                }
            }
        }

        if (lowestHealthAlly == null || lowestHealthAlly.Health.Value == lowestHealthAlly.StatsDefense.Value.MaxHealth)
        {
            return null;
        }

        return lowestHealthAlly;
    }

}