using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class BuildingController : NetworkBehaviour
{

    [SerializeField] private Material _buildOKMat;
    [SerializeField] private Material _buildErrorMat;

    private Player _player;

    public NetworkList<int> SpawnerIDList;
    public List<Spawner> SpawnerPrefabs { get; private set; } = new();
    public BuildingPlaceable BuildingToPlace { get; private set; }
    private byte _currentBuildingPrefabIndex;

    private const float _regretBuildSeconds = 10;

    private void Awake()
    {
        _player = GetComponent<Player>();
        SpawnerIDList = new();
    }

    private void Update()
    {
        if (BuildingToPlace == null || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        PlacementColorValidation();

        if (InputController.InputActions.Player.Primary.WasPressedThisFrame())
        {
            TryPlaceBuilding();
        }
        else if (InputController.InputActions.Player.Secondary.WasPressedThisFrame())
        {
            CancelBuildingPlacement();
        }
    }

    public void Init(int[] buildingIDArray)
    {
        var buildingEntities = EntityDatabase.S.GetBatch(buildingIDArray);
        foreach (var entity in buildingEntities)
        {
            SpawnerPrefabs.Add(entity.GetComponent<Spawner>());
        }

        if (IsServer)
        {
            for (int i = 0; i < buildingIDArray.Length; i++)
            {
                SpawnerIDList.Add(buildingIDArray[i]);
            }
        } 
    }

    private void PlacementColorValidation()
    {
        if (BuildingSystem.S.CanBePlaced(BuildingToPlace, _player.Team.Value))
        {
            if (BuildingToPlace.Renderer.material != _buildOKMat)
                BuildingToPlace.Renderer.material = _buildOKMat;
        }
        else
        {
            if (BuildingToPlace.Renderer.material != _buildErrorMat)
                BuildingToPlace.Renderer.material = _buildErrorMat;
        }
    }

    public void TryPlaceBuilding()
    {
        Spawner spawner = SpawnerPrefabs[_currentBuildingPrefabIndex];

        if (!Player.S.ResourceController.CanBuy(spawner.BuildingSO.Cost))
        {
            return;
        }

        if (BuildingSystem.S.CanBePlaced(BuildingToPlace, _player.Team.Value))
        {

            BuildingToPlace.Place();
            Vector3Int start = BuildingSystem.S.GridLayout.WorldToCell(BuildingToPlace.GetStartPosition());
            BuildingSystem.S.TakeArea(start, BuildingToPlace.Size);

            SpawnServerRpc((byte)OwnerClientId, _currentBuildingPrefabIndex, BuildingToPlace.transform.position);

            Player.S.ResourceController.DecrGold(spawner.BuildingSO.Cost);

            CancelBuildingPlacement();

            if (InputController.Shift)
            {
                InitializeBuilding(_currentBuildingPrefabIndex);
            }
        }
        else
        {
            AnnouncementManager.S.Announce("CAN'T BUILD THERE");
        }
    }

    public void CancelBuildingPlacement()
    {
        if (!BuildingToPlace) return;

        Destroy(BuildingToPlace.gameObject);
        BuildingToPlace = null;
    }

    public void InitializeBuilding(byte index)
    {
        if (BuildingToPlace != null) return;

        Building prefab = SpawnerPrefabs[index];

        if (!Player.S.ResourceController.CanBuy(prefab.BuildingSO.Cost))
        {
            return;
        }

        _currentBuildingPrefabIndex = index;

        Vector3 position = BuildingSystem.S.SnapCoordianteToGrid(Vector3.zero);

        GameObject go = Instantiate(prefab.gameObject, position, Quaternion.identity);
        var building = go.GetComponent<Building>();
        building.Collider.enabled = false;
        building.enabled = false;
        BuildingToPlace = building.GetComponent<BuildingPlaceable>();

        go.AddComponent<BuildingDrag>();
    }


    public void Sell(Spawner spawner)
    {
        if (Time.time - spawner.CreationTime < _regretBuildSeconds)
        {
            Player.S.ResourceController.IncrGold(spawner.BuildingSO.Cost);
        }
        else
        {
            Player.S.ResourceController.IncrGold(spawner.BuildingSO.Cost / 2);
        }

        BuildingPlaceable placeableBuilding = spawner.GetComponent<BuildingPlaceable>();
        Vector3Int start = BuildingSystem.S.GridLayout.WorldToCell(placeableBuilding.GetStartPosition());
        BuildingSystem.S.ClearArea(start, placeableBuilding.Size);

        SelectionManager.S.ClearSelection();

        _player.DespawnServerRpc(spawner.NetworkObject.NetworkObjectId);

        if (IsServer)
        {
            ServerSpawnManager.S.RemoveUnit(_player.ClientID.Value, spawner.UnitToSpawn);
        }
        
    }

    [ServerRpc]
    private void SpawnServerRpc(byte clientID, byte prefabIndex, Vector3 position)
    {
        Spawner spawner = Instantiate(SpawnerPrefabs[prefabIndex], position, Quaternion.identity);

        spawner.NetworkObject.SpawnWithOwnership(clientId: clientID, destroyWithScene: true);

        Player player = Server.S.GetPlayer(clientID);
        spawner.Team.Value = player.Team.Value;

        spawner.Initialize(spawner.BuildingSO.UnitNetworkObject.GetComponent<Unit>());
        ServerSpawnManager.S.AddUnit(clientID, spawner.UnitToSpawn, player.SpawnPosition.Value - new Vector3(position.x, 0, position.z));
    }
}
