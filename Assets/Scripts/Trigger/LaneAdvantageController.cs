using Unity.Netcode;
using UnityEngine;

public class LaneAdvantageController : MonoBehaviour
{

    [SerializeField] private byte _team;

    private Entity _cacheEntity;

    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.TryGetComponent(out _cacheEntity))
        {
            if (_cacheEntity.Team.Value == _team)
            {
                LaneAdvantageManager.S.Add(_cacheEntity);
            }
        }
    }

}
