using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public Action<PointerEventData> OnClickHandler = null;		// 이벤트 구독을 위한 Action
    public Action<PointerEventData> OnDragHandler = null;		

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
}
