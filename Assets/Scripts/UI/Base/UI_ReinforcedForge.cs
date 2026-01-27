using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 간단한 테스트용 대장간 UI
// - 슬롯 하나에 인벤토리 아이템을 드래그 드롭
// - 강화 버튼 클릭 시 성공/실패 여부를 Debug.Log로 출력
public class UI_ReinforcedForge : UI_Popup
{
    enum GameObjects
    {
        Slot,               // 아이템을 올려둘 슬롯 (예: Image + Text)
        Button_Reinforce,   // 강화 버튼
        DragArea,           // 상단 드래그 영역 (선택)
    }

    private UI_Inven_Item _slotItem;     // 슬롯에 표시할 UI
    private ItemData _targetItem;        // 강화 대상 아이템

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        // 슬롯을 UI_Inven_Item로 구성했다면 여기서 가져옴
        GameObject slotObj = Get<GameObject>((int)GameObjects.Slot);
        if (slotObj != null)
        {
            _slotItem = slotObj.GetComponent<UI_Inven_Item>();

            // 슬롯에 붙은 UI_Inven_Item도 Init 호출로 이벤트 바인딩
            if (_slotItem != null)
            {
                _slotItem.Init();
            }

            // 슬롯에 드롭 이벤트 바인딩 (인벤토리의 SlotDrop 로직을 응용)
            BindEvent(slotObj, OnSlotDrop, Define.UIEvent.Drop);
        }

        // 강화 버튼 설정
        GameObject btnObj = Get<GameObject>((int)GameObjects.Button_Reinforce);
        if (btnObj != null)
        {
            Button reinforceButton = btnObj.GetComponent<Button>();
            if (reinforceButton != null)
            {
                reinforceButton.onClick.AddListener(OnClickReinforce);
            }
        }

        // 드래그 가능한 상단 영역 설정 (선택)
        GameObject dragArea = Get<GameObject>((int)GameObjects.DragArea);
        if (dragArea != null)
        {
            SetDraggableArea(dragArea);
        }
        else
        {
            // DragArea가 없으면 전체 창을 드래그 가능하게 설정
            SetDraggableArea(gameObject);
        }
    }

    // 인벤토리에서 드래그해 온 아이템을 슬롯에 올리는 로직
    // 기존 인벤토리의 OnSlotDrop 로직을 "대상 슬롯 1개" 버전으로 단순화
    private void OnSlotDrop(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        // 드래그 중인 오브젝트에서 UI_Inven_Item을 찾음
        UI_Inven_Item draggedItem = eventData.pointerDrag.GetComponent<UI_Inven_Item>();
        if (draggedItem == null)
            return;

        ItemData draggedData = draggedItem.ItemData;
        if (draggedData == null)
            return;

        // 강화 대상 아이템 등록
        _targetItem = draggedData;

        // 슬롯 UI에 아이템 정보 표시 (슬롯에 UI_Inven_Item이 붙어 있다고 가정)
        if (_slotItem != null)
        {
            // 대장간 슬롯은 인벤이 아니므로, 인덱스/인벤 참조는 의미 없어서 0, null 전달
            _slotItem.SetInfo(_targetItem, 0, null);
        }

        Debug.Log($"[Reinforced Forge] 슬롯에 아이템 '{_targetItem.name}' 등록");
    }

    // 강화 버튼 클릭
    private void OnClickReinforce()
    {
        if (_targetItem == null)
        {
            Debug.Log("[Reinforced Forge] 강화할 아이템이 없습니다.");
            return;
        }

        // 테스트용: 50% 확률로 성공/실패
        float successRate = 0.5f;
        bool isSuccess = Random.value < successRate;

        if (isSuccess)
        {
            Debug.Log($"[Reinforced Forge] '{_targetItem.name}' 강화 성공!");
        }
        else
        {
            Debug.Log($"[Reinforced Forge] '{_targetItem.name}' 강화 실패...");
        }
    }
}
