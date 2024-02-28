using UnityEngine;

public class Building : Entity
{
    [field:Header("Building")]

    public Collider Collider { get; private set; }

    [field:SerializeField] public BuildingSO BuildingSO { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        EntityType = EntityType.Building;
        Collider = GetComponent<Collider>();
    }

}