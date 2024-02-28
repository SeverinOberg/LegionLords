using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{

    [SerializeField] private float _movementSpeed = 30f;
    [SerializeField] private Vector3 _XZDistanceClamp = new Vector3(100f, 0, 100f);

    [SerializeField] private float _maxCameraZoomDistance = 25f;
    [SerializeField] private float _minCameraZoomDistance = 10;
    private const float _borderSize = 1;

    private Vector3 _startBasePositon = new();

    private Vector2 _screenBounds = new(Screen.width, Screen.height);
    private bool _isMouseAtBorder;
    private Vector3 _direction;
    private Vector3 _clampedPosition = new();

    private void Update()
    {
        if (!IsOwner) return;

        if (!Application.isFocused)
        {
            if (_direction != Vector3.zero)
            {
                _direction = Vector3.zero;
            }
            return;
        }

        Move();
    }

    public void Init(Vector3 teamBasePosition)
    {
        _startBasePositon = teamBasePosition;
    }

    private void Move()
    {

        _direction = transform.right.x * new Vector3(InputController.Movement.x, 0, InputController.Movement.y);
        MouseBorderValidation();

        if (_isMouseAtBorder)
        {
            _direction = (new Vector3(InputController.PointerPosition.x, 0, InputController.PointerPosition.y) - new Vector3(_screenBounds.x, 0, _screenBounds.y) / 2).normalized;
        }

        if (InputController.Movement.normalized != Vector2.zero || _isMouseAtBorder)
        {
            if (MatchManager.S.GameSpeed.Value != 0)
            {
                transform.Translate(_movementSpeed * (Time.deltaTime / MatchManager.S.GameSpeed.Value) * _direction);
            }
            else
            {
                transform.Translate(_movementSpeed * Time.deltaTime * _direction);
            }
            
            ClampCameraPosition();
        }
    }

    private void MouseBorderValidation()
    {
        if (
            InputController.PointerPosition.x <= _borderSize ||
            InputController.PointerPosition.x >= _screenBounds.x - _borderSize ||
            InputController.PointerPosition.y <= _borderSize ||
            InputController.PointerPosition.y >= _screenBounds.y - _borderSize)
        {
            _isMouseAtBorder = true;
        }
        else
        {
            _isMouseAtBorder = false;
        }
    }

    private void ClampCameraPosition()
    {
        _clampedPosition = transform.position;
        _clampedPosition.x = Mathf.Clamp(transform.position.x, -_XZDistanceClamp.x, _XZDistanceClamp.x);
        _clampedPosition.z = Mathf.Clamp(transform.position.z, -_XZDistanceClamp.z, _XZDistanceClamp.z);
        transform.position = _clampedPosition;
    }

    public void Zoom(float value)
    {
        _clampedPosition = transform.position;
        _clampedPosition.y = Mathf.Clamp(transform.position.y - value, _minCameraZoomDistance, _maxCameraZoomDistance);
        transform.position = _clampedPosition;
    }

    public void MoveToBase()
    {
        transform.position =  new Vector3(_startBasePositon.x, transform.position.y, _startBasePositon.z);
    }

}
