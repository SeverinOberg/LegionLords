using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private GameObject _collisionPrefab;

    public bool    IsInitialized       { get; private set; }
    public Entity  Instigator          { get; private set; }
    public Entity Target               { get; private set; }
    public Vector3 SpawnPosition       { get; private set; }
    public Vector3 EndDestination      { get; private set; }
    public DamagePayload DamagePayload { get; private set; }
    public byte   Team                 { get; private set; }
    public float  Delay                { get; private set; }
    public float  CollisionDistance    { get; private set; }
    public float  Distance             { get; private set; }
    public bool   IsThrow              { get; private set; }
    public AltitudeType AltitudeType   { get; private set; }

    private float _timeToHitTarget = 0;
    private const float MAX_HEIGHT_FACTOR = 0.5f;

    private float _maxThrowHeight;
    private float _currentHeight;
    private Vector3 _currentPosition;
   
    public void Spawn(Entity instigator, Entity target, Vector3 spawnPosition, DamagePayload damagePayload, byte team, AltitudeType altitudeType = AltitudeType.All, bool isThrow = false)
    {
        if (!target) return;
        Projectile self = Instantiate(this, spawnPosition, Quaternion.identity);
        self.Instigator = instigator;
        self.Target = target;
        self.Team = team;
        self.SpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);

        Physics.Raycast(target.transform.position + (Vector3.up * 100), Vector3.down, out RaycastHit hitInfo, float.PositiveInfinity, ReferenceManager.S.GroundMask);
        self.EndDestination = hitInfo.point;
        
        self.Delay = instigator.StatsAttack.Value.TravelSpeed;
        self.CollisionDistance = instigator.StatsAttack.Value.RangedCollisionDistance;
        self.IsThrow = isThrow;
        self.AltitudeType = altitudeType;
        self.DamagePayload = damagePayload;

        self.transform.LookAt(target.transform);

        self.IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized) return;

        _timeToHitTarget += Time.deltaTime / Mathf.Min(Delay, SpawnPosition.DistanceXZ(Target.transform.position) / Delay);

        if (IsThrow)
        {
            if (Vector3.Distance(transform.position, EndDestination) <= CollisionDistance || _timeToHitTarget > Delay)
            {
                IsInitialized = false;
                if (NetworkManager.Singleton.IsServer)
                {
                    DealSplashDamage();
                }

                if (_collisionPrefab)
                {
                    Instantiate(_collisionPrefab, EndDestination, Quaternion.identity);
                }
                Destroy(gameObject);
                return;
            }

            _maxThrowHeight = EndDestination.y + SpawnPosition.DistanceXZ(EndDestination) * MAX_HEIGHT_FACTOR;
            _currentHeight = SpawnPosition.y + (_maxThrowHeight - SpawnPosition.y) * (4f * _timeToHitTarget - Mathf.Pow(2f * _timeToHitTarget, 2f));

            _currentPosition = Vector3.Lerp(SpawnPosition, EndDestination, _timeToHitTarget);
            _currentPosition.y = _currentHeight;

            transform.position = _currentPosition;
        }
        else
        {
            if (!Target || !Instigator || Target.IsDead.Value)
            {
                if (_collisionPrefab != null)
                {
                    Instantiate(_collisionPrefab, transform.position, Quaternion.identity).transform.localScale = Target.StatsGeneral.Value.HitBoxSize.MakeVector3(); 
                }
                Destroy(gameObject);
                return;
            }

            if (transform.position.DistanceXZ(Target.transform.position) <= CollisionDistance)
            {
                if (_collisionPrefab != null)
                {
                    Instantiate(_collisionPrefab, transform.position, Quaternion.identity).transform.localScale = Target.StatsGeneral.Value.HitBoxSize.MakeVector3(); 
                }
                if (NetworkManager.Singleton.IsServer)
                {
                    Instigator.Attack(Target);
                }
  
                Destroy(gameObject);
                return;
            }

            transform.position = Vector3.Lerp(SpawnPosition, Target.transform.position, _timeToHitTarget);
        }
    }


    private void DealSplashDamage()
    {
        Entity cacheEntity = null;
        Collider[] cacheHits = Physics.OverlapSphere(EndDestination, DamagePayload.SplashRadius, ReferenceManager.S.UnitMask | ReferenceManager.S.ObstacleMask);
        for (int i = 0; i < cacheHits.Length; i++)
        {
            if (cacheHits[i].isTrigger) continue;
            
            cacheEntity = cacheHits[i].GetComponent<Entity>();
            if (cacheEntity == null || cacheEntity.Team.Value == Team) continue;

            if (AltitudeType == AltitudeType.Ground)
            {
                if (cacheEntity.IsFlying.Value == true) continue;
            }
            else if (AltitudeType == AltitudeType.Flying)
            {
                if (cacheEntity.IsFlying.Value == false) continue;
            }

            cacheEntity.TakeDamage(DamagePayload);

            if (_collisionPrefab)
            {
                Instantiate(_collisionPrefab, cacheEntity.transform.position, Quaternion.identity);
            }
        }
    }

}
