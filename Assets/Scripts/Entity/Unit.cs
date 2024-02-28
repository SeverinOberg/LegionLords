using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Pathfinding;

public class Unit : Entity
{

    public AIPath AIPath                           { get; private set; }
    public AIDestinationSetter AIDestinationSetter { get; private set; }
    public NetworkTransform NetworkTransform       { get; private set; }

    [field:SerializeField] public Spawner Spawner { get; private set; }

    [HideInInspector] public float AttackTimer;
    [HideInInspector] public float AbilityTimer;

    private Vector3    _cacheDirection;
    private Quaternion _cacheTargetRotation;
    private Quaternion _cacheRotation;

    private bool _canMove = true;
    public bool CanMove 
    { 
        get 
        {
            return _canMove;
        } 
        set 
        { 
            _canMove = value;
            AIPath.canMove = value;
        } 
    }
    
    protected override void Awake()
    {
        base.Awake();

        AIPath              = GetComponent<AIPath>();
        AIDestinationSetter = GetComponent<AIDestinationSetter>();
        NetworkTransform    = GetComponent<NetworkTransform>();
        EntityType          = EntityType.Unit;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            AIPath.maxSpeed = Stats.General.MovementSpeed;
        }
        else
        {
            AIPath.maxSpeed = Stats.General.MovementSpeed + 0.05f;
        }

        StatsGeneral.OnValueChanged += OnStatsGeneralChanged;
        CurrentAttackRange.OnValueChanged += OnCurrentAttackRangeChanged;

        if (!IsServer) return;

        Player playerOwner = Server.S.GetPlayer(LocalOwnerClientID);
        InitializeNetworkVariables(playerOwner.UnitsManager.Get(ID).Stats);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        StatsGeneral.OnValueChanged -= OnStatsGeneralChanged;
        CurrentAttackRange.OnValueChanged -= OnCurrentAttackRangeChanged;
    }

    protected virtual void Update()
    {
        AbilityTimer += Time.deltaTime;
        AttackTimer  += Time.deltaTime;

        if (CanMove)
            Animator.SetFloat("movement", AIPath.velocity.normalized.magnitude);
        else
            Animator.SetFloat("movement", 0);
        

        HandleRotationDirection();
    }

    private void OnStatsGeneralChanged(General previousValue, General newValue)
    {
        if (IsServer)
        {
            AIPath.maxSpeed = Stats.General.MovementSpeed;
        }
        else
        {
            AIPath.maxSpeed = Stats.General.MovementSpeed + 0.05f;
        }
    }

    private void OnCurrentAttackRangeChanged(float previousCurrentAttackRange, float newCurrentAttackRange)
    {
        AIPath.endReachedDistance = newCurrentAttackRange;
    }

    protected override void Die(Entity killer = null)
    {
        base.Die(killer);

        AIDestinationSetter.target = null;
        Target = null;

        AIPath.enabled = false;
        AIDestinationSetter.enabled = false;
    }

    private void HandleRotationDirection()
    {
        if (IsDead.Value || AIDestinationSetter.target == null) return;
        
        _cacheDirection = (AIDestinationSetter.target.transform.position - transform.position).normalized;
        _cacheDirection.y = 0;

        _cacheTargetRotation = Quaternion.LookRotation(_cacheDirection);
        _cacheRotation = Quaternion.Euler(transform.eulerAngles.x, _cacheTargetRotation.eulerAngles.y, transform.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, _cacheRotation, RotationSpeed * Time.deltaTime);
    }

    [ClientRpc]
    public void SetDestinationTargetClientRpc(ulong netObjId)
    {
        Entity target = NetworkObjectManager.S.GetEntity(netObjId);
        AIDestinationSetter.target = target.transform;
    }

    [ClientRpc]
    public void SetDestinationTargetAndTargetClientRpc(ulong netObjId)
    {
        Entity target = NetworkObjectManager.S.GetEntity(netObjId);
        AIDestinationSetter.target = target.transform;
        Target = target;
    }

}