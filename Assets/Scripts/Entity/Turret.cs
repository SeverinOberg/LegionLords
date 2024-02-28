using System.Collections;
using UnityEngine;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;

public class Turret : Building
{

    public enum States
    {
        Scan,
        Attack,
    }

    [SerializeField]
    private HUDFloatingText _hudFloatingText;

    [SerializeField] 
    private Projectile _projectilePrefab;
    private Transform _projectileSpawnPoint;

    private States _state = States.Scan;
    [SerializeField] 
    private int _destroyGoldReward;

    private Entity _cacheTarget;
    private Entity _closestTarget;
    private float _scanCooldown = 1;
    private float _scanTimer;
    private float _attackTimer;

    private Collider[] _hits;

    protected override void Awake()
    {

        base.Awake();

        Stats = new Stats
        {
            General = new General
            (
                id: 1,
                unitType: UnitType.Unique,
                role: RoleType.None,
                faction: Faction.None,
                name: "Turret",
                hitBoxSize: 5,
                scanRange: 45,
                movementSpeed: 0f
            ),
            Defense = new Defense
            (
                maxHealth: 300,
                maxShield: 100,
                armor: 0,
                dodgeChance: 0,
                resistances: new Resistances
                (
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    poison: 0,
                    nature: 0,
                    holy: 0,
                    shadow: 0,
                    arcane: 0
                )
            ),
            Attack = new Attack
            (
                element: Element.None,
                altitude: AltitudeType.All,
                hitChance: 0,
                damage: 8,
                criticalChance: 0,
                criticalDamageMultiplier: 0,
                penetration: 50,
                speed: 0.3f,
                range: 45,
                isRanged: true,
                rangedCollisionDistance: 0.2f,
                isSplash: false,
                splashRadius: 0,
                travelSpeed: 0.5f,
                animationDelay: 0
            )
        };

        EntityType = EntityType.Defense;
        _projectileSpawnPoint = transform.Find("Projectile Spawn Point");
 
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer) return;

        InitializeNetworkVariables(Stats);
    }

    private void Update()
    {
        if (IsDead.Value) return;

        if (_state == States.Scan)
        {
            Scan();
        }
        else
        {
            Attack();
        }
    }

    private void Scan()
    {
        _scanTimer += Time.deltaTime;
        if (_scanTimer < _scanCooldown) return;
        _scanTimer = 0;

        _hits = Physics.OverlapSphere(transform.position, StatsAttack.Value.Range, ReferenceManager.S.UnitMask);

        if (_hits.Length == 0) return;

        _cacheTarget   = null;
        _closestTarget = null;

        for (int i = 0; i < _hits.Length; i++)
        {
            _cacheTarget = _hits[i].GetComponent<Entity>();

            if (_cacheTarget == null || _cacheTarget.IsDead.Value || _cacheTarget.Team.Value == 0 || _cacheTarget.Team.Value == Team.Value)
                continue;
            

            if (_closestTarget == null ||
                transform.position.DistanceXZ(_cacheTarget.transform.position) < transform.position.DistanceXZ(_closestTarget.transform.position))
            {
                _closestTarget = _cacheTarget;
            }
        }

        if (_closestTarget == null) return;

        //Target = _closestTarget;
        if (IsServer) SetTargetClientRpc(_closestTarget.NetworkObjectId);

        _state = States.Attack;
    }

    protected virtual void Attack()
    {
        if (!Target || Target.IsDead.Value || transform.position.DistanceXZ(Target.transform.position) > StatsAttack.Value.Range)
        {
            Target = null;
            _scanTimer = _scanCooldown; // Makes scan start immediately to target quickly
            _state = States.Scan;
            return;
        }

        _attackTimer += Time.deltaTime;
        if (_attackTimer < StatsAttack.Value.Speed) return;
        
        _attackTimer = 0;

        _projectilePrefab.Spawn
        (
        instigator:        this,
        target:            Target,
        spawnPosition:     _projectileSpawnPoint.position,
        damagePayload:     new DamagePayload(StatsAttack.Value),
        team:              Team.Value,
        isThrow:           false
        ); 
    }

    protected override void Die(Entity killer)
    {
        foreach (var player in Server.S.Players)
        {
            if (player.Team.Value != Team.Value)
            {
                player.ResourceController.IncrGold(_destroyGoldReward);
                SpawnFloatingTextClientRpc(_destroyGoldReward, player.ClientRpcParams);
            }
        }

        Target = null;

        base.Die(killer);
    }

    [ClientRpc]
    private void SpawnFloatingTextClientRpc(int value, ClientRpcParams clientRpcParams)
    {
        var floatingText = Instantiate(_hudFloatingText, transform.position + Vector3.up * 5, Quaternion.identity);
        floatingText.Text.text = $"+{value}";
    }

}
