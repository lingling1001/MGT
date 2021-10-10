using MFrameWork;

public class FsmGameStateInit : FsmBase
{
    public FsmGameStateInit(FsmManager fasManager, string strName) : base(fasManager, strName)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        GameFrameworkLog.SetLogHelper(new DefaultLogHelper());
        NotificationManager.InstanceInitial();
        UIManager.InstanceInitial();

    }
    public override void OnEnter()
    {
        ChangeState(FsmManagerGame.GAME_STATE_PRELOAD);
    }

    public override void OnRelease()
    {
        UIManager.InstanceRelease();
        NotificationManager.InstanceRelease();
        base.OnRelease();
    }

}
