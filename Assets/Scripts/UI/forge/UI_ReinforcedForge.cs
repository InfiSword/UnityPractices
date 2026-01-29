using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 강화 대장간 UI
/// - IItemDropTarget 인터페이스를 구현하여 아이템 드롭 처리
/// - 슬롯에 인벤토리 아이템을 드래그 앤 드롭
/// - 강화 버튼 클릭 시 성공/실패 처리
/// </summary>
public class UI_ReinforcedForge : UI_Popup, IItemDropTarget
{
    enum GameObjects
    {
        Slot,               // 아이템을 올려둘 슬롯
        Button_Reinforce,   // 강화 버튼
        DragArea,           // 상단 드래그 영역
    }

    private UI_Item _slotItem;          // 슬롯에 표시할 아이템 UI
    private ItemData _targetItem;       // 강화 대상 아이템 데이터

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));

        // 슬롯 초기화
        GameObject slotObj = Get<GameObject>((int)GameObjects.Slot);
        if (slotObj != null)
        {
            _slotItem = slotObj.GetComponent<UI_Item>();
            if (_slotItem != null)
            {
                _slotItem.Init();
            }
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
    }

    #region IItemDropTarget 구현

    /// <summary>
    /// 아이템 드롭 가능 여부 확인
    /// </summary>
    public bool CanAcceptDrop(UI_Item item)
    {
        if (item == null || item.ItemData == null)
            return false;

        // 무기 아이템만 강화 가능
        if (item.ItemData.itemType != ItemType.Weapon)
        {
            Debug.Log($"[Reinforced Forge] 무기 아이템만 강화 가능합니다. (현재: {item.ItemData.itemType})");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 아이템 드롭 처리
    /// </summary>
    public bool OnItemDropped(UI_Item item)
    {
        if (item == null || item.ItemData == null)
            return false;

        // 드롭 가능 여부 재확인
        if (!CanAcceptDrop(item))
            return false;

        // 강화 대상 아이템 등록
        _targetItem = item.ItemData;

        // 슬롯 UI에 아이템 정보 표시
        if (_slotItem != null)
        {
            // 대장간 슬롯은 인벤이 아니므로 슬롯 인덱스 0, 인벤 참조 null 전달
            _slotItem.SetInfo(_targetItem, 1, 0, null);
        }

        Debug.Log($"[Reinforced Forge] 슬롯에 아이템 '{_targetItem.name}' 등록 완료");
        return true;
    }

    #endregion

    /// <summary>
    /// 강화 버튼 클릭 처리
    /// </summary>
    private void OnClickReinforce()
    {
        if (_targetItem == null)
        {
            Debug.Log("[Reinforced Forge] 강화할 아이템이 없습니다.");
            return;
        }

        // 강화 처리 (50% 확률)
        float successRate = 0.5f;
        bool isSuccess = Random.value < successRate;

        if (isSuccess)
        {
            Debug.Log($"[Reinforced Forge] ✅ '{_targetItem.name}' 강화 성공!");
            OnReinforceSuccess();
        }
        else
        {
            Debug.Log($"[Reinforced Forge] ❌ '{_targetItem.name}' 강화 실패...");
            OnReinforceFailed();
        }
    }

    /// <summary>
    /// 강화 성공 처리
    /// </summary>
    private void OnReinforceSuccess()
    {
        // 무기 아이템의 경우 공격력 증가
        if (_targetItem is WeaponItemData weaponData)
        {
            int previousAttack = weaponData.stats.attackPower;
            weaponData.stats.attackPower += 5;
            Debug.Log($"[Reinforced Forge] 공격력 증가: {previousAttack} → {weaponData.stats.attackPower}");
        }

        // TODO: 아이템 정보 갱신, 이펙트 효과 등
    }

    /// <summary>
    /// 강화 실패 처리
    /// </summary>
    private void OnReinforceFailed()
    {
        // TODO: 실패 이펙트, 내구도 감소 등
    }

    /// <summary>
    /// 슬롯 초기화 (아이템 제거)
    /// </summary>
    public void ClearSlot()
    {
        _targetItem = null;
        
        if (_slotItem != null)
        {
            _slotItem.SetInfo(null, 0, 0, null);
        }
    }
}
