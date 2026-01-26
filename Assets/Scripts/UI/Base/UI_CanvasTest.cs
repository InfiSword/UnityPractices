using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CanvasTest : UI_Popup
{
    enum ButtonEnum
    {
        ButtonTest,        
    }
    enum ImageEnum
    {
        ImageTest,
    }

    Button btn;
    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(ButtonEnum));
        Bind<Image>(typeof(ImageEnum));

        btn = GetButton((int)ButtonEnum.ButtonTest);
        btn.gameObject.BindEvent((PointerEventData data) =>
        {
            Debug.Log("Click");
        },
        Define.UIEvent.Click);
    }
}
