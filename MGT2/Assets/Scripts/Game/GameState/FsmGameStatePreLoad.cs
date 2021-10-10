using com.ootii.Messages;
using MFrameWork;
using System;

public class FsmGameStatePreLoad : FsmBase
{
    public FsmGameStatePreLoad(FsmManager fasManager, string strName) : base(fasManager, strName)
    {
    }

    public override void OnInit()
    {
        NotificationManager.AddListener(NotificationName.EventLoadFinish, EventLoadFinish);
        base.OnInit();
    }
    public override void OnRelease()
    {
        NotificationManager.RemoveListener(NotificationName.EventLoadFinish, EventLoadFinish);
        base.OnRelease();
    }

    private void EventLoadFinish(IMessage rMessage)
    {
        PrototypeHelper.LoadAllData();

        ChangeState(FsmManagerGame.GAME_STATE_START);
    }

    public override void OnEnter()
    {
        PreResLoadHelper.Instance.OnInitPreLoad();
    }

}
