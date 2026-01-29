using UnityEngine;

/// <summary>
/// 아이템 드롭을 받을 수 있는 UI가 구현할 인터페이스
/// </summary>
public interface IItemDropTarget
{
    /// <summary>
    /// 아이템이 드롭 가능한지 확인
    /// </summary>
    /// <param name="item">드롭하려는 아이템 UI</param>
    /// <returns>드롭 가능하면 true</returns>
    bool CanAcceptDrop(UI_Item item);
    
    /// <summary>
    /// 아이템이 드롭되었을 때 호출
    /// </summary>
    /// <param name="item">드롭된 아이템 UI</param>
    /// <returns>드롭 처리 성공 여부</returns>
    bool OnItemDropped(UI_Item item);
}

