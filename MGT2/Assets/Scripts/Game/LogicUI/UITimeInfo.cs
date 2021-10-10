using MFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UITimeInfo : BaseUI, IUpdate
{
    public int Priority => 50;

    public override void OnInit()
    {
        base.OnInit();
        GetBindComponents(ObjUI);
       
        m_Btn_Change.onClick.AddListener(OnClickBtnChange);
        m_Btn_Pause.onClick.AddListener(OnClickBtnPause);
        m_Btn_Start.onClick.AddListener(OnClickBtnStart);

        RegisterInterfaceManager.RegisteUpdate(this);
        RefreshSpeed();
        RefreshState();
    }

    private void RefreshSpeed()
    {
        m_Txt_Speed.text = FrameManager.Instance.GetCurFrameName();
    }
    private void RefreshState()
    {
        bool isStart = FrameManager.Instance.TaskInfo.IsRuning();
        NGUITools.SetActive(m_Btn_Start, isStart);
        NGUITools.SetActive(m_Btn_Pause, !isStart);
    }

    public void On_Update(float elapseSeconds, float realElapseSeconds)
    {
        UIHelper.SetText(m_Txt_Time, FrameManager.Instance.GetCurrentTime());
        UIHelper.SetText(m_Txt_TimeReal, FrameManager.Instance.GetTimeSumRun());
    }

    public void OnClickBtnChange()
    {
        FrameManager.Instance.AutoChangeSpeed();
        RefreshSpeed();
    }
    public void OnClickBtnPause()
    {
        FrameManager.Instance.TaskInfo.Resume();
        RefreshState();

    }
    public void OnClickBtnStart()
    {
        FrameManager.Instance.TaskInfo.Suspend();
        RefreshState();
    }
    public override void OnRelease()
    {
        m_Btn_Change.onClick.RemoveListener(OnClickBtnChange);
        m_Btn_Pause.onClick.RemoveListener(OnClickBtnPause);
        m_Btn_Start.onClick.RemoveListener(OnClickBtnStart);
        RegisterInterfaceManager.UnRegisteUpdate(this);
        base.OnRelease();
    }


}
