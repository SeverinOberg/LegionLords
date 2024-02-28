using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class Resource : Entity
{

    [Header("Resource")]
    [SerializeField]
    private GameObject _harvestEffectPrefab;

    [SerializeField]
    private HUDFloatingText _hudFloatingText;

    private const int _reward = 30;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        transform.DOMove(transform.position + Vector3.up * 2, 3);
        transform.DOShakeRotation(3, 30);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        transform.DOKill();
        Instantiate(_harvestEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
    }

    public override void TakeDamage(DamagePayload dmgPayload, Entity attacker)
    {
        if (!IsSpawned) return;

        if (Health.Value <= 1)
        {
            Die(attacker);
            return;
        }

        Health.Value -= 1;
    }

    protected override void Die(Entity killer)
    {
        if (killer != null)
        {
            SpawnFloatingTextClientRpc(Server.S.GetPlayer((byte)killer.OwnerClientId).ClientRpcParams);
            ServerResourceManager.S.AddGoldToClient(killer.OwnerClientId, _reward);
        }

        GoldVeinManager.S.OnGoldVeinMined?.Invoke();
        NetworkObject.Despawn(destroy: true);
    }

    [ClientRpc]
    private void SpawnFloatingTextClientRpc(ClientRpcParams clientRpcParams)
    {
        var floatingText = Instantiate(_hudFloatingText, transform.position + Vector3.up * 3, Quaternion.identity);
        floatingText.Text.text = $"+{_reward}";
    }

}
