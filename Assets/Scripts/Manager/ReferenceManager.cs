using Unity.Netcode;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{

    public static ReferenceManager S { get; private set; }

    [field: SerializeField] public GameObject DefaultDeathPS { get; private set; }
    [field: SerializeField] public GameObject SpellWrathPS { get; private set; }
    [field: SerializeField] public ParticleSystem PoisonTickPS { get; private set; }

    [field: SerializeField] public Material MaterialTeam1 { get; private set; }
    [field: SerializeField] public Material MaterialTeam2 { get; private set; }

    [field: SerializeField] public NetworkObject TeamBase1 { get; private set; }
    [field: SerializeField] public NetworkObject TeamBase2 { get; private set; }
    
    [field:SerializeField] 
    public Transform Team1Spawn { get; private set; }
    [field:SerializeField] 
    public Transform Team2Spawn { get; private set; }

    [field: SerializeField]
    public PlayerZonePositions.OneVSOne OneVSOne { get; private set; }
    [field: SerializeField]
    public PlayerZonePositions.TwoVSTwo TwoVSTwo { get; private set; }
    [field: SerializeField]
    public PlayerZonePositions.ThreeVSThree ThreeVSThree { get; private set; }

    public LayerMask EntityMask { get; private set; }
    public LayerMask UnitMask { get; private set; }
    public LayerMask BuildingMask { get; private set; }
    public LayerMask ObstacleMask { get; private set; }
    public LayerMask ResourceMask { get; private set; }
    public LayerMask GroundMask { get; private set; }

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

        GroundMask   = LayerMask.GetMask("Ground");
        ObstacleMask = LayerMask.GetMask("Obstacle");
        ResourceMask = LayerMask.GetMask("Resource");

        EntityMask   = LayerMask.GetMask("Entity");
        UnitMask     = LayerMask.GetMask("Unit");
        BuildingMask = LayerMask.GetMask("Building");
    }

    public Transform GetEnemyBaseTransform(byte team)
    {
        if (team == 1)
        {
            return TeamBase2.transform;
        }
        else
        {
            return TeamBase1.transform;
        }
    }

    public NetworkObject GetEnemyBaseNetworkObject(byte team)
    {
        if (team == 1)
        {
            return TeamBase2;
        }
        else
        {
            return TeamBase1;
        }
    }

    public Material GetMaterialByTeam(byte team)
    {
        if (team == 1)
        {
            return MaterialTeam1;
        }
        else
        {
            return MaterialTeam2;
        }
    }

}