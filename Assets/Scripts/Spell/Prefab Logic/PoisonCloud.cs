using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem _poisonCloudPS;

    [SerializeField]
    private SpellSO _poisonCloudData;

    private float _timer = 2;
    private float cooldown = 2;
    private bool _isFinished;

    private Collider[] _hits;
    private Entity _entity;

    private void Awake()
    {
        StartCoroutine(StopEmittingAfter(_poisonCloudData.Duration)); 
    }

    private void Update()
    {
        if (_isFinished) return;

        _timer += Time.deltaTime;
        if (_timer > cooldown)
        {
            _timer = 0;
            AddPoisonEffect();
        }
    }

    private void AddPoisonEffect()
    {
        _hits = Physics.OverlapSphere(transform.position, _poisonCloudData.Radius, ReferenceManager.S.UnitMask);

        for (int i = 0; i < _hits.Length; i++)
        {
            if (_hits[i].TryGetComponent(out _entity))
            {
                if (!NetworkManager.Singleton.IsServer) continue;
                _entity.AddEffectClientRpc((byte)Effects.PoisonCloud);
            }
        }
    }

    private IEnumerator StopEmittingAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        _isFinished = true;
        _poisonCloudPS.Stop();

        yield return new WaitForSeconds(5);

        if (NetworkManager.Singleton.IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

}
