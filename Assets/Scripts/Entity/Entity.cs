using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using DG.Tweening;

public class Entity : NetworkBehaviour
{

    [field: Header("Entity")]
    public Animator         Animator { get; private set; }
    public NetworkAnimator  NetworkAnimator { get; private set; }
    public EffectController EffectController { get; private set; } = new();

    private HUDHealthbarController _hudHealthbarController;

    [SerializeField]private Renderer _teamColorGO;

    [field:SerializeField] public int      ID     { get; private set; }
    [field:SerializeField] public byte     Tier   { get; private set; }
    [field:SerializeField] public Sprite   Icon   { get; private set; }
    [field:SerializeField] public bool     IsUnit { get; private set; }

    public byte LocalOwnerClientID;

    public const float RotationSpeed = 5;

    private const float _corpseDuration = 10f;

    public EntityType EntityType             { get; protected set; }
    public float CreationTime                { get; private set; }

    private WaitForSeconds _waitForAttackAnimationDelay;

    private float         _processDmgFactor;
    private float         _damage;
    private DamagePayload _dmgPayload;
    private Entity        _cacheEntity;
    private Collider[]    _cacheHits;

    public Action<Entity> OnDie;

    public Stats Stats { get; protected set; }

    [HideInInspector] public Entity Target;
    [HideInInspector] public Action<Entity> OnTargetChanged;

    public NetworkVariable<General> StatsGeneral { get; set; } = new();
    public NetworkVariable<Defense> StatsDefense { get; set; } = new();
    public NetworkVariable<Attack>  StatsAttack  { get; set; } = new();
    public NetworkVariable<Ability> StatsAbility { get; set; } = new();

    public NetworkVariable<byte>         CustomOwnerClientID { get; private set; } = new();
    public NetworkVariable<byte>         Team          { get; private set; } = new();
    public NetworkVariable<float>        Health        { get; private set; } = new();
    public NetworkVariable<bool>         IsDead        { get; private set; } = new();
    public NetworkVariable<bool>         IsFlying      { get; private set; } = new();
    public NetworkVariable<bool>         IsStunned     { get; private set; } = new();
    public NetworkVariable<bool>         IsStealthed   { get; private set; } = new();
    public NetworkVariable<Vector3>      SpawnPosition { get; private set; } = new();
    public NetworkVariable<float>        CurrentAttackRange { get; private set; } = new();

    protected virtual void Awake() 
    {
        if (TryGetComponent(out Animator animator))
        {
            Animator = animator;
        }
        else
        {
            Animator = GetComponentInChildren<Animator>();
        }

        if (TryGetComponent(out NetworkAnimator networkAnimator))
        {
            NetworkAnimator = networkAnimator;
        }
        else
        {
            NetworkAnimator = GetComponent<NetworkAnimator>();
        }
        
        _hudHealthbarController = GetComponent<HUDHealthbarController>();

        CreationTime = Time.time;

        SpawnPosition.OnValueChanged += OnSpawnPositionChanged;
        Team.OnValueChanged          += ChangeTeamMaterial;
        StatsAttack.OnValueChanged   += OnStatsAttackChanged;
    }

    private void OnStatsAttackChanged(Attack previousValue, Attack newValue)
    {
        _waitForAttackAnimationDelay = new WaitForSeconds(newValue.AnimationDelay);
    }

