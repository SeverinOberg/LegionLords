using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UILegionUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{

    [field:SerializeField]
    public int Tier { get; private set; }

    [SerializeField]
    private Sprite _emptySprite;

    [HideInInspector]
    public Entity Entity;

    private Transform _contentParent;
    private Image _background;
    private Image _icon;

    [SerializeField]
    private ScrollRect _scrollRect;

    [SerializeField]
    private bool _isPlayerLegion;
    
    private Vector3 _pointerEnterSize = new Vector3(0.90f, 0.90f, 1f);

    private void Awake()
    {
        _scrollRect = FindAnyObjectByType<ScrollRect>();
        if (!_contentParent) _contentParent = transform.Find("Content");
        if (!_background)    _background    = _contentParent.Find("Background").GetComponent<Image>();
        if (!_icon)          _icon          = _contentParent.Find("Icon").GetComponent<Image>();
    }

    public void Init(Entity entity = null)
    {
        if (!_contentParent) _contentParent = transform.Find("Content");
        if (!_background)    _background    = _contentParent.Find("Background").GetComponent<Image>();
        if (!_icon)          _icon          = _contentParent.Find("Icon").GetComponent<Image>();
        if (entity != null) Set(entity);
    }

    public void Set(Entity entity)
    {
        if (!_icon)
        {
            Init(entity);
            return;
        }

        if (entity == null)
        {
            if (Entity == null) return;

            Entity = null;
            _icon.sprite = _emptySprite;
            return;
        }

        Entity = entity;
        _icon.sprite = entity.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _icon.rectTransform.DOScale(_pointerEnterSize, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _icon.rectTransform.DOScale(Vector3.one, 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _scrollRect.StopMovement();
        _scrollRect.enabled = false;

        if (Entity)
        {
            Debug.Log($"cicked on {Entity.name} [id:{Entity.ID}]");
        }
        else
        {
            Debug.Log("empty slot");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _scrollRect.enabled = true;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }

}
