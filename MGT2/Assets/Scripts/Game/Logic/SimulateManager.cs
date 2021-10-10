using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateManager : Singleton<SimulateManager>
{
    protected override void OnInit()
    {
        MapManager.InstanceInitial();
        WorldEntityManager.InstanceInitial();
        FrameManager.InstanceInitial();
    }
    protected override void OnRelease()
    {
        WorldEntityManager.InstanceRelease();
        MapManager.InstanceRelease();
        FrameManager.InstanceRelease();
    }
}
