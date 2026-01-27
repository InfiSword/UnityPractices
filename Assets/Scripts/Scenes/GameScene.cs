using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();        
        Managers.UI.ShowPopupUI<UI_Inven>().Init();
        Managers.UI.ShowPopupUI<UI_CanvasTest>().Init();
    }
    public override void Clear()
    {
        
    }
}
