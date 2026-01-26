using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10;            // 최근에 사용한 순번을 지정

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();        // UI_Popup클래스를 담는 stack을 만듬
    UI_Scene _sceneUI = null;

    public GameObject Root      // Root라는 게임 오브젝트를 만듬
    {
        get
        {
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };
            return root;
		}
    }

    public void SetCanvas(GameObject go, bool sort = true)      // UI가 생길 때 자신의 캔버스의 order를 조정해줌
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);     // Util의 static메서드 사용, Canvas를 추가한 gameObject
        //Canvas canvas = go.GetOrAddComponent<Canvas>();       // 확장매서드를 사용한 방법

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;      // ScreenSpaceOverlay로 렌더러 모드를 사용
        canvas.overrideSorting = true;                          // Unity가 기본적으로 설정하는 렌더링 순서를 무시하고 Sorting Layer 및 Sorting Order에 따라 렌더링 순서가 결정된다

        if (sort)                                               // sort가 true라면
        {   
            canvas.sortingOrder = _order;                       // sortingOrder를 10으로함
            _order++;                                           // order를 ++시킴
        }
        else
        {
            canvas.sortingOrder = 0;                            // 0으로 설정, 일반 UI라는 의미
        }
    }

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base      // UI_Base를 상속, 가지고 있는 타입만 가능
	{   
		if (string.IsNullOrEmpty(name))             // name이 비어있다면
			name = typeof(T).Name;                  // T타입의 이름을 받음

		GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");        // 리소스 
		if (parent != null)
			go.transform.SetParent(parent);             // parent가 null이 아니면, 부모위치로 go 오브젝트를 위치시킴

		return Util.GetOrAddComponent<T>(go);           // 해당 타입을 들고온다.
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup              // 팝업을 띄우는 식, 이 이름은 프리팹의 이름, T타입은 스크립트, 보통 프리팹과 스크립트의 이름을 같게한다.
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

		return popup;
    }

    public void ClosePopupUI(UI_Popup popup)              // 해당 팝업창을 닫을 수 있는지   
    {
		if (_popupStack.Count == 0)
			return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()                          // 팝업을 닫는 식
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
