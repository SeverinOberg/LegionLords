using UnityEngine;

public class Spawner : Building
{

    [field:Header("Spawner")]

    [field:SerializeField] public Unit UnitToSpawn { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        EntityType = EntityType.Spawner;
    }

    public void Initialize(Unit unitToSpawn)
    {
        if (UnitToSpawn)
        {
            Debug.Log("UnitToSpawn has already been initialized");
            return;
        }

        UnitToSpawn = unitToSpawn;
    }

}