using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{

    public static BuildingSystem S;

    [SerializeField] private TileBase _indicatorTile;
    [SerializeField] private TileBase _clearTile;

    public GridLayout GridLayout { get; private set; }
    public Grid Grid             { get; private set; }

    private Tilemap _mainTilemap;
    
    #region Unity

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }

        GridLayout = FindFirstObjectByType<Grid>();
        Grid = GridLayout.GetComponent<Grid>();
        _mainTilemap = FindFirstObjectByType<Tilemap>();
    }

    #endregion

    #region Util

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordianteToGrid(Vector3 position)
    {
        Vector3Int cellPosition = GridLayout.WorldToCell(position);
        position = Grid.GetCellCenterWorld(cellPosition);
        return position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] tiles = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int position = new Vector3Int(v.x, v.y, 0);
            tiles[counter] = tilemap.GetTile(position);
            counter++;
        }

        return tiles;
    }

    #endregion

    #region Building Placement

    public bool CanBePlaced(BuildingPlaceable placeableBuilding, byte team)
    {
        
       if (team == 1)
        {
            if (!Physics.Raycast(placeableBuilding.transform.position, Vector3.down, float.PositiveInfinity, LayerMask.GetMask("Team 1 Zone")))
            {
                return false;
            }
        }
        else
        {
            if (!Physics.Raycast(placeableBuilding.transform.position, Vector3.down, float.PositiveInfinity, LayerMask.GetMask("Team 2 Zone")))
            {
                return false;
            }
        }

        if (!Physics.Raycast(placeableBuilding.transform.position, Vector3.down, float.PositiveInfinity, LayerMask.GetMask("Building Zone")))
        {
            return false;
        }

        BoundsInt area = new()
        {
            position = GridLayout.WorldToCell(placeableBuilding.GetStartPosition()),
            size = new Vector3Int(placeableBuilding.Size.x + 1, placeableBuilding.Size.y + 1, placeableBuilding.Size.z)
        };

        TileBase[] tiles = GetTilesBlock(area, _mainTilemap);

        foreach (var tile in tiles)
        {
            if (tile == _indicatorTile)
            {
                return false;
            }
        }

        Collider[] hits = Physics.OverlapBox(placeableBuilding.transform.position, placeableBuilding.transform.localScale);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.parent == null || hits[i].transform.parent.gameObject == placeableBuilding.gameObject || hits[i].isTrigger || hits[i].CompareTag("Ground")) continue;
            return false;
        }

        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size)
    {
        _mainTilemap.BoxFill(start, _indicatorTile, start.x, start.y,
                             start.x + size.x, start.y + size.y);
    }

    public void ClearArea(Vector3Int start, Vector3Int size)
    {
        _mainTilemap.BoxFill(start, _clearTile, start.x, start.y,
                             start.x + size.x, start.y + size.y);
    }


    #endregion

}
