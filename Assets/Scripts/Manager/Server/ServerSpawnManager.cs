using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using static UnityEditor.PlayerSettings;

/// <summary>
/// [SERVER ONLY] Manages wave spawning
/// </summary>
public class ServerSpawnManager : NetworkBehaviour
{

    public static ServerSpawnManager S { get; private set; }

    private List<PlayerSpawnData> PlayerSpawnData = new();

    private List<PlayerSpawnData> _cacheTeam1SpawnData = new();
    private List<PlayerSpawnData> _cacheTeam2SpawnData = new();

    private const float SPAWN_COOLDOWN = 45;
    public float TimeUntilSpawn = SPAWN_COOLDOWN;

    private Transform _team1SpawnZone;
    private Transform _team2SpawnZone;

    private WaitForSeconds _waitForSpawnCooldown = new WaitForSeconds(SPAWN_COOLDOWN);

    private Vector3 _cacheSpawnPosition;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback  += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback  -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }

    private void Update()
    {
        TimeUntilSpawn -= Time.deltaTime;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerSpawnDataActive((byte)clientId, true);
    }

    
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        SetPlayerSpawnDataActive((byte)clientId, false);
    }

    public void Run(Transform team1Spawn, Transform team2Spawn)
    {
        if (!IsServer) return;

        _team1SpawnZone = team1Spawn;
        _team2SpawnZone = team2Spawn;

        for (int i = 0; i < Server.S.Players.Count; i++)
        {
            var player = Server.S.Players[i];

            if (i < 2)
            {
                UISpawnTimeController.S.SetSpawnTimeClientRpc(45, player.ClientRpcParams);
            }
            else if (i < 4)
            {
                UISpawnTimeController.S.SetSpawnTimeClientRpc(90, player.ClientRpcParams);
            }
            else
            {
                UISpawnTimeController.S.SetSpawnTimeClientRpc(135, player.ClientRpcParams);
            }
        }

        StartCoroutine(DoSpawnLegion());
    }

    private IEnumerator DoSpawnLegion()
    {
        if (!IsServer) yield break;

        yield return _waitForSpawnCooldown;

        TimeUntilSpawn = SPAWN_COOLDOWN;

        for (int i = 0; i < PlayerSpawnData.Count; i++)
        {
            if (!PlayerSpawnData[i].UpNext || !PlayerSpawnData[i].IsActive) continue;

            Debug.Log($"Spawning for client: '{ PlayerSpawnData[i].ClientId} ', who has { PlayerSpawnData[i].UnitSpawnData.Count } units");

            foreach (var unitData in PlayerSpawnData[i].UnitSpawnData)
            {
                SpawnUnit(ref PlayerSpawnData[i].ClientId, ref PlayerSpawnData[i].Team, unitData);
            } 
        }

        if (Server.S.MatchType != MatchType.OneVSOne)
        {

            _cacheTeam1SpawnData = PlayerSpawnData.FindAll((spd) => spd.Team == 1);
            _cacheTeam2SpawnData = PlayerSpawnData.FindAll((spd) => spd.Team == 2);

            for (int i = 0; i < _cacheTeam1SpawnData.Count; i++)
            {
                if (_cacheTeam1SpawnData[i].IsActive == false) continue;

                if (_cacheTeam1SpawnData[i].UpNext == true)
                {
                    _cacheTeam1SpawnData[i].UpNext = false;

                    if (_cacheTeam1SpawnData.Count >= i)
                    {
                        _cacheTeam1SpawnData[0].UpNext = true;
                    }
                    else
                    {
                        _cacheTeam1SpawnData[i + 1].UpNext = true;
                    }
                    break;
                }
            }

            for (int i = 0; i < _cacheTeam2SpawnData.Count; i++)
            {
                if (_cacheTeam2SpawnData[i].IsActive == false) continue;

                if (_cacheTeam2SpawnData[i].UpNext == true)
                {
                    _cacheTeam2SpawnData[i].UpNext = false;

                    if (_cacheTeam2SpawnData.Count >= i)
                    {
                        _cacheTeam2SpawnData[0].UpNext = true;
                    }
                    else
                    {
                        _cacheTeam2SpawnData[i + 1].UpNext = true;
                    }
                    break;
                }
            }
        }

        _cacheTeam1SpawnData = null;
        _cacheTeam2SpawnData = null;

        StartCoroutine(DoSpawnLegion());
    }
    
    private void SpawnUnit(ref byte clientId, ref byte team, UnitSpawnData unitData)
    {
        if (team == 1)
        {
            _cacheSpawnPosition = _team1SpawnZone.position - unitData.LocalOffset;

            Unit unit = Instantiate(unitData.Unit, _cacheSpawnPosition, _team1SpawnZone.rotation);
            unit.LocalOwnerClientID = clientId;

            unit.NetworkObject.Spawn();

            unit.SpawnPosition.Value       = _cacheSpawnPosition;
            unit.CustomOwnerClientID.Value = clientId;
            unit.Team.Value                = team;
        }
        else
        {
            _cacheSpawnPosition = _team2SpawnZone.position - unitData.LocalOffset;

            Unit unit = Instantiate(unitData.Unit, _cacheSpawnPosition, _team2SpawnZone.rotation);
            unit.LocalOwnerClientID = clientId;

            unit.NetworkObject.Spawn();

            unit.SpawnPosition.Value       = _cacheSpawnPosition;
            unit.CustomOwnerClientID.Value = clientId;
            unit.Team.Value                = team;
        }
    }

    public void AddClientSpawnData(PlayerSpawnData clientSpawnData)
    {
        if (!IsServer) return;

        if (PlayerSpawnData.Count < 2) clientSpawnData.UpNext = true;

        PlayerSpawnData.Add(clientSpawnData);
    }

    public void SetPlayerSpawnDataActive(byte clientId, bool isActive)
    {
        if (!IsServer) return;

        var data = PlayerSpawnData.Find((csd) => csd.ClientId == clientId);

        if (data != null) data.IsActive = isActive;
    }

    public void AddUnit(byte clientID, Unit unit, Vector3 localOffset)
    {
        if (!IsServer) return;

        var psd = PlayerSpawnData.Find(c => c.ClientId == clientID);

        if (psd == null) return;

        psd.UnitSpawnData.Add(new UnitSpawnData(unit, localOffset));
    }

    public void RemoveUnit(byte clientID, Unit unit)
    {
        if (!IsServer) return;

        var psd = PlayerSpawnData.Find(c => c.ClientId == clientID);

        if (psd == null) return;

        psd.UnitSpawnData.Remove(psd.UnitSpawnData.Find(u => u.Unit == unit));
    }

    private void UpdateSpawnTimers()
    {

    }

    private List<PlayerSpawnData> GetActiveTeamSpawnData(byte team)
    {
        return PlayerSpawnData.FindAll((psd) => psd.IsActive && psd.Team == team);
    }

}

public class PlayerSpawnData
{
    public byte ClientId;
    public byte Team;
    public bool UpNext;
    public bool IsActive = true;
    public List<UnitSpawnData> UnitSpawnData = new();

    public PlayerSpawnData(byte clientId, byte team)
    {
        ClientId = clientId;
        Team = team;
    }
}

public class UnitSpawnData
{
    public Unit Unit;
    public Vector3 LocalOffset;

    public UnitSpawnData(Unit unit, Vector3 localOffset)
    {
        Unit = unit;
        LocalOffset = localOffset;
    }
}