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
        ButtonForge,
        ButtonCloseInventory,
    }

    Button btn;
    Button expandBtn;
    Button forgeBtn;
    Button closeInvenBtn;
    private UI_Inven _inven;

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(ButtonEnum));

        btn = GetButton((int)ButtonEnum.ButtonTest);
        btn.gameObject.BindEvent((PointerEventData data) =>
        {
            Debug.Log("Click");
        },
        Define.UIEvent.Click);

        // 인벤토리 확장 버튼
        expandBtn = GetButton((int)ButtonEnum.ButtonExpandInventory);
        expandBtn.gameObject.BindEvent((PointerEventData data) =>
        {
            ExpandInventory();
        },
        Define.UIEvent.Click);

        // 인벤토리 닫기 버튼
        closeInvenBtn = GetButton((int)ButtonEnum.ButtonCloseInventory);
        closeInvenBtn.gameObject.BindEvent((PointerEventData data) =>
        {
            CloseInventory();
        },
        Define.UIEvent.Click);

        // 강화 대장간 스폰 버튼
        forgeBtn = GetButton((int)ButtonEnum.ButtonForge);
        forgeBtn.gameObject.BindEvent((PointerEventData data) =>
        {
            SpawnForge();
        },
        Define.UIEvent.Click);

        // 초기 버튼 상태 설정
        UpdateInventoryButtons();
    }

    private void Update()
    {
        // 인벤토리 상태에 따라 버튼 표시 업데이트
        UpdateInventoryButtons();
    }

    private void UpdateInventoryButtons()
    {
        // 인벤토리 인스턴스 찾기
        if (_inven == null)
        {
            _inven = FindFirstObjectByType<UI_Inven>();
        }

        // 인벤토리가 활성화되어 있으면 확장/닫기 버튼 표시
        bool isInvenActive = _inven != null && _inven.gameObject.activeInHierarchy;
        
        if (expandBtn != null)
            expandBtn.gameObject.SetActive(isInvenActive);
        
        if (closeInvenBtn != null)
            closeInvenBtn.gameObject.SetActive(isInvenActive);
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

    private void CloseInventory()
    {
        // 인벤토리 인스턴스 찾기
        if (_inven == null)
        {
            _inven = FindFirstObjectByType<UI_Inven>();
        }

        if (_inven != null)
        {
            Managers.UI.ClosePopupUI(_inven);
            _inven = null;
            Debug.Log("인벤토리 닫기 완료");
        }
        else
        {
            Debug.LogWarning("인벤토리를 찾을 수 없습니다.");
        }
    }

    private void SpawnForge()
    {
        UI_ReinforcedForge forge = Managers.UI.ShowPopupUI<UI_ReinforcedForge>();
        if (forge != null)
        {
            forge.Init();
            Debug.Log("강화 대장간 스폰 완료");
        }
        else
        {
            Debug.LogWarning("강화 대장간을 스폰할 수 없습니다.");
        }
    }
}
