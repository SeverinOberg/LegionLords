using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{

    private CameraController _cameraController;

    public static InputActions InputActions { get; private set; }

    public static Vector2 Movement        { get; private set; }
    public static Vector2 PointerPosition { get; private set; }
   
    public static bool Shift { get; private set; }

    private void Awake()
    {
        _cameraController = GetComponent<CameraController>();

        InputActions = new();
        InputActions.Player.Enable();
    }

    private void OnEnable()
    {
        InputActions.Player.Primary.started   += OnPrimaryCallback;
        InputActions.Player.Secondary.started += OnSecondaryCallback;

        InputActions.Player.Move.performed  += OnMoveCallback;
        InputActions.Player.Move.canceled   += OnMoveCallback;

        InputActions.Player.Pointer.performed += OnPointerCallback;
        InputActions.Player.Pointer.canceled  += OnPointerCallback;

        InputActions.Player.Scroll.started += OnScrollCallback;

        InputActions.Player.Shift.performed += OnShiftCallback;
        InputActions.Player.Shift.canceled  += OnShiftCallback;

        InputActions.Player.Menu.started += OnMenuCallback;

        InputActions.Player.Keys.started += OnKeysCallback;

        InputActions.Player.MoveCameraToBase.started += OnMoveCameraToBaseCallback;

        InputActions.Player.Sell.started += OnSellCallback;
    }

    private void OnDisable()
    {
        InputActions.Player.Primary.started   -= OnPrimaryCallback;
        InputActions.Player.Secondary.started -= OnSecondaryCallback;

        InputActions.Player.Move.performed  -= OnMoveCallback;
        InputActions.Player.Move.canceled   -= OnMoveCallback;

        InputActions.Player.Pointer.performed -= OnPointerCallback;
        InputActions.Player.Pointer.canceled  -= OnPointerCallback;

        InputActions.Player.Scroll.started   -= OnScrollCallback;

        InputActions.Player.Shift.performed -= OnShiftCallback;
        InputActions.Player.Shift.canceled  -= OnShiftCallback;

        InputActions.Player.Menu.started -= OnMenuCallback;

        InputActions.Player.Keys.started -= OnKeysCallback;

        InputActions.Player.MoveCameraToBase.started -= OnMoveCameraToBaseCallback;

        InputActions.Player.Sell.started -= OnSellCallback;
    }

    private void OnPrimaryCallback(InputAction.CallbackContext ctx)
    {
        SelectionManager.S?.TrySelect();
    }

    private void OnSecondaryCallback(InputAction.CallbackContext ctx)
    {
        SelectionManager.S?.ClearSelection();
    }

    private void OnMoveCallback(InputAction.CallbackContext ctx)
    {
        Movement = ctx.ReadValue<Vector2>();
    }

    private void OnPointerCallback(InputAction.CallbackContext ctx)
    {
        PointerPosition = ctx.ReadValue<Vector2>();
    }

    private void OnScrollCallback(InputAction.CallbackContext ctx)
    {
        _cameraController.Zoom(ctx.ReadValue<Vector2>().normalized.y);
    }

    private void OnShiftCallback (InputAction.CallbackContext ctx)
    {
        Shift = !Shift;
    }

    private void OnMenuCallback(InputAction.CallbackContext ctx)
    {
        UIManager.Singleton.ToggleMenu();
    }

    private void OnMoveCameraToBaseCallback(InputAction.CallbackContext ctx)
    {
        _cameraController.MoveToBase();
    }

    private void OnSellCallback(InputAction.CallbackContext ctx)
    {
        var selected = SelectionManager.S.Selected;

        if (selected != null)
        {
            if (selected.TryGetComponent(out Spawner selectedSpawner))
            {
                Player.S.BuildingController.Sell(selectedSpawner);
            }
        }
        else
        {
            AnnouncementManager.S.Announce("need to select a building to sell it");
        }
    }

    private void OnKeysCallback(InputAction.CallbackContext ctx)
    {
        if (Shift == false)
        {
            switch (ctx.control.name)
            {
                case "1":
                    Player.S.BuildingController.InitializeBuilding(0);
                    break;
                case "2":
                    Player.S.BuildingController.InitializeBuilding(1);
                    break;
                case "3":
                    Player.S.BuildingController.InitializeBuilding(2);
                    break;
                case "4":
                    Player.S.BuildingController.InitializeBuilding(3);
                    break;
                case "5":
                    Player.S.BuildingController.InitializeBuilding(4);
                    break;
            }
        }
        else
        {
            switch (ctx.control.name)
            {
                case "1":
                    Player.S.BuildingController.InitializeBuilding(5);
                    break;
                case "2":
                    Player.S.BuildingController.InitializeBuilding(6);
                    break;
                case "3":
                    Player.S.BuildingController.InitializeBuilding(7);
                    break;
                case "4":
                    Player.S.BuildingController.InitializeBuilding(8);
                    break;
                case "5":
                    Player.S.BuildingController.InitializeBuilding(9);
                    break;
            }
        }
    }
}
