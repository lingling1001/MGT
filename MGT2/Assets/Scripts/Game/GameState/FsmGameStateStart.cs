using MFrameWork;
using UnityEngine;

public class FsmGameStateStart : FsmBase
{
    public FsmGameStateStart(FsmManager fasManager, string strName) : base(fasManager, strName)
    {

    }

    public override void OnEnter()
    {
        SimulateManager.InstanceInitial();
        UIManager.Instance.OpenUI<UITimeInfo>(AssetsName.UITimeInfo);

    }

    public override void OnLeave()
    {
        SimulateManager.InstanceRelease();
    }

}
