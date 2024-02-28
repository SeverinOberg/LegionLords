using UnityEngine;
using UnityEngine.UI;

public class UIBuildingButton : MonoBehaviour
{

    [SerializeField] private byte _index;
    public BuildingSO BuildingSO { get; private set; }

    private void Awake()
    {
       
       GetComponent<Button>().onClick.AddListener(OnButtonClick);
       BuildingSO = Player.S.BuildingController.SpawnerPrefabs[_index].BuildingSO;
    }

    private void OnButtonClick()
    {
        Player.S.BuildingController.CancelBuildingPlacement();
        Player.S.BuildingController.InitializeBuilding(_index);
    }
}
