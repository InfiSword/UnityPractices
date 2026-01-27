using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
	protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();	// 딕셔너리를 통해 Bind에 들어온 타입과, 그 오브젝트들을 저장
	
	public abstract void Init();

	protected void Bind<T>(Type type) where T : UnityEngine.Object				
	{
		string[] names = Enum.GetNames(type);									// 들어온 타입에 대한 Enum이름의 배열을 names에 초기화
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];	// Enum길이만큼 유니티오브젝트 배열을 생성
		_objects.Add(typeof(T), objects);										// 타입에 관련한 오브젝트 타입과 빈 배열을 저장

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))								// 타입 이름과 오브젝트가 동일하다면 실행
				objects[i] = Util.FindChild(gameObject, names[i], true);		// objects에 하나하나씩 값을 넣고, 결과값을 배열[i]에 넣는다.
			else
				objects[i] = Util.FindChild<T>(gameObject, names[i], true);		

			if (objects[i] == null)							
				Debug.Log($"Failed to bind({names[i]})");
		}
	}

	protected T Get<T>(int idx) where T : UnityEngine.Object				// T타입에 맞는 Enum의 index값을 가져옴
	{	
		UnityEngine.Object[] objects = null;
		if (_objects.TryGetValue(typeof(T), out objects) == false)			// T타입에 맞는 오브젝트의 값이 없다면 
			return null;

		return objects[idx] as T;											
	}

	protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	protected Text GetText(int idx) { return Get<Text>(idx); }
	protected Button GetButton(int idx) { return Get<Button>(idx); }
	protected Image GetImage(int idx) { return Get<Image>(idx); }

	public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)	// 해당 이벤트를 발생시킬 오브젝트와, 이벤트가 발생시, 실행할 함수, 그리고 UI이벤트관련 Enum값
	{
		UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);		// 해당 오브젝트에 컴포넌트를 추가 및 가져옴
		
		switch (type)
		{
			case Define.UIEvent.Click:			// 해당 Enum이 클릭일 시.
				evt.OnClickHandler -= action;	// 클릭시 발생할 함수를 구독해제 및 구독
				evt.OnClickHandler += action;
				break;
			case Define.UIEvent.Drag:			// 드래그일시
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
			case Define.UIEvent.BeginDrag:		// 드래그 시작일시
				evt.OnBeginDragHandler -= action;
				evt.OnBeginDragHandler += action;
				break;
			case Define.UIEvent.EndDrag:		// 드래그 종료일시
				evt.OnEndDragHandler -= action;
				evt.OnEndDragHandler += action;
				break;
			case Define.UIEvent.Drop:			// 드롭일시
				evt.OnDropHandler -= action;
				evt.OnDropHandler += action;
				break;
		}
	}
}
