using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
public class TaskAsyncThreadFrame : ITaskAsyncable
{
    public int TaskId { get; private set; }

    public TaskAsynStatus TaskStatus { get; private set; }

    private Thread _thread;
    private EventWaitHandle _eventWait = new EventWaitHandle(false, EventResetMode.ManualReset);
    /// <summary>
    /// 计时
    /// </summary>
    private Stopwatch _stopWatch = new Stopwatch();
    /// <summary>
    /// 计时类
    /// </summary>
    public Stopwatch StopWatchInfo { get { return _stopWatch; } }
    /// <summary>
    /// 运行间隔 毫秒
    /// </summary>
    private int _intervalMs;
    public int IntervalMs { get { return _intervalMs; } }
    /// <summary>
    /// 当前执行帧数，可能会跳帧
    /// </summary>
    private long _curFrameCount;
    public long CurFrameCount { get { return _curFrameCount; } }
    /// <summary>
    /// 上次暂停时间
    /// </summary>
    private long _pauseTimeLast;
    /// <summary>
    /// 暂停总时间
    /// </summary>
    private long _pauseTimeSum;
    /// <summary>
    /// 回调
    /// </summary>
    public System.Action<long, long> EventExecute;
    /// <summary>
    /// 循环执行
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="intervalMs">间隔毫秒</param>
    public TaskAsyncThreadFrame(int taskId, int intervalMs)
    {

        TaskId = taskId;
        _thread = new Thread(Execute);
        SetIntervalMs(intervalMs);
        SetRunStatus(TaskAsynStatus.None);
    }

    public bool Start()
    {
        if (TaskStatus == TaskAsynStatus.None)
        {
            //重置计时
            SetRunStatus(TaskAsynStatus.Run);
            _curFrameCount = 0;
            _stopWatch.Restart();
            _thread.Start();
            OnExecute(0, 0);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public bool Suspend()
    {
        SetRunStatus(TaskAsynStatus.Suspend);
        return true;
    }
    /// <summary>
    /// 继续
    /// </summary>
    public bool Resume()
    {
        SetRunStatus(TaskAsynStatus.Run);
        return true;

    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        SetRunStatus(TaskAsynStatus.Stop);
    }

    public bool IsRuning()
    {
        return TaskStatus == TaskAsynStatus.Run || TaskStatus == TaskAsynStatus.Running;
    }
    protected void SetRunStatus(TaskAsynStatus status)
    {
        TaskStatus = status;
    }

    /// <summary>
    /// Sets the interval ms.设置 每秒刷新间隔
    /// </summary>
    public void SetIntervalMs(int intervalMs)
    {
        _intervalMs = intervalMs;
    }
    private long _tempTimeReal;
    private long _tempTimeSub;
    private long _tempTimeLast;

    private void Execute()
    {
        while (true)
        {
            if (TaskStatus == TaskAsynStatus.Running)
            {
                //真实当前时间=当前时间-上次多出的时间差
                _tempTimeReal = _stopWatch.ElapsedMilliseconds - _pauseTimeSum;
                //时间差值= 当前时间 减去 上次的时间
                _tempTimeSub = _tempTimeReal - _tempTimeLast;
                //差值大于运行间隔 执行逻辑, 
                if (_tempTimeSub >= IntervalMs)
                {
                    //当前计数=当前计数+ 时间差值/运行间隔（可能跳帧的关键）
                    _curFrameCount = _curFrameCount + _tempTimeSub / IntervalMs;
                    //记录当前时间
                    _tempTimeLast = _tempTimeReal - _tempTimeSub % IntervalMs;

                    OnExecute(CurFrameCount, _tempTimeLast);
                }
                Thread.Sleep(IntervalMs);
            }
            else if (TaskStatus == TaskAsynStatus.Run)
            {
                _eventWait.Set();
                _pauseTimeSum = _pauseTimeSum + _stopWatch.ElapsedMilliseconds - _pauseTimeLast;
                //_stopWatch.Start();
                SetRunStatus(TaskAsynStatus.Running);
            }
            else if (TaskStatus == TaskAsynStatus.Suspend)
            {
                _pauseTimeLast = _stopWatch.ElapsedMilliseconds;
                _eventWait.WaitOne();
                SetRunStatus(TaskAsynStatus.Suspended);
            }
            else if (TaskStatus == TaskAsynStatus.Suspended)
            {
                continue;
            }
            else if (TaskStatus == TaskAsynStatus.Stop)
            {
                break;
            }
        }
        _thread.Abort();
        _thread = null;
    }

    private void OnExecute(long frameCount, long ticks)
    {
        if (EventExecute != null)
        {
            EventExecute(frameCount, ticks);
        }
    }
}
