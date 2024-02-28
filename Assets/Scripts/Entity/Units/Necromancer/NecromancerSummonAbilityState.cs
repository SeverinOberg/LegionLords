using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class NecromancerSummonAbilityState : UnitAbilityState
{

    [SerializeField] private Entity _skeletonPrefab;
    [SerializeField] private int _summonAmount;

    private float _castDurationTimer;

    public override void StartState(UnitStateManager manager)
    {
        if (!_m) _m = manager;

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
        _castDurationTimer += Time.deltaTime;

        if (_castDurationTimer > _m.StatsAbility.Value.CastDuration)
        {
            _m.Animator.SetBool("casting", false);

            if (NetworkManager.Singleton.IsServer)
            {
                for (int i = 0; i < _m.StatsAbility.Value.Power; i++)
                {
                    NetworkObject netObj = Object.Instantiate(_skeletonPrefab, GetSkeletonSpawnPosition(i), Quaternion.identity).GetComponent<NetworkObject>();
                    netObj.SpawnWithOwnership(_m.OwnerClientId, netObj);
                    Unit unit = netObj.GetComponent<Unit>();
                    unit.Team.Value = _m.Team.Value;
                    unit.SpawnPosition.Value = GetSkeletonSpawnPosition(i);
                }
            }

            _m.SwitchState(_m.EngageState);
            return;
        }
    }

    private Vector3 GetSkeletonSpawnPosition(int incrementer)
    {
        return _m.transform.position + Vector3.up * 0.1f + _m.transform.forward + (0.25f * incrementer * _m.transform.right) ;
    }

}