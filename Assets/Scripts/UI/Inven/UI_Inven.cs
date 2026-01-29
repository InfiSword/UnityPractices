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
    private List<UI_InventorySlot> _slotComponents = new List<UI_InventorySlot>();
    
    private InventoryData _inventoryData;
    private UI_Item _draggedItem;
    private UI_ItemInfo _itemInfoPanel;
    public InventoryData InventoryData => _inventoryData;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        _content = Get<GameObject>((int)GameObjects.Content);
        
        // 인벤토리 데이터 초기화
        _inventoryData = new InventoryData(InventoryConfig.INITIAL_SLOTS);
               
        // 드래그 가능한 영역 설정
        GameObject dragArea = Get<GameObject>((int)GameObjects.DragArea);
        if (dragArea != null)
        {
            SetDraggableArea(dragArea);
        }
        else
        {
            SetDraggableArea(gameObject);
        }

        // UI 초기화
        ClearUI();
        InitializeSlots();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // 테스트 아이템은 에디터/개발 빌드에서만 추가
        InventoryTestUtility.AddTestItems(this);
#endif
    }

    /// <summary>
    /// UI 요소 초기화 및 정리
    /// </summary>
    private void ClearUI()
    {
        foreach (Transform child in _content.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        _slotComponents.Clear();
    }

    /// <summary>
    /// 초기 슬롯 생성
    /// </summary>
    private void InitializeSlots()
    {
        // _inventoryData.MaxSlots를 기준으로 UI 슬롯 생성
        for (int i = 0; i < _inventoryData.MaxSlots; i++)
        {
            CreateSlot(i);
        }
        
        ValidateSynchronization();
    }

    /// <summary>
    /// 슬롯(빈 칸) 생성
    /// </summary>
    private void CreateSlot(int index)
    {
        GameObject slotObj = Managers.Resource.Instantiate(InventoryConfig.SLOT_PREFAB_PATH);
        slotObj.transform.SetParent(_content.transform);
        slotObj.transform.localScale = Vector3.one;
        
        // UI_InventorySlot 컴포넌트 추가 및 초기화
        UI_InventorySlot slotComponent = Util.GetOrAddComponent<UI_InventorySlot>(slotObj);
        slotComponent.Init();
        slotComponent.SetSlotInfo(index, this);
        
        _slotComponents.Add(slotComponent);
    }

    /// <summary>
    /// 데이터와 UI 리스트 동기화 검증
    /// </summary>
    private void ValidateSynchronization()
    {
        if (_inventoryData == null)
            return;

        int dataSlotCount = _inventoryData.MaxSlots;
        int uiSlotCount = _slotComponents.Count;

        if (dataSlotCount != uiSlotCount)
        {
            Debug.LogError($"[UI_Inven] 동기화 오류 감지! Data: {dataSlotCount}, UI Slots: {uiSlotCount}");
        }
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        // 데이터 추가
        bool result = _inventoryData.AddItem(itemData, quantity);
        
        if (result)
        {
            // 성공 시 UI 갱신
            for (int i = 0; i < _inventoryData.MaxSlots; i++)
            {
                RefreshSlotUI(i);
            }
        }
        else
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
        }
        
        return result;
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        // 데이터 제거
        bool result = _inventoryData.RemoveItem(slotIndex, quantity);
        
        if (result)
        {
            // 성공 시 해당 슬롯 UI 갱신
            RefreshSlotUI(slotIndex);
        }
        
        return result;
    }

    /// <summary>
    /// 특정 슬롯 UI 갱신
    /// </summary>
    private void RefreshSlotUI(int index)
    {
        if (index < 0 || index >= _inventoryData.MaxSlots)
            return;

        // UI 리스트가 아직 확장되지 않은 경우 대기
        if (index >= _slotComponents.Count)
        {
            Debug.LogWarning($"[UI_Inven] UI 리스트가 아직 확장되지 않음. Index: {index}, Count: {_slotComponents.Count}");
            return;
        }

        InventorySlot slotData = _inventoryData.GetSlot(index);
        UI_InventorySlot slotComponent = _slotComponents[index];
        
        if (slotData.IsEmpty)
        {
            // 슬롯이 비었으면 UI 제거
            slotComponent.ClearItem();
        }
        else
        {
            // 슬롯에 아이템이 있으면 UI 생성 또는 갱신
            slotComponent.SetItem(slotData.itemData, slotData.quantity);
        }
    }

    /// <summary>
    /// 슬롯 드롭 처리 (UI_InventorySlot에서 호출)
    /// </summary>
    public void HandleSlotDrop(PointerEventData eventData, int slotIndex)
    {
        if (_draggedItem == null)
            return;
        
        int draggedIndex = _draggedItem.SlotIndex;
        
        // 같은 슬롯에 드롭
        if (draggedIndex == slotIndex)
        {
            ResetItemPosition(_draggedItem, draggedIndex);
            _draggedItem = null;
            return;
        }
        
        // 데이터 레이어에서 아이템 이동 처리
        _inventoryData.MoveItem(draggedIndex, slotIndex);
        
        // 두 슬롯 UI 갱신
        RefreshSlotUI(draggedIndex);
        RefreshSlotUI(slotIndex);

        _draggedItem = null;
    }

    /// <summary>
    /// 아이템을 원래 위치로 복귀
    /// </summary>
    private void ResetItemPosition(UI_Item item, int slotIndex)
    {
        if (item == null || slotIndex < 0 || slotIndex >= _inventoryData.MaxSlots)
            return;

        if (slotIndex >= _slotComponents.Count)
            return;

        Transform slotTransform = _slotComponents[slotIndex].transform;
        item.transform.SetParent(slotTransform);
        item.transform.localPosition = Vector3.zero;
    }

    public void SetDraggedItem(UI_Item item)
    {
        _draggedItem = item;
    }

    public void ClearDraggedItem()
    {
        _draggedItem = null;
    }

    public UI_Item GetDraggedItem()
    {
        return _draggedItem;
    }

    /// <summary>
    /// 아이템 정보 팝업 표시
    /// </summary>
    public void ShowItemInfo(ItemData itemData)
    {
        if (itemData == null)
            return;

        // 기존 팝업이 있으면 먼저 정리
        CloseItemInfoPanel();
        
        _itemInfoPanel = Managers.UI.ShowPopupUI<UI_ItemInfo>(InventoryConfig.ITEM_INFO_PREFAB_PATH);
        if (_itemInfoPanel == null)
        {
            Debug.LogError("UI_ItemInfo 팝업을 생성할 수 없습니다.");
            return;
        }
        
        _itemInfoPanel.Init();
        _itemInfoPanel.SetInfo(itemData);
    }

    /// <summary>
    /// 아이템 정보 패널 닫기
    /// </summary>
    private void CloseItemInfoPanel()
    {
        if (_itemInfoPanel != null)
        {
            Managers.UI.ClosePopupUI(_itemInfoPanel);
            _itemInfoPanel = null;
        }
    }

    /// <summary>
    /// 인벤토리 확장
    /// </summary>
    public bool ExpandInventory()
    {
        if (_inventoryData.ExpansionLevel >= InventoryConfig.MAX_EXPANSION_LEVEL) 
        {
            Debug.Log("인벤토리 확장 불가. 최대 확장 레벨에 도달했습니다.");
            return false;
        }
        
        int additionalSlots = InventoryConfig.GetExpansionSlots(_inventoryData.ExpansionLevel);
        
        // 확장 전 슬롯 수 (_inventoryData 기준)
        int previousSlotCount = _inventoryData.MaxSlots;
        
        // 데이터 확장
        if (_inventoryData.ExpandInventory(additionalSlots))
        {
            // UI 슬롯 추가 (새로 추가된 슬롯만)
            for (int i = previousSlotCount; i < _inventoryData.MaxSlots; i++)
            {
                CreateSlot(i);
            }
            
            // 동기화 검증
            ValidateSynchronization();
            
            Debug.Log($"인벤토리 확장 완료. 총 슬롯 수: {_inventoryData.MaxSlots}");
            return true;
        }
        
        return false;
    }

    public int GetMaxSlots() => _inventoryData?.MaxSlots ?? 0;
    public int GetExpansionLevel() => _inventoryData?.ExpansionLevel ?? 0;
    public int GetOccupiedSlotCount() => _inventoryData?.OccupiedSlotCount ?? 0;

    /// <summary>
    /// 팝업 닫을 때 정리
    /// </summary>
    public override void ClosePopupUI()
    {
        CloseItemInfoPanel();
        base.ClosePopupUI();
    }
}
