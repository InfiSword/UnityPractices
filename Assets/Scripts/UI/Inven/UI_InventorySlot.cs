using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 슬롯 UI 컴포넌트
/// 각 슬롯이 자신의 UI_Item을 관리하고 드래그/드롭 이벤트를 처리
/// </summary>
public class UI_InventorySlot : UI_Base
{
    private int _slotIndex;
    private UI_Inven _parentInventory;
    private UI_Item _currentItem;

    public int SlotIndex => _slotIndex;
    public bool IsEmpty => _currentItem == null;
    public UI_Item CurrentItem => _currentItem;

    public override void Init()
    {
        // 드롭 이벤트 바인딩
        BindEvent(gameObject, OnDrop, Define.UIEvent.Drop);
    }

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    public void SetSlotInfo(int index, UI_Inven parentInventory)
    {
        _slotIndex = index;
        _parentInventory = parentInventory;
    }

    /// <summary>
    /// 슬롯에 아이템 설정
    /// </summary>
    public void SetItem(ItemData itemData, int quantity)
    {
        if (itemData == null || quantity <= 0)
        {
            ClearItem();
            return;
        }

        // 아이템이 없으면 생성
        if (_currentItem == null)
        {
            CreateItem();
        }

        // 아이템 정보 갱신
        _currentItem.SetInfo(itemData, quantity, _slotIndex, _parentInventory);
    }

    /// <summary>
    /// 슬롯의 아이템 UI 생성
    /// </summary>
    private void CreateItem()
    {
        GameObject itemObj = Managers.Resource.Instantiate(InventoryConfig.ITEM_PREFAB_PATH);
        itemObj.transform.SetParent(transform);
        itemObj.transform.localScale = Vector3.one;

        RectTransform itemRect = itemObj.GetComponent<RectTransform>();
        if (itemRect != null)
        {
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = Vector2.zero;
        }

        _currentItem = Util.GetOrAddComponent<UI_Item>(itemObj);
        _currentItem.Init();
    }

    /// <summary>
    /// 슬롯의 아이템 제거
    /// </summary>
    public void ClearItem()
    {
        if (_currentItem != null)
        {
            Managers.Resource.Destroy(_currentItem.gameObject);
            _currentItem = null;
        }
    }

    /// <summary>
    /// 드롭 이벤트 처리
    /// </summary>
    private void OnDrop(PointerEventData eventData)
    {
        if (_parentInventory == null)
            return;

        _parentInventory.HandleSlotDrop(eventData, _slotIndex);
    }

    /// <summary>
    /// 슬롯 하이라이트 (선택적 기능)
    /// </summary>
    public void SetHighlight(bool highlight)
    {
        // 추후 슬롯 하이라이트 효과 추가 가능
        // 예: 테두리 색상 변경, 크기 확대 등
    }
}
