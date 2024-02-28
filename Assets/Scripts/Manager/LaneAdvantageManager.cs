using System;
using Unity.Netcode;
using UnityEngine;

public class LaneAdvantageManager : NetworkBehaviour
{

    public static LaneAdvantageManager S { get; private set; }

    [SerializeField] private Renderer _laneAdvantageIndicator;

    private int _team1Units;
    private int _team2Units;

    private byte _currentTeamWithAdvantage;

    private const byte _advantageReward = 1;

    public Action<byte> OnLaneAdvantageChanged;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDieCallback(Entity entity)
    {
        Remove(entity);
        entity.OnDie -= OnDieCallback;
    }

    public void Add(Entity entity)
    {
        if (!IsServer) return;

        entity.OnDie += OnDieCallback;

        if (entity.Team.Value == 1)
        {
            _team1Units++;
        }
        else
        {
            _team2Units++;
        }

        ValidateAdvantage();
    }

    public void Remove(Entity entity)
    {
        if (!IsServer) return;

        if (entity.Team.Value == 1)
        {
            _team1Units--;

        }
        else
        {
            _team2Units--;
        }

        ValidateAdvantage();
    }

    private void ValidateAdvantage()
    {

            if (_team1Units > 0 && _team2Units < 1)
            {
                SetTeamAdvantage(1);
            }
            else if (_team2Units > 0 && _team1Units < 1)
            {
                SetTeamAdvantage(2); 
            }
        
    }

    private void SetTeamAdvantage(byte team)
    {

        if (team == 1)
        {
            if (_currentTeamWithAdvantage == 1) return;

            if (_currentTeamWithAdvantage == 0)
            {
                ServerResourceManager.S.IncreaseGoldPerSecond(1, _advantageReward);
            }
            else
            {
                ServerResourceManager.S.DecreaseGoldPerSecond(2, _advantageReward);
                ServerResourceManager.S.IncreaseGoldPerSecond(1, _advantageReward);
            }

            _currentTeamWithAdvantage = 1;
        }
        else
        {
            if (_currentTeamWithAdvantage == 2) return;

            if (_currentTeamWithAdvantage == 0)
            {
                ServerResourceManager.S.IncreaseGoldPerSecond(2, _advantageReward);
            }
            else
            {
                ServerResourceManager.S.DecreaseGoldPerSecond(1, _advantageReward);
                ServerResourceManager.S.IncreaseGoldPerSecond(2, _advantageReward);
            }

            _currentTeamWithAdvantage = 2;
        }

        OnLaneAdvantageChanged?.Invoke(_currentTeamWithAdvantage);
        ChangeIndicatorColorClientRpc(team);
        
    }

    [ClientRpc]
    private void ChangeIndicatorColorClientRpc(byte team)
    {
        _laneAdvantageIndicator.material = ReferenceManager.S.GetMaterialByTeam(team);
    }

}