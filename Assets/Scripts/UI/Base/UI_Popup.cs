using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Popup : UI_Base
{
    private Vector2 _dragOffset;
    private RectTransform _rectTransform;
    private bool _isDragging = false;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
        _rectTransform = GetComponent<RectTransform>();
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
        
        // 드래그 시작 시 마우스 위치와 창 위치의 오프셋 저장
        Vector2 screenPoint = eventData.position;
        Vector2 currentPosition = _rectTransform.position;
        _dragOffset = screenPoint - currentPosition;
        _isDragging = true;
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _rectTransform == null) return;

        // 마우스 위치에서 오프셋을 빼서 창의 새 위치 계산
        Vector2 newPosition = eventData.position - _dragOffset;
        _rectTransform.position = newPosition;
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    public void SetDraggableArea(GameObject draggableArea)
    {
        UI_EventHandler handler = Util.GetOrAddComponent<UI_EventHandler>(draggableArea);
        handler.OnBeginDragHandler += OnBeginDrag;
        handler.OnDragHandler += OnDrag;
        handler.OnEndDragHandler += OnEndDrag;
    }
}
