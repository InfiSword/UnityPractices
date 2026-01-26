using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
  enum GameObjects
    {
        Panel,
    }

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        GameObject Panel = Get<GameObject>((int)GameObjects.Panel);

        foreach(Transform child in Panel.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }

        for(int i=0; i<8; i++)
        {
            GameObject Item = Managers.Resource.Instantiate("UI/Scene/UI_Inven_Item");                        
            Item.transform.SetParent(Panel.transform);
            Item.transform.localScale = new Vector3(1, 1, 1);

            UI_Inven_Item invenItem =  Util.GetOrAddComponent<UI_Inven_Item>(Item);
            invenItem.SetInfo($"바인딩 테스트{i + 1}");
        }

    }
}
