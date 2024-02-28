using UnityEngine;

public class HUDHealthbarController : MonoBehaviour
{

    private Entity        _entity;
    private GameObject    _canvas;
    private RectTransform _healthRect;

    private void Awake()
    {
        _entity = GetComponent<Entity>();
        _canvas = transform.Find("UI Entity Canvas")?.gameObject;
        _healthRect = _canvas?.transform.Find("Health").GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!_canvas.activeSelf) return;
        _canvas.transform.eulerAngles = new Vector3(65, 0, 0);
    }

    public void UpdateHealthUI(float health)
    {
        if (!_canvas) return;

        if (health < _entity.StatsDefense.Value.MaxHealth && health > 0)
        {
            if (!_canvas.activeSelf)
            {
                _canvas.SetActive(true);
            }
            _healthRect.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, health / _entity.StatsDefense.Value.MaxHealth);
        }
        else
        {
            DisableHUD();
        }
    }

    public void DisableHUD()
    {
        if (_canvas.activeSelf)
            _canvas.SetActive(false);
    }

}
