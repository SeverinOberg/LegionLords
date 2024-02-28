
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GoldVeinManager : NetworkBehaviour
{

    public static GoldVeinManager S;

    [SerializeField]
    private GameObject _goldVeinPrefab;
    [SerializeField]
    private float RespawnCooldown = 60;
    [SerializeField]
    private GameObject[] _goldVeinsSpawns;

    public System.Action OnGoldVeinMined;

    private LayerMask _resourceLayerMask;
    private WaitForSeconds _cacheWaitForSeconds;
    private Collider[] _hits;
    private GameObject _goldVein;

    private int _randomIndex;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
        {
            Destroy(gameObject);
            return;
        }

        _resourceLayerMask = LayerMask.GetMask("Resource");
        _cacheWaitForSeconds = new WaitForSeconds(RespawnCooldown);

        OnGoldVeinMined += OnGoldVeinMinedCallback;

        StartCoroutine(InitializeStartGoldVeins());
    }    

    private IEnumerator InitializeStartGoldVeins()
    {
        yield return new WaitUntil(() => NetworkObjectManager.S != null);
        yield return new WaitForSeconds(15);

        for (int i = 0; i < _goldVeinsSpawns.Length / 5; i++)
        {
            SpawnRandomGoldVein();
        }
    }

    private void OnGoldVeinMinedCallback()
    {
        StartCoroutine(StartRandomGoldVeinRespawn());
    }

    private IEnumerator StartRandomGoldVeinRespawn()
    {
        yield return _cacheWaitForSeconds;
        SpawnRandomGoldVein();
    }

    private void SpawnRandomGoldVein()
    {
        while (true)
        {
            _randomIndex = Random.Range(0, _goldVeinsSpawns.Length);
            _hits = Physics.OverlapSphere(_goldVeinsSpawns[_randomIndex].transform.position, 7, _resourceLayerMask);
            if (_hits.Length > 0) continue;

            _goldVein = Instantiate(_goldVeinPrefab, _goldVeinsSpawns[_randomIndex].transform.position - Vector3.up * 2, Quaternion.identity);
            _goldVein.GetComponent<NetworkObject>().Spawn();
            break;
        }
   
    }

}
