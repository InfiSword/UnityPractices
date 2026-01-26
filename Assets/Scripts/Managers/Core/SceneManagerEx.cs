using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }      // 현재 씬의 오브젝트를 가져온다

	public void LoadScene(Define.Scene type)    // 씬타입의 매개변수
    {
        Managers.Clear();

        SceneManager.LoadScene(GetSceneName(type));     // 씬매니저의 로드씬, 씬의 이름을 가져와 로드한다.
    }

    string GetSceneName(Define.Scene type)              // 씬타입의 매개변수
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);  
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
