using System;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{

    /// <summary>Singleton</summary>
    public static Player S   { get; private set; }

    public uint    SteamID   { get; private set; }
    public int     AccountID { get; private set; }
    public string  SteamName { get; private set; }

    public int[] SpawnersIDArray;
    public int[] UnitsIDArray;

    public bool IsAccountDataSet { get; private set; }

    public ClientRpcParams ClientRpcParams = new();

    [HideInInspector] public NetworkObject IslandNetObj;

    [HideInInspector] public NetworkVariable<byte>    ClientID   = new();
    [HideInInspector] public NetworkVariable<byte>    Team       = new();
    [HideInInspector] public NetworkVariable<byte>    SpawnRow   = new();
    [HideInInspector] public NetworkVariable<Vector3> SpawnPosition = new();
    [HideInInspector] public NetworkVariable<Vector3> SpawnRotation = new();
    
    public Camera             Camera             { get; private set; }
    public AudioListener      AudioListener      { get; private set; }

    public InputController    InputController    { get; private set; }
    public CameraController   CameraController   { get; private set; }
    public ResourceController ResourceController { get; private set; }
    public BuildingController BuildingController { get; private set; }
    public UpgradeController  UpgradeController  { get; private set; }
    public SpellController    SpellController    { get; private set; }

    public UnitsManager UnitsManager;

    public float SpawnTime;
    public float SpawnTimeThreshold;

    private void Awake()
    {
        InitComponents();
        SpawnPosition.OnValueChanged += OnSpawnPositionChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;
        
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        SpawnPosition.OnValueChanged -= OnSpawnPositionChanged;
    }

    private void Update()
    {
        HandleSpawnTimer();
    }

    private void HandleSpawnTimer()
    {
        SpawnTime -= Time.deltaTime;

        if (SpawnTime <= 0)
        {
            SpawnTime = SpawnTimeThreshold;
        }
    }

    public void InitComponents()
    {
        Camera             = transform.GetComponentInChildren<Camera>(includeInactive:true);
        AudioListener      = transform.GetComponentInChildren<AudioListener>(includeInactive:true);

        InputController    = transform.GetComponent<InputController>();
        CameraController   = transform.GetComponent<CameraController>();
        ResourceController = transform.GetComponent<ResourceController>();
        BuildingController = transform.GetComponent<BuildingController>();
        UpgradeController  = transform.GetComponent<UpgradeController>();
        SpellController    = transform.GetComponent<SpellController>();
    }

    public void SetAccountData(uint steamID, int accountID, string steamName)
    {
        SteamID   = steamID;
        AccountID = accountID;
        SteamName = steamName;
        IsAccountDataSet = true;
    }

    public void SetPlayerData(byte team, byte spawnRow, Vector3 spawnPosition, Vector3 spawnRotation)
    {
        Team.Value          = team;
        SpawnRow.Value      = spawnRow;
        SpawnPosition.Value = spawnPosition;
        SpawnRotation.Value = spawnRotation;
    }

    public void SetClientData(ulong clientID)
    {
        ClientID.Value = (byte)clientID;
        ClientRpcParams = new ClientRpcParams { Send = { TargetClientIds = new ulong[] { clientID } } };
    }

    public void EnableAllPlayerScripts()
    {
        Camera.enabled             = true;
        AudioListener.enabled      = true;
        InputController.enabled    = true;
        CameraController.enabled   = true;
        ResourceController.enabled = true;
        BuildingController.enabled = true;
        UpgradeController.enabled  = true;
        SpellController.enabled    = true;
    }


    public void SetSpawnTimer(byte time)
    {
        UISpawnTimeController.S.SetTimer(time);
    }

    #region Callbacks

    private void OnSpawnPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        transform.position = newValue;
        CameraController?.Init(newValue);
    }

    #endregion

    #region ClientRpc

    #endregion

    #region ServerRpc

    [ServerRpc]
    public void DespawnServerRpc(ulong networkObjectId)
    {
        NetworkManager.SpawnManager.SpawnedObjects[networkObjectId].Despawn();
    }

    [ServerRpc]
    public void SpawnServerRpc(ulong networkObjectId, byte clientId, bool withOwnership = true)
    {
        if (withOwnership)
        {
            NetworkManager.SpawnManager.GetClientOwnedObjects(clientId).Find((obj) => obj.NetworkObjectId == networkObjectId).SpawnWithOwnership(clientId);
        }
        else
        {
            NetworkManager.SpawnManager.GetClientOwnedObjects(clientId).Find((obj) => obj.NetworkObjectId == networkObjectId).Spawn();
        }
    }

    [ServerRpc]
    public void SetClientAccountDataServerRpc(uint steamID, int accountID, string steamName)
    {
        SetAccountData(steamID, accountID, steamName);
    }

    [ServerRpc]
    public void SetClientLegionDataServerRpc(int[] legionUnitArray)
    {
        var units = EntityDatabase.S.GetBatch(legionUnitArray);

        int[] legionBuildingsIDArray = new int[15];

        for (int i = 0; i < units.Count; i++)
        {
            legionBuildingsIDArray[i] = units[i].GetComponent<Unit>().Spawner.ID;
        }

        SpawnersIDArray = legionBuildingsIDArray;
        UnitsIDArray = legionUnitArray;
    }

    #endregion
}
