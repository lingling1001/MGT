using HedgehogTeam.EasyTouch;
using MFrameWork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class UIJoystick : BaseUI, IUpdate
{
    private ETCJoystick _ectJoystick;
    private AssemblyCache _assemblyCacheView;
    private CameraManager _cameraManager;

    public int Priority => DefinePriority.NORMAL;

    public EnumFixedUpdateOrder Order => EnumFixedUpdateOrder.Third;

    public override void OnInit()
    {
        //EasyTouch.On_Swipe += EventOnSwipe;
        //EasyTouch.On_SimpleTap += EventOnSimpleTap;
        // _ectJoystick = ObjUI.GetComponentInChildren<ETCJoystick>();
        RegisterInterfaceManager.RegisteUpdate(this);
        _cameraManager = GameManager<CameraManager>.QGetOrAddMgr();
        GetBindComponents(ObjUI);
        RefreshContent();

    }
    public void On_Update(float elapseSeconds, float realElapseSeconds)
    {
        Gesture gesture = EasyTouch.current;
        if (gesture == null)
        {
            return;
        }
        if (gesture.type == EasyTouch.EvtType.On_Drag)
        {
            EventOnDrag(gesture);
        }

    }

  

    private void EventOnDrag(Gesture gesture)
    {
        _cameraManager.ResetCameraFollow();
        Vector3 deltaPosition = gesture.deltaPosition;

        //gesture.deltaPosition;
        deltaPosition.Normalize();

        Vector3 target = _cameraManager.FollowPosition - new Vector3(deltaPosition.x, 0, deltaPosition.y);

        Vector3 step = Vector3.MoveTowards(_cameraManager.FollowPosition, target, Time.deltaTime * 10);
        _cameraManager.SetFollowPosition(step);


    }
    private void RefreshContent()
    {




    }
    private int GetClampValue(float value)
    {
        if (value > 0.2f)
        {
            return 1;
        }
        if (value < -0.2f)
        {
            return -1;
        }
        return 0;
    }

    public override void OnRelease()
    {
        //EasyTouch.On_Swipe -= EventOnSwipe;
        //EasyTouch.On_SimpleTap -= EventOnSimpleTap;
        RegisterInterfaceManager.UnRegisteUpdate(this);

        base.OnRelease();
    }


}

