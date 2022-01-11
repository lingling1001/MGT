using MFrameWork;
/// <summary>
/// 游戏进程状态
/// </summary>
public class GameStateManager : Singleton<GameStateManager>
{
    public FsmManagerGame FsmGameState { get { return _fsmGameState; } }
    private FsmManagerGame _fsmGameState;
    protected override void  OnInit()
    {
        _fsmGameState = new FsmManagerGame();
        _fsmGameState.OnInit();
    }
    public bool GameIsRuning()
    {
        if (FsmGameState == null)
        {
            return false;
        }
        return FsmGameState.CurrentState.Name == FsmManagerGame.GAME_STATE_START;
    }
    public void ChangeState(string strState)
    {
        if (FsmGameState == null)
        {
            return ;
        }
        FsmGameState.ChangeState(strState);
    }
    protected override void OnRelease()
    {
        if (_fsmGameState != null)
        {
            _fsmGameState.OnRelease();
        }
    }
}
