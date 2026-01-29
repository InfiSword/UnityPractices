using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 슬롯 데이터 (아이템과 수량 정보)
/// </summary>
[Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;

    public InventorySlot()
    {
        itemData = null;
        quantity = 0;
    }

    public InventorySlot(ItemData itemData, int quantity)
    {
        this.itemData = itemData;
        this.quantity = quantity;
    }

    public bool IsEmpty => itemData == null || quantity <= 0;

    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }

    public void SetItem(ItemData item, int qty)
    {
        itemData = item;
        quantity = qty;
    }
}

/// <summary>
/// 인벤토리 데이터 모델 (UI와 분리된 순수 데이터)
/// </summary>
public class InventoryData
{
    private List<InventorySlot> _slots;
    private int _maxSlots;
    private int _expansionLevel;

    public int MaxSlots => _maxSlots;
    public int ExpansionLevel => _expansionLevel;
    public int OccupiedSlotCount
    {
        get
        {
            int count = 0;
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty)
                    count++;
            }
            return count;
        }
    }

    public InventoryData(int initialSlots)
    {
        _maxSlots = initialSlots;
        _expansionLevel = 0;
        _slots = new List<InventorySlot>(_maxSlots);

        for (int i = 0; i < _maxSlots; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 특정 슬롯의 데이터 가져오기
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= _slots.Count)
            return null;
        return _slots[index];
    }

    /// <summary>
    /// 아이템 추가 (빈 슬롯에 추가하거나 기존 아이템에 수량 증가)
    /// </summary>
    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity <= 0)
            return false;

        // 같은 아이템이 있는 슬롯 찾기 (스택 가능한 경우)
        if (IsStackable(itemData))
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (!_slots[i].IsEmpty && _slots[i].itemData.id == itemData.id)
                {
                    _slots[i].quantity += quantity;
                    return true;
                }
            }
        }

        // 빈 슬롯 찾기
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].IsEmpty)
            {
                _slots[i].SetItem(itemData, quantity);
                return true;
            }
        }

        return false; // 인벤토리 가득 참
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
            return false;

        InventorySlot slot = _slots[slotIndex];
        if (slot.IsEmpty)
            return false;

        slot.quantity -= quantity;
        if (slot.quantity <= 0)
        {
            slot.Clear();
        }

        return true;
    }

    /// <summary>
    /// 두 슬롯의 아이템 교환 (내부 전용)
    /// </summary>
    private void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _slots.Count ||
            toIndex < 0 || toIndex >= _slots.Count)
            return;

        InventorySlot temp = _slots[fromIndex];
        _slots[fromIndex] = _slots[toIndex];
        _slots[toIndex] = temp;
    }

    /// <summary>
    /// 슬롯 간 아이템 이동 (스택 가능한 경우 합치기)
    /// </summary>
    public bool MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _slots.Count ||
            toIndex < 0 || toIndex >= _slots.Count)
            return false;

        InventorySlot fromSlot = _slots[fromIndex];
        InventorySlot toSlot = _slots[toIndex];

        if (fromSlot.IsEmpty)
            return false;

        // 대상 슬롯이 비어있으면 단순 이동
        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.itemData, fromSlot.quantity);
            fromSlot.Clear();
            return true;
        }

        // 같은 아이템이고 스택 가능하면 합치기
        if (toSlot.itemData.id == fromSlot.itemData.id && IsStackable(fromSlot.itemData))
        {
            toSlot.quantity += fromSlot.quantity;
            fromSlot.Clear();
            return true;
        }

        // 다른 아이템이면 교환
        SwapSlots(fromIndex, toIndex);
        return true;
    }

    /// <summary>
    /// 인벤토리 확장
    /// </summary>
    public bool ExpandInventory(int additionalSlots)
    {
        if (additionalSlots <= 0)
            return false;

        _expansionLevel++;
        _maxSlots += additionalSlots;

        for (int i = 0; i < additionalSlots; i++)
        {
            _slots.Add(new InventorySlot());
        }

        return true;
    }

    /// <summary>
    /// 아이템이 스택 가능한지 확인 (현재는 소모품만)
    /// </summary>
    private bool IsStackable(ItemData itemData)
    {
        return itemData.itemType == ItemType.Consumable;
    }

    /// <summary>
    /// 특정 아이템의 총 수량 계산
    /// </summary>
    public int GetTotalQuantity(int itemId)
    {
        int total = 0;
        foreach (var slot in _slots)
        {
            if (!slot.IsEmpty && slot.itemData.id == itemId)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    public void Clear()
    {
        foreach (var slot in _slots)
        {
            slot.Clear();
        }
    }
}
