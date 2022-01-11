using com.ootii.Messages;
using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>, IUpdate
{
    public int Priority { get { return 50; } }

    protected override void OnInit()
    {
        RegisterInterfaceManager.RegisteUpdate(this);
    }

    protected override void OnRelease()
    {
        MessageDispatcher.ClearMessages();
        MessageDispatcher.ClearListeners();
        RegisterInterfaceManager.UnRegisteUpdate(this);
    }

    public static void SendMessage(string strType)
    {
        MessageDispatcher.SendMessage(strType);
    }

    public static void AddListener(string strType, MessageHandler rHandler)
    {
        MessageDispatcher.AddListener(strType, rHandler);
    }
    public static void RemoveListener(string strType, MessageHandler rHandler)
    {
        MessageDispatcher.RemoveListener(strType, rHandler);
    }

    public void On_Update(float elapseSeconds, float realElapseSeconds)
    {
        MessageDispatcher.Update();
    }
}
