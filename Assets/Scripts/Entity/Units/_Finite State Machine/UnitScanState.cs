using UnityEngine;


public class UnitScanState : UnitBaseState
{

    protected Entity _target;
    protected Entity _selectedTarget;

    protected float _scanTimer    = 1;
    protected float _scanCooldown = 1;

    protected Collider[] _hits;

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) _m = manager;
        
        _scanTimer = 1;

        _m.Target = null;
        _m.AIDestinationSetter.target = null;

        _selectedTarget = null;
        _target = null;
    }

    public override void UpdateState()
    {
        _scanTimer += Time.deltaTime;

        if (_scanTimer < _scanCooldown) return;
        _scanTimer = 0;

        if (_m.AIDestinationSetter.target == null)
        {
            var enemyBase = ReferenceManager.S.GetEnemyBaseNetworkObject(_m.Team.Value);
            _m.AIDestinationSetter.target = enemyBase.transform;
            if (_m.IsServer) _m.SetDestinationTargetClientRpc(enemyBase.NetworkObjectId);
        }

        Scan();
    }

    private void Scan()
    {
        _hits = Physics.OverlapSphere(_m.transform.position, _m.StatsGeneral.Value.ScanRange, ReferenceManager.S.UnitMask | ReferenceManager.S.ObstacleMask);

        if (_hits.Length == 0)
        {
            return;
        }
        else
        {
            SortForClosestTargetFromHits(ref _hits);
        } 

        if (_selectedTarget == null) return;

        _m.AIDestinationSetter.target = _selectedTarget.transform;
            _m.Target = _selectedTarget;

        if (_m.IsServer)
        {
            _m.CurrentAttackRange.Value = _m.Stats.Attack.Range + _m.Target.StatsGeneral.Value.HitBoxSize;
            _m.SetDestinationTargetAndTargetClientRpc(_selectedTarget.NetworkObjectId);
        }

        _m.SwitchState(_m.ChaseState);
    }

    private void SortForClosestTargetFromHits(ref Collider[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            _target = hits[i].GetComponent<Entity>();
            if (_target == null)
            {
                _target = hits[i].transform.parent.GetComponent<Entity>();
                if (_target == null)
                {
                    continue;
                }
            }

            if (!_target.IsSpawned || _target.IsDead.Value || _target.IsStealthed.Value || _m.Team.Value == _target.Team.Value) 
                continue;

            if (_m.StatsAttack.Value.Altitude == AltitudeType.Ground)
            {
                if (_target.IsFlying.Value) continue;
            }
            else if (_m.StatsAttack.Value.Altitude == AltitudeType.Flying)
            {
                if (!_target.IsFlying.Value) continue;
            }

            if (_selectedTarget == null || _m.transform.position.DistanceXZ(_target.transform.position) < _m.transform.position.DistanceXZ(_selectedTarget.transform.position))
            {
                _selectedTarget = _target;
            }
        }
    }

}
