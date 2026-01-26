using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object              // T 타입의 Path string을 받는 Load함수
    {
        if (typeof(T) == typeof(GameObject))                    // 해당 타입이 GameObject 타입이라면
        {
            string name = path;                                 // name을 path로 저장
            int index = name.LastIndexOf('/');                  // 경로에 /문자의 인덱스를 마지막 끝자리부터 찾음
            if (index >= 0)                                     // 0보다 크다면
                name = name.Substring(index + 1);               // '/'의 뒤의 인덱스부터 잘라 문자열 끝까지 name에 저장함

            GameObject go = Managers.Pool.GetOriginal(name);    // Pool매니저의 GetOriginal함수를 실행
            if (go != null)         // null이 아니면
                return go as T;     // T타입을반환
        }

        return Resources.Load<T>(path); // 해당 타입의 경로의 Resources의 Load함수를 실행함
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
}
