using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();     // T타입에 맞는 컴포넌트를 들고옴
		if (component == null)                  // 만약 컴포넌트가 null이라면 
            component = go.AddComponent<T>();   // AddCompoenet로 컴포넌트를 추가시킴
        return component;                       // 해당 T타입에 맞는 컴포넌트를 리턴시킴
	}

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);        // Transform으로 타입을 지정하여 FindChild를 실행
        if (transform == null)
            return null;

        return transform.gameObject;        // 게임오브젝트를 리턴
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)       // 게임 오브젝트의 자식오브젝트의 수만큼 for문 실행
            {
                Transform transform = go.transform.GetChild(i);     // i번째 자식오브젝트의 위치값을 가져옴
                if (string.IsNullOrEmpty(name) || transform.name == name)   
                {
                    T component = transform.GetComponent<T>();      // T타입에 맞는 컴포넌트를 가져옴 
                    if (component != null)
                        return component;
                }
            }
		}
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())    // 게임오브젝트 아래에 존재하는 자식오브젝트를 순회
            {
                if (string.IsNullOrEmpty(name) || component.name == name)   // 빈 문자열인지 검사, 매개변수로 받은 이름과, 자식오브젝트의 이름이 같다면
                    return component;       // 자식오브젝트를 반환
            }
        }

        return null;
    }


}