    private void OnSpawnPositionChanged(Vector3 previousValue, Vector3 newValue)
    {
        transform.position = newValue;
        SpawnPosition.OnValueChanged -= OnSpawnPositionChanged;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkObjectManager.S.Add(NetworkObject);

        if (SpawnPosition.Value != Vector3.zero) transform.position = SpawnPosition.Value;
        IsDead.OnValueChanged += OnIsDeadChanged;
        
        if (IsClient)
        {
            Health.OnValueChanged += OnHealthChangeCallback;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkObjectManager.S.Remove(NetworkObjectId);
        
        SpawnPosition.OnValueChanged -= OnSpawnPositionChanged;
        Team.OnValueChanged          -= ChangeTeamMaterial;
        StatsAttack.OnValueChanged   -= OnStatsAttackChanged;

        if (transform) transform.DOKill();
    }

    public virtual void InitStats() {}

    protected void InitializeNetworkVariables(Stats stats)
    {
        Health.Value       = stats.Defense.MaxHealth;
        StatsGeneral.Value = stats.General;
        StatsDefense.Value = stats.Defense;
        StatsAttack.Value  = stats.Attack;
        StatsAbility.Value = stats.Ability;
    }

    public void Attack(Entity target)
    {
        if (!IsServer) return;

        if (NetworkAnimator) NetworkAnimator.SetTrigger("attack");
        
        StartCoroutine(DoAttack(target));
    }

    protected virtual IEnumerator DoAttack(Entity target)
    {
        if (StatsAttack.Value.AnimationDelay > 0) yield return _waitForAttackAnimationDelay;

        if (IsDead.Value) yield break;

        _dmgPayload = new DamagePayload(StatsAttack.Value);
        _damage = _dmgPayload.Damage;

        if (_dmgPayload.CriticalChance > 0 && UnityEngine.Random.Range(0, 100) <= _dmgPayload.CriticalChance)
            _damage += _dmgPayload.CriticalDamage;

        if (StatsAttack.Value.IsSplash)
        {
            _cacheHits =  Physics.OverlapSphere(target.transform.position, StatsAttack.Value.SplashRadius, ReferenceManager.S.UnitMask | ReferenceManager.S.ObstacleMask);
            for (int i = 0; i < _cacheHits.Length; i++)
            {
                _cacheEntity = _cacheHits[i].GetComponent<Entity>();
                if (_cacheEntity == null || _cacheEntity.Team.Value != target.Team.Value) continue;

                _cacheEntity.TakeDamage(_dmgPayload, this);
            }
        }
        else
        {
            target.TakeDamage(_dmgPayload, this);
        }
    }

    public virtual void TakeDamage(DamagePayload dmgPayload, Entity attacker = null)
    {
        if (!IsSpawned || IsDead.Value) return;

        _damage = ProcessDamageTakenModifiers(dmgPayload);

        if (Health.Value - _damage <= 0)
        {
            Die(attacker);
            return;
        }

        if (!IsServer) return;

        if (_damage != 0)
        {
            //NetworkAnimator.SetTrigger("takeDamage");
            Health.Value -= _damage;
        }
    }

    private float ProcessDamageTakenModifiers(DamagePayload dmgPayload)
    {
        _damage = dmgPayload.Damage;
       
        _processDmgFactor = 0f;
        //if (_resistanceValues.TryGetValue(dmgPayload.Element, out sbyte resistance))
        //{
            //if (resistance != 0)
            //{
            //    _processDmgFactor += resistance / 100f;
            //}

            if (_processDmgFactor <= 0)
            {
                return _damage * (1f - _processDmgFactor);
            }

            if (dmgPayload.Penetration > 0)
            {
                _processDmgFactor -= dmgPayload.Penetration / 100f;
            }

            if (_processDmgFactor <= 0)
            {
                return _damage;
            }
        //}

        return _damage * (1f - _processDmgFactor);
    }

    public void Heal(int value)
    {
        if (Health.Value == StatsDefense.Value.MaxHealth) return;

        if (Health.Value + value > StatsDefense.Value.MaxHealth)
        {
            Health.Value = StatsDefense.Value.MaxHealth;
            return;
        }

        Health.Value += value;
    }

    private void OnIsDeadChanged(bool previousValue, bool newValue)
    {
        if (newValue == true)
        {
            Die();
            IsDead.OnValueChanged -= OnIsDeadChanged;
        }
    }

    protected virtual void Die(Entity killer = null)
    {
        if (IsServer) IsDead.Value = true;
        OnDie?.Invoke(this);

        Animator?.SetBool("die", true);

        _hudHealthbarController?.DisableHUD();

        if (gameObject.activeInHierarchy) 
            StartCoroutine(DoDie());
    }

    public void Resurrect(int healAmount)
    {
        Heal(healAmount);

        if (IsServer) IsDead.Value = false;

        Animator?.SetBool("die", false);
    }

    public bool IsScanStateCriteria()
    {
        if (!Target || Target.IsDead.Value || IsTargetOutOfRange())
        {
            return true;
        }

        return false;
    }

    public bool IsTargetWithinAttackRange()
    {
        if (transform.position.DistanceXZ(Target.transform.position) < CurrentAttackRange.Value)
        {
            return true;
        }

        return false;
    }

    public bool IsTargetOutOfRange()
    {
        if (transform.position.DistanceXZ(Target.transform.position) > StatsGeneral.Value.ScanRange)
        {
            return true;
        }

        return false;
    }

    public bool IsTargetOutOfAttackRange()
    {
        if (transform.position.DistanceXZ(Target.transform.position) > CurrentAttackRange.Value)
        {
            return true;
        }

        return false;
    }

    protected virtual IEnumerator DoDie()
    {
        yield return new WaitForSeconds(_corpseDuration);

        if (IsServer)
        {
            if (IsSpawned) NetworkObject.Despawn();
        }
        else
        {
            if (gameObject) gameObject.SetActive(false);
        }
    }

    public void ChangeTeamMaterial(byte prevTeamValue, byte newTeamValue)
    {
        if (!_teamColorGO) return;

        if (newTeamValue == 1)
        {
            if (_teamColorGO)
            {
                _teamColorGO.material = ReferenceManager.S.MaterialTeam1;
            }
        }
        else
        {
            if (_teamColorGO)
            {
                _teamColorGO.material = ReferenceManager.S.MaterialTeam2;
            }
        }
    }

    private void OnHealthChangeCallback(float previousValue, float newValue)
    {
        _hudHealthbarController?.UpdateHealthUI(newValue);
    }

    [ClientRpc]
    public void AddEffectClientRpc(byte effectIndex)
    {
        EffectController.Add(this, EffectManager.S.Get((Effects)effectIndex));
    }

    [ClientRpc]
    public void SetTargetClientRpc(ulong netObjId)
    {
        Target = NetworkObjectManager.S.GetEntity(netObjId);
    }

    [ClientRpc]
    public void SyncPositionClientRpc(Vector3 serverPosition, ClientRpcParams clientRpcParams = default)
    {
        transform.position = serverPosition;
        Physics.SyncTransforms();
    }

    [ServerRpc]
    public void SyncPositionServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(serverRpcParams);
        Debug.Log(serverRpcParams.Receive);
        Debug.Log(serverRpcParams.Send);
        if (serverRpcParams.Receive.SenderClientId == 0)
        {
            SyncPositionClientRpc(transform.position);
        }
        else
        {
            SyncPositionClientRpc(transform.position, Server.S.GetPlayer((byte)serverRpcParams.Receive.SenderClientId).ClientRpcParams);
        }
    }

}
