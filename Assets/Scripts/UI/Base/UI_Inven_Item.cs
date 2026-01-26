using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        Image_Item,
        Text_Item,
    }

    string _name;

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
       Bind<GameObject>(typeof(GameObjects));
       Get<GameObject>((int)GameObjects.Text_Item).GetComponent<Text>().text = _name;
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
