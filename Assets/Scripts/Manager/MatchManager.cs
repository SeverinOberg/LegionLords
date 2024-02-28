using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{

    public static MatchManager S;
    
    public NetworkVariable<bool> GameIsInitialized = new();
    public NetworkVariable<bool> GameOver = new();

    public NetworkVariable<float> GameSpeed = new(1);
    
    public List<PlayerData> PlayerData = new();

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 1;
        GameSpeed.OnValueChanged += OnChangeGameSpeed;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsServer)
        {
            GameSpeed.Value = 1;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        GameSpeed.OnValueChanged -= OnChangeGameSpeed;
    }

    public void OnClickQuitMatch()
    {
        OnGameOverServerRpc(defeatedTeam: Player.S.Team.Value);
    }

    private void OnChangeGameSpeed(float prev, float next)
    {
        if (GameOver.Value) return;
        Time.timeScale = next;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnGameOverServerRpc(byte defeatedTeam)
    {
        GameSpeed.Value = 0;
        GameOver.Value = true;

        foreach (var player in Server.S.Players)
        {
            if (player.Team.Value == defeatedTeam)
            {
                OnGameOverDefeatClientRpc(player.ClientRpcParams);
            }
            else
            {
                OnGameOverVictoryClientRpc(player.ClientRpcParams);
            }
        }
 
        // @TODO: Send all data about the match to the server database to handle ranking, statistics, etc.
    }

    [ClientRpc]
    private void OnGameOverVictoryClientRpc(ClientRpcParams clientRpcParams)
    {
        Time.timeScale = 0;
        UIManager.Singleton.ActivateVictoryUI();
        
    }

    [ClientRpc]
    private void OnGameOverDefeatClientRpc(ClientRpcParams clientRpcParams)
    {
        Time.timeScale = 0;
        UIManager.Singleton.ActivateDefeatUI();
    }

    [ClientRpc]
    public void ScanPathingGridClientRpc()
    {
        AstarPath.active.Scan(AstarPath.active.data.gridGraph);
    }


}

