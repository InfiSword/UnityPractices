using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;   // 처음에는 Scene Enum의 Unknown으로 초기값을 맞춰줌

	void Awake()
	{
		Init();
	}

	protected virtual void Init()
    {
        Object obj = GameObject.FindFirstObjectByType(typeof(EventSystem));  // 이벤트 시스템이라는 타입의 오브젝트를 찾음
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";  // 리소스매니저에서, 해당 경로의 오브젝트의 클론을 생성하고 그 이름을 @EventSystem이라고 지음
    }

    public abstract void Clear();
}
