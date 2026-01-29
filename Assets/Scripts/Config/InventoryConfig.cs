using UnityEngine;

/// <summary>
/// 인벤토리 설정 값 (매직 넘버 제거)
/// </summary>
public static class InventoryConfig
{
    // 슬롯 설정
    public const int INITIAL_SLOTS = 20;
    public const int MAX_EXPANSION_LEVEL = 2;
    
    // 확장 레벨별 추가 슬롯 수
    public static readonly int[] EXPANSION_SLOTS = { 20, 20 }; // 레벨 1: +20 (총 40), 레벨 2: +20 (총 60)
    
    // UI 설정
    public const float DRAG_ALPHA = 0.5f;
    public const float NORMAL_ALPHA = 1.0f;
    
    // 드래그 설정
    public const bool DRAG_FROM_CENTER = true; // true: 아이템 중앙을 커서에 맞춤, false: 클릭 위치 유지
    
    // 리소스 경로
    public const string SLOT_PREFAB_PATH = "UI/Popup/UI_Inven_Slot";
    public const string ITEM_PREFAB_PATH = "UI/Item/UI_Item";  // UI_Inven_Item → UI_Item
    public const string ITEM_INFO_PREFAB_PATH = "UI_ItemInfo";
    
    /// <summary>
    /// 다음 레벨로 확장 시 추가될 슬롯 수
    /// </summary>
    public static int GetExpansionSlots(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= EXPANSION_SLOTS.Length)
            return 0;
        return EXPANSION_SLOTS[currentLevel];
    }
}
