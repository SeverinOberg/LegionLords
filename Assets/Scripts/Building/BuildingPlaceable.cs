using UnityEngine;

public class BuildingPlaceable : MonoBehaviour
{

    public Renderer Renderer { get; private set; }

    public bool Placed { get; private set; }
    public Vector3Int Size { get; private set; }

    private Vector3[] _vertices;

    private void Awake()
    {
        Renderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider b = GetComponentInChildren<BoxCollider>();
        _vertices = new Vector3[4];
        _vertices[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
        _vertices[1] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
        _vertices[2] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
        _vertices[3] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
    }

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[_vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = transform.TransformPoint(_vertices[i]);
            vertices[i] = BuildingSystem.S.GridLayout.WorldToCell(worldPosition);
        }

        Size = new(
            x: Mathf.Abs((vertices[0] - vertices[1]).x),
            y: Mathf.Abs((vertices[0] - vertices[3]).y),
            z: 1
        );

    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(_vertices[0]);
    }

    public virtual void Place()
    {
        BuildingDrag drag = GetComponent<BuildingDrag>();
        Destroy(drag);

        Placed = true;

        // Invoke events of placement if necessary
    }

}
