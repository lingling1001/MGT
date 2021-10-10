using UnityEngine.UI;

public class MonoPoolClickBase<T> : MonoPoolItem
{
    private System.Action<T> _callBack;
    private Button _btnEvent;
    public void SetButton(Button btn)
    {
        _btnEvent = btn;

    }
    private void OnClickThis(Button btn)
    {
       
    }
    public void SetClickEvent(System.Action<T> callback)
    {
        _callBack = callback;
    }

}

