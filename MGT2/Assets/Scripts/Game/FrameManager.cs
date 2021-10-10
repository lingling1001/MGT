using MFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : Singleton<FrameManager>
{
    public const int DEFAULT_SPEED = 10;
    private string _strSyl = "X";
    private List<FrameSpeedData> _listMsSpeed;
    private TaskAsyncThreadFrame _taskInfo = null;
    public TaskAsyncThreadFrame TaskInfo { get { return _taskInfo; } }

    private FrameSpeedData _speedInfo;
    public FrameSpeedData SpeedInfo { get { return _speedInfo; } }
    protected override void OnInit()
    {
        Application.targetFrameRate = 60;
        _taskInfo = new TaskAsyncThreadFrame(1, 1);
        SetIntervalMs(GetSpeedDatas()[0]);
        TaskAsynManager.Instance.AdditionTask(_taskInfo);
    }

    public void AutoChangeSpeed()
    {
        int index = SpeedInfo.Index + 1;
        if (index >= GetSpeedDatas().Count)
        {
            index = 0;
        }
        SetIntervalMs(GetSpeedDatas()[index]);
    }


    /// <summary>
    /// 设置帧率间隔
    /// </summary>
    public void SetIntervalMs(FrameSpeedData info)
    {
        _speedInfo = info;
        _taskInfo.SetIntervalMs(info.Speed);

    }

    /// <summary>
    /// 获取每秒运行间隔
    /// </summary>
    public int GetIntervalMs()
    {
        return _taskInfo.IntervalMs;
    }
    public long GetCurFrameCount()
    {
        return _taskInfo.CurFrameCount;
    }
    public string GetCurrentTime()
    {
        return Utility.ToDayHourMinuteSecondString((int)(_taskInfo.CurFrameCount / DEFAULT_SPEED));
    }
    public string GetTimeSumRun()
    {
        return Utility.ToDayHourMinuteSecondString((int)(_taskInfo.StopWatchInfo.ElapsedMilliseconds / 1000));
    }

    public string GetCurFrameName()
    {
        return _strSyl + (_speedInfo.Index + 1);
    }
    private List<FrameSpeedData> GetSpeedDatas()
    {
        if (_listMsSpeed == null)
        {
            _listMsSpeed = new List<FrameSpeedData>();
            _listMsSpeed.Add(new FrameSpeedData(100, 0));//1倍素
            _listMsSpeed.Add(new FrameSpeedData(50, 1));//2倍速
            _listMsSpeed.Add(new FrameSpeedData(25, 2));//4倍速
        }
        return _listMsSpeed;

    }
    protected override void OnRelease()
    {
        TaskAsynManager.InstanceRelease();
    }
}

public class FrameSpeedData
{
    public int Speed;
    public int Index;
    public FrameSpeedData(int speed, int index)
    {
        Speed = speed;
        Index = index;
    }
}