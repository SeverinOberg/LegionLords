using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class InitializeMatch : NetworkBehaviour
{

    [SerializeField] private Transform _team1SpawnZone;
    [SerializeField] private Transform _team2SpawnZone;

    [SerializeField] private Entity _team1Base;
    [SerializeField] private Entity _team1Turret;

    [SerializeField] private Entity _team2Base;
    [SerializeField] private Entity _team2Turret;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) 
            StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;

        _team1Base.Team.Value   = 1;
        _team1Turret.Team.Value = 1;

        _team2Base.Team.Value   = 2;
        _team2Turret.Team.Value = 2;

        foreach (var player in Server.S.Players)
        {
            ServerResourceManager.S.AddClientResourceData(new ServerResourceManager.ClientResourceData(player.ClientID.Value, player.Team.Value, player.ResourceController));
            ServerSpawnManager.S.AddClientSpawnData(new PlayerSpawnData(player.ClientID.Value, player.Team.Value));
        }

        ServerSpawnManager.S.Run(_team1SpawnZone, _team2SpawnZone);
        InitClientRPC();
    }

    [ClientRpc]
    private void InitClientRPC()
    {
        Player.S?.EnableAllPlayerScripts();
        Player.S?.CameraController.Init(Player.S.SpawnPosition.Value);

        Destroy(gameObject, 10);
    }

}
