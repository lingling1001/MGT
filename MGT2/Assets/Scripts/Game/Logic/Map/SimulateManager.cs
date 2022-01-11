using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateManager : Singleton<SimulateManager>
{
    protected override void OnInit()
    {
        MapManager.InstanceInitial();
        

    }
   
    protected override void OnRelease()
    {
        MapManager.InstanceRelease();
      
    }
}
