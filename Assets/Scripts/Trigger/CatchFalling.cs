using UnityEngine;
using Unity.Netcode;

public class CatchFalling : NetworkBehaviour
{

    private Unit _unit;
    private LayerMask _groundLayerMask;

    private void Awake()
    {
        _groundLayerMask = LayerMask.GetMask("Ground");
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.TryGetComponent(out _unit)) return;

        if (IsServer)
        {
            Debug.LogWarning("CatchFalling: An entity fell through the ground. Attempting to move it back to ground above.");
            if (Physics.Raycast(_unit.transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 500, _groundLayerMask))
            {
                _unit.SyncPositionClientRpc(hit.point + Vector3.up * _unit.AIPath.height);
            }
            else
            {
                Debug.LogWarning("CatchFalling: Couldn't find ground above, returning the entity to spawn position.");
                _unit.SyncPositionClientRpc(_unit.SpawnPosition.Value);
            }
        }
        else
        {
            Debug.LogWarning("CatchFalling: An entity fell through the ground. Syncing it back to server position.");
            _unit.SyncPositionServerRpc();
        }

    }

}
