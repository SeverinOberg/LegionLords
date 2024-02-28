using UnityEngine;

public class BuildingDrag : MonoBehaviour
{

    private void Update()
    {
        Vector3 position = BuildingSystem.GetMouseWorldPosition();
        transform.position = BuildingSystem.S.SnapCoordianteToGrid(position);
    }

}
