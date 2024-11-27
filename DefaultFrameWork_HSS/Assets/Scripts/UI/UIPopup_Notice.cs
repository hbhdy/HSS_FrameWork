using HSS;
using UnityEngine;

public class UIPopup_Notice : UIBase
{
    // ----- Param -----

    public override UIType UIType { get { return UIType.UIPopup_Notice; } }

    // ----- Init -----

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    // ----- Main ----- 

    protected override void OpenUI()
    {
        base.OpenUI();
    }

    protected override void CloseUI()
    {
        base.CloseUI();
    }

    public void Close()
    {
        CloseUI();
    }
}
