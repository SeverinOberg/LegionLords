using System.Collections;
using UnityEngine;

public class RunebornSentinel : UnitStateManager
{

    [Header("RunebornSentinel")]
    [SerializeField]
    private ParticleSystem _revealStealthPS;
    

    private float _timer;

    private Collider[] _hits;
    private Entity _entity;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        EngageState = new();
    }

    protected override void Update()
    {
        base.Update();

        _timer += Time.deltaTime;
        if (_timer > StatsAttack.Value.Speed)
        {
            _timer = 0;

            StartCoroutine(CastRevealStealth());
        }
    }

    private IEnumerator CastRevealStealth()
    {
        Animator.SetTrigger("cast");

        yield return new WaitForSeconds(StatsAttack.Value.AnimationDelay);

        _revealStealthPS.Play();

        if (!IsServer) yield break;

        _hits = Physics.OverlapSphere(transform.position, StatsGeneral.Value.ScanRange, ReferenceManager.S.UnitMask);

        if (_hits.Length == 0) yield break;

        for (int i = 0; i < _hits.Length; i++)
        {
            if (_hits[i].TryGetComponent(out _entity))
            {
                if (_entity.Team.Value == Team.Value) continue;

                if (_entity.IsStealthed.Value == true)
                {
                    _entity.IsStealthed.Value = false;
                }
            }
        }

    }

}
