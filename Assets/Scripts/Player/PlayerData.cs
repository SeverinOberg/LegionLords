using Unity.Netcode;

public class PlayerData
{

    public byte ClientID { get; private set; }
    public byte Team     { get; private set; }
    public byte SpawnRow { get; private set; }

    public NetworkObject   PlayerPrefab       { get; private set; }
    public NetworkObject   BuildingZonePrefab { get; private set; }
    public ClientRpcParams ClientRpcParams  { get; private set; }

    // + Controllers +
    public Player             Player             { get; private set; }
    public InputController    InputController    { get; private set; }
    public ResourceController ResourceController { get; private set; }
    public BuildingController BuildingController { get; private set; }
    public UpgradeController  UpgradeController  { get; private set; }
    public SpellController    SpellController    { get; private set; }
    //-  Controllers -

    public PlayerData(byte clientID, byte team, NetworkObject playerObject, NetworkObject buildingZonePrefab, Player player, InputController inputController, ResourceController resourceController, BuildingController buildingController, UpgradeController upgradeController, SpellController spellController)
    {
        ClientID = clientID;
        Team = team;
        PlayerPrefab = playerObject;
        BuildingZonePrefab = buildingZonePrefab;
        ClientRpcParams = new ClientRpcParams { Send = { TargetClientIds = new ulong[] { clientID } } };
        Player = player;
        InputController = inputController;
        ResourceController = resourceController;
        BuildingController = buildingController;
        UpgradeController = upgradeController;
        SpellController = spellController;
    }
}
