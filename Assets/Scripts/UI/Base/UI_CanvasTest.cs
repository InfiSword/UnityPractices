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
        ButtonExpandInventory,        
    }
    enum ImageEnum
    {
        ImageTest,
    }

    Button btn;
    Button expandBtn;
    private UI_Inven _inven;
    
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

        // 인벤토리 확장 버튼
        expandBtn = GetButton((int)ButtonEnum.ButtonExpandInventory);
        if (expandBtn != null)
        {
            expandBtn.gameObject.BindEvent((PointerEventData data) =>
            {
                ExpandInventory();
            },
            Define.UIEvent.Click);
        }
    }

    private void ExpandInventory()
    {
        // 인벤토리 인스턴스 찾기
        if (_inven == null)
        {
            _inven = FindFirstObjectByType<UI_Inven>();
        }
        
        if (_inven != null)
        {
            _inven.ExpandInventory();
            Debug.Log($"인벤토리 확장 완료. 현재 슬롯: {_inven.GetMaxSlots()}개");
        }
        else
        {
            Debug.LogWarning("인벤토리를 찾을 수 없습니다.");
        }
    }
}
