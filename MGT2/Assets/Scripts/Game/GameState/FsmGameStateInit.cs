using MFrameWork;

public class FsmGameStateInit : FsmBase
{
    public FsmGameStateInit(FsmManager fasManager, string strName) : base(fasManager, strName)
    {

    }
    public override void OnLoad()
    {
        ES3.Init();
        //初始化log
        GameFrameworkLog.SetLogHelper(new DefaultLogHelper());
        NotificationManager.Instance.GetInstance();
        GameManager<ManagerBase>.Instance.GetInstance();

        UIManager.Instance.GetInstance(); 

    }
    public override void OnEnter()
    {
        ChangeState(FsmManagerGame.GAME_STATE_PRELOAD);
    }

    public override void OnRelease()
    {
        GameManager<ManagerBase>.InstanceRelease();

        TaskAsynManager.InstanceRelease();

        UIManager.InstanceRelease();

        NotificationManager.InstanceRelease();


        TransparentNode.ReleaseInstance();

        base.OnRelease();
    }


}
