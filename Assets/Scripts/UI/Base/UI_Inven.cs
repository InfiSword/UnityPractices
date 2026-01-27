using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven : UI_Popup
{
    enum GameObjects
    {
        ScrollView,
        Content,
        DragArea,  // 드래그 가능한 영역 (상단 바)
    }

    private GameObject _content;
    private List<UI_Inven_Item> _invenItems = new List<UI_Inven_Item>();
    private List<ItemData> _invenData = new List<ItemData>();
    
    private int _maxSlots = 20;
    private int _expansionLevel = 0; // 0: 20, 1: 40, 2: 60
    private UI_Inven_Item _draggedItem;
    private UI_ItemInfo _itemInfoPanel;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        _content = Get<GameObject>((int)GameObjects.Content);
               
        // 드래그 가능한 영역 설정
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

        // 인벤토리 초기화
        foreach (Transform child in _content.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        _invenItems.Clear();
        _invenData.Clear();

        // 슬롯 생성
        for (int i = 0; i < _maxSlots; i++)
        {
            CreateSlot(i);
        }

        // 테스트 아이템 추가
        AddTestItems();
    }

    private void CreateSlot(int index)
    {
        GameObject itemObj = Managers.Resource.Instantiate("UI/Popup/UI_Inven_Item");
        itemObj.transform.SetParent(_content.transform);
        itemObj.transform.localScale = Vector3.one;
        
        UI_Inven_Item invenItem = Util.GetOrAddComponent<UI_Inven_Item>(itemObj);

        invenItem.Init();
        invenItem.SetInfo(null, index, this);
        
        _invenItems.Add(invenItem);
        _invenData.Add(null);
        
        // 드롭 이벤트 바인딩
        BindEvent(itemObj, (data) => OnSlotDrop(data, index), Define.UIEvent.Drop);
    }

    private void AddTestItems()
    {
        // 테스트 아이템 추가
        for(int i = 0; i < 5; i++)
        {
            WeaponItemData testItem = new WeaponItemData(i + 1, $"테스트 아이템{i + 1}", $"테스트 아이템 설명{i + 1}");
            testItem.stats = new WeaponStats { attackPower = 10 + i * 5, attackSpeed = 1.0f, durability = 100 };
            AddItem(testItem);
        }
    }

    public void AddItem(ItemData itemData)
    {
        for(int i = 0; i < _invenData.Count; i++)
        {
            if (_invenData[i] == null)
            {
                _invenData[i] = itemData;
                _invenItems[i].SetInfo(itemData, i, this);
                return;
            }
        }
        Debug.Log("인벤토리 가득 찼습니다.");
    }

    private void OnSlotDrop(PointerEventData eventData, int slotIndex)
    {
        if (_draggedItem == null) return;
        
        int draggedIndex = _draggedItem.SlotIndex;
        ItemData draggedData = _draggedItem.ItemData;
        
        // 드래그한 아이템이 이미 해당 슬롯에 있는 경우
        if (draggedIndex == slotIndex)
        {
            // 드래그한 아이템을 원래 위치로 복귀
            _draggedItem.transform.SetParent(_invenItems[draggedIndex].transform.parent);
            _draggedItem.transform.localPosition = Vector3.zero;
            _draggedItem = null;
            return;
        }
        
        // 드래그한 아이템을 놓은 슬롯의 아이템 데이터 저장
        ItemData targetData = _invenData[slotIndex];
        
        _invenData[slotIndex] = draggedData;
        _invenData[draggedIndex] = targetData;
        
        _invenItems[slotIndex].SetInfo(draggedData, slotIndex, this);
        _invenItems[draggedIndex].SetInfo(targetData, draggedIndex, this);
        
        // 드래그한 아이템을 놓은 슬롯의 부모로 이동
        _draggedItem.transform.SetParent(_invenItems[slotIndex].transform.parent);
        _draggedItem.transform.localPosition = Vector3.zero;
        _draggedItem.transform.SetSiblingIndex(slotIndex);
        
        // 드래그한 아이템의 슬롯 인덱스 업데이트
        _draggedItem.SetSlotIndex(slotIndex);
        if (targetData != null)
        {
            _invenItems[draggedIndex].SetSlotIndex(draggedIndex);
        }
        
        _draggedItem = null;
    }

    public void SetDraggedItem(UI_Inven_Item item)
    {
        _draggedItem = item;
    }

    public void ClearDraggedItem()
    {
        _draggedItem = null;
    }

    public UI_Inven_Item GetDraggedItem()
    {
        return _draggedItem;
    }

    public void ShowItemInfo(ItemData itemData)
    {
        // 기존 팝업이 있으면 닫기
        if (_itemInfoPanel != null)
        {
            Managers.UI.ClosePopupUI(_itemInfoPanel);
            _itemInfoPanel = null;
        }
        
        // ShowPopupUI를 사용하여 팝업 스택에 제대로 등록
        _itemInfoPanel = Managers.UI.ShowPopupUI<UI_ItemInfo>();
        if (_itemInfoPanel == null)
        {
            Debug.LogError("UI_ItemInfo 팝업을 생성할 수 없습니다.");
            return;
        }
        _itemInfoPanel.Init();
        _itemInfoPanel.SetInfo(itemData);
    }

    public void ExpandInventory()
    {
        if (_expansionLevel >= 2) 
        {
            Debug.Log("인벤토리 확장 불가. 최대 확장 레벨 도달");
            return;
        }
        
        _expansionLevel++;
        _maxSlots = _expansionLevel == 1 ? 40 : 60;
        
        // 인벤토리 슬롯 추가
        int currentCount = _invenItems.Count;
        for(int i = currentCount; i < _maxSlots; i++)
        {
            CreateSlot(i);
        }
        
        Debug.Log($"인벤토리 확장. 총 슬롯 수: {_maxSlots}");
    }

    public int GetMaxSlots() => _maxSlots;
    public int GetExpansionLevel() => _expansionLevel;
}
