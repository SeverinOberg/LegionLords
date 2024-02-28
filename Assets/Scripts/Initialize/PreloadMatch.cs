using UnityEngine;
using System.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PreloadMatch : NetworkBehaviour
{

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerIslandPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        MapData mapData = MapDatabase.S.Get(Map.DesertIsland);

        foreach (var client in NetworkManager.ConnectedClientsList)
        {
            Player player = Instantiate(_playerPrefab).GetComponent<Player>();

            player.NetworkObject.SpawnAsPlayerObject(client.ClientId);
            player.NetworkObject.ActiveSceneSynchronization = true;

            Server.S.Players.Add(player);

            player.InitComponents();

            player.SetClientData(client.ClientId);

            RequestClientAccountDataClientRpc(player.ClientRpcParams);

            yield return new WaitUntil(() => player.IsAccountDataSet);

            RequestClientLegionIDArrayClientRpc(player.ClientRpcParams);

            yield return new WaitUntil(() => player.SpawnersIDArray.Length > 0);

            SetClientSpawnersClientRpc(player.SpawnersIDArray, player.ClientRpcParams);

            player.BuildingController.Init(player.SpawnersIDArray);

            Dictionary<int, Unit> unitManagerInitData = new();

            foreach (var unitID in player.UnitsIDArray)
            {
                Unit unit = EntityDatabase.S.Get(unitID).GetComponent<Unit>();
                unit.InitStats();
                unitManagerInitData.Add(unitID, unit);
            }

            player.UnitsManager = new UnitsManager(unitManagerInitData);
        }

        SetPlayerData(mapData);

        SpawnPlayerIslands();

        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(Refs.SceneNames.Battlefield, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.Log($"ERROR: Failed to load new scene, returning to Menu. Status: {status}");
            SceneManager.LoadScene(Refs.SceneNames.Menu);
        }
    }

    private void SetPlayerData(MapData mapData)
    {
        var players = Server.S.Players;

        if (players.Count == 2)
        {
            players[0].SetPlayerData(1, 1, mapData.SpawnPositions.OneVSOne_Team1_Position1, mapData.SpawnRotations.OneVSOne_Team1_Rotation1);
            players[1].SetPlayerData(2, 1, mapData.SpawnPositions.OneVSOne_Team2_Position1, mapData.SpawnRotations.OneVSOne_Team2_Rotation1);
        }
        else if (players.Count == 4)
        {
            players[0].SetPlayerData(1, 1, mapData.SpawnPositions.TwoVSTwo_Team1_Position1, mapData.SpawnRotations.TwoVSTwo_Team1_Rotation1);
            players[1].SetPlayerData(1, 2, mapData.SpawnPositions.TwoVSTwo_Team1_Position2, mapData.SpawnRotations.TwoVSTwo_Team1_Rotation2);

            players[2].SetPlayerData(2, 1, mapData.SpawnPositions.TwoVSTwo_Team2_Position1, mapData.SpawnRotations.TwoVSTwo_Team2_Rotation1);
            players[3].SetPlayerData(2, 2, mapData.SpawnPositions.TwoVSTwo_Team2_Position2, mapData.SpawnRotations.TwoVSTwo_Team2_Rotation2);
        }
        else
        {
            players[0].SetPlayerData(1, 1, mapData.SpawnPositions.ThreeVSThree_Team1_Position1, mapData.SpawnRotations.ThreeVSThree_Team1_Rotation1);
            players[1].SetPlayerData(1, 2, mapData.SpawnPositions.ThreeVSThree_Team1_Position2, mapData.SpawnRotations.ThreeVSThree_Team1_Rotation2);
            players[2].SetPlayerData(1, 3, mapData.SpawnPositions.ThreeVSThree_Team1_Position3, mapData.SpawnRotations.ThreeVSThree_Team1_Rotation3);

            players[3].SetPlayerData(2, 1, mapData.SpawnPositions.ThreeVSThree_Team2_Position1, mapData.SpawnRotations.ThreeVSThree_Team2_Rotation1);
            players[4].SetPlayerData(2, 2, mapData.SpawnPositions.ThreeVSThree_Team2_Position2, mapData.SpawnRotations.ThreeVSThree_Team2_Rotation2);
            players[5].SetPlayerData(2, 3, mapData.SpawnPositions.ThreeVSThree_Team2_Position3, mapData.SpawnRotations.ThreeVSThree_Team2_Rotation3);
        }
    }

    private void SpawnPlayerIslands()
    {
        var players = Server.S.Players;

        if (players.Count == 2)
        {
            NetworkObject islandNetObj0 = Instantiate(_playerIslandPrefab, players[0].SpawnPosition.Value, Quaternion.Euler(players[0].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[0].IslandNetObj = islandNetObj0;
            islandNetObj0.SpawnWithOwnership(players[0].ClientID.Value);
            islandNetObj0.ActiveSceneSynchronization = true;

            NetworkObject islandNetObj1 = Instantiate(_playerIslandPrefab, players[1].SpawnPosition.Value, Quaternion.Euler(players[1].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[1].IslandNetObj = islandNetObj1;
            islandNetObj1.SpawnWithOwnership(players[1].ClientID.Value);
            islandNetObj1.ActiveSceneSynchronization = true;
        }
        else if (players.Count == 4)
        {
            NetworkObject islandNetObj0 = Instantiate(_playerIslandPrefab, players[0].SpawnPosition.Value, Quaternion.Euler(players[0].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[0].IslandNetObj = islandNetObj0;
            islandNetObj0.SpawnWithOwnership(players[0].ClientID.Value);
            islandNetObj0.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj1 = Instantiate(_playerIslandPrefab, players[1].SpawnPosition.Value, Quaternion.Euler(players[1].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[1].IslandNetObj = islandNetObj1;
            islandNetObj1.SpawnWithOwnership(players[1].ClientID.Value);
            islandNetObj1.ActiveSceneSynchronization = true;
            
            NetworkObject islandNetObj2 = Instantiate(_playerIslandPrefab, players[2].SpawnPosition.Value, Quaternion.Euler(players[2].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[2].IslandNetObj = islandNetObj2;
            islandNetObj2.SpawnWithOwnership(players[2].ClientID.Value);
            islandNetObj2.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj3 = Instantiate(_playerIslandPrefab, players[3].SpawnPosition.Value, Quaternion.Euler(players[3].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[3].IslandNetObj = islandNetObj3;
            islandNetObj3.SpawnWithOwnership(players[3].ClientID.Value);
            islandNetObj3.ActiveSceneSynchronization = true;
        }
        else
        {
            NetworkObject islandNetObj0 = Instantiate(_playerIslandPrefab, players[0].SpawnPosition.Value, Quaternion.Euler(players[0].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[0].IslandNetObj = islandNetObj0;
            islandNetObj0.SpawnWithOwnership(players[0].ClientID.Value);
            islandNetObj0.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj1 = Instantiate(_playerIslandPrefab, players[1].SpawnPosition.Value, Quaternion.Euler(players[1].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[1].IslandNetObj = islandNetObj1;
            islandNetObj1.SpawnWithOwnership(players[1].ClientID.Value);
            islandNetObj1.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj2 = Instantiate(_playerIslandPrefab, players[2].SpawnPosition.Value, Quaternion.Euler(players[2].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[2].IslandNetObj = islandNetObj2;
            islandNetObj2.SpawnWithOwnership(players[2].ClientID.Value);
            islandNetObj2.ActiveSceneSynchronization = true;

            NetworkObject islandNetObj3 = Instantiate(_playerIslandPrefab, players[3].SpawnPosition.Value, Quaternion.Euler(players[3].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[3].IslandNetObj = islandNetObj3;
            islandNetObj3.SpawnWithOwnership(players[3].ClientID.Value);
            islandNetObj3.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj4 = Instantiate(_playerIslandPrefab, players[4].SpawnPosition.Value, Quaternion.Euler(players[4].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[4].IslandNetObj = islandNetObj4;
            islandNetObj4.SpawnWithOwnership(players[4].ClientID.Value);
            islandNetObj4.ActiveSceneSynchronization = true;
            NetworkObject islandNetObj5 = Instantiate(_playerIslandPrefab, players[5].SpawnPosition.Value, Quaternion.Euler(players[5].SpawnRotation.Value)).GetComponent<NetworkObject>();
            players[5].IslandNetObj = islandNetObj5;
            islandNetObj5.SpawnWithOwnership(players[5].ClientID.Value);
            islandNetObj5.ActiveSceneSynchronization = true;
        }
    }

    [ClientRpc]
    private void RequestClientAccountDataClientRpc(ClientRpcParams clientRpcParams)
    {
        Player.S.SetClientAccountDataServerRpc(Client.S.SteamID.AccountId, Client.S.AccountID, Client.S.SteamName);
    }

    [ClientRpc]
    private void RequestClientLegionIDArrayClientRpc(ClientRpcParams clientRpcParams)
    {   
        var legion = Client.S.LegionManager.GetActiveLegion();

        List<int> accountUnitsIDList = new();
        foreach (var data in legion.Data)
        {
            for (int i = 0; i < data.Units.Length; i++)
            {
                if (data.Units[i] == null) continue;
                accountUnitsIDList.Add(data.Units[i].ID);
            }
        }

        Player.S.SetClientLegionDataServerRpc(accountUnitsIDList.ToArray());
    }

    [ClientRpc]
    private void SetClientSpawnersClientRpc(int[] spawnerIDs, ClientRpcParams clientRpcParams)
    {
        Player.S.BuildingController.Init(spawnerIDs);
    }

}
