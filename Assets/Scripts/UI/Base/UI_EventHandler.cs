using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public Action<PointerEventData> OnClickHandler = null;		// 이벤트 구독을 위한 Action
    public Action<PointerEventData> OnDragHandler = null;		
    public Action<PointerEventData> OnBeginDragHandler = null;   // 드래그 시작 이벤트
    public Action<PointerEventData> OnEndDragHandler = null;     // 드래그 종료 이벤트
    public Action<PointerEventData> OnDropHandler = null;        // 드롭 이벤트

	public void OnPointerClick(PointerEventData eventData)      // 클릭시, 발생시킬 이벤트
    {
		if (OnClickHandler != null)
			OnClickHandler.Invoke(eventData);					// 클릭이벤트 관련하여 구독한 함수를 실행
	}

	public void OnDrag(PointerEventData eventData)				// 드래그시, 발생시킬 이벤트
    {
		if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);					// 드래그이벤트 관련하여 구독한 함수를 실행
	}

    public void OnBeginDrag(PointerEventData eventData)         // 드래그 시작시
    {
        if (OnBeginDragHandler != null)
            OnBeginDragHandler.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)           // 드래그 종료시
    {
        if (OnEndDragHandler != null)
            OnEndDragHandler.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)              // 드롭시
    {
        if (OnDropHandler != null)
            OnDropHandler.Invoke(eventData);
    }
}
