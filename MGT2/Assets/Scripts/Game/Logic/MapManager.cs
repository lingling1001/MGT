using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private FindPathTools _findPathTools;
    protected override void OnInit()
    {
        _findPathTools = new FindPathTools();
    }
    /// <summary>
    /// ���õ�ͼ��Ϣ
    /// </summary>
    public void SetMapInfo(int width,int heigh,string strContent)
    {




    }
    protected override void OnRelease()
    {
        _findPathTools.OnRelease();
        _findPathTools = null;
    }
}
