using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Singleton<T> : ISingleton<T> where T : class, ISingleton<T>, new()
{
    //单例模式
    private static T _instance;
    //线程安全使用
    private static readonly object _lockObj = new object();

    public static T Instance
    {
        get
        {
            if (null == _instance)
            {
                lock (_lockObj)
                {
                    if (null == _instance)
                    {
                        _instance = new T();
                        _instance.On_Init();
                    }
                }
            }
            return _instance;
        }
    }
    public void On_Init()
    {
        OnInit();
    }

    public void On_Release()
    {
        OnRelease();
    }
    protected virtual void OnInit()
    {

    }
    protected virtual void OnRelease()
    {

    }

    public T GetInstance()
    {
        return Instance;
    }
    public static bool InstanceIsNull()
    {
        return _instance == null;
    }
    public static void InstanceRelease()
    {
        if (!InstanceIsNull())
        {
            _instance.On_Release();
            _instance = null;
        }
    }



}


