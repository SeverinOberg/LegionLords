public class IncomeBuilding : Building
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ServerResourceManager.S.IncreaseGoldPerSecond(OwnerClientId, BuildingSO.Income);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        ServerResourceManager.S.DecreaseGoldPerSecond(OwnerClientId, BuildingSO.Income);
    }

}
