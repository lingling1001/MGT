using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIWorldOperateRole : MonoPoolItem, IWorldNodeable
{
    public List<UIWorldOperateItem> _listMenuItems = new List<UIWorldOperateItem>();
    public EnumWorldResNode ResNodeType { get; private set; }
    private AssemblyCache _assemblyCache;
    private void Awake()
    {
        GetBindComponents(gameObject);
        EventHelper.RegistEvent(m_Btn_Event, EventClickItem);
    }

    private void EventClickItem(Button obj)
    {
        GameManager<CameraManager>.QGetMgr().SetFollowTarget(_assemblyCache.AssemblyView.Trans);



    }

    public void OnInit(EnumWorldResNode type, AssemblyCache resInfo)
    {
        _assemblyCache = resInfo;
        RefreshItemRole.Refresh(m_Scr_RoleItem, resInfo.AssyRoleInfo);
        if (resInfo.AssyRoleControl != null)
        {
            AddMenus(EnumWorldResTP.Move);
            AddMenus(EnumWorldResTP.Attack);
            AddMenus(EnumWorldResTP.Guard);
            AddMenus(EnumWorldResTP.Infomation);
        }
        else
        {
            AddMenus(EnumWorldResTP.Infomation);
        }

    }

    private void AddMenus(EnumWorldResTP type)
    {
        string strPath = AssetsName.UIWorldOperateItem;
        Transform parent = m_GGroup_Node.transform;
        UIWorldOperateItem item = ItemPoolMgr.CreateOrGetItem<UIWorldOperateItem>(strPath, parent);
        item.SetClickEvent(EventClickMenu);
        item.SetData(type, _assemblyCache);
        _listMenuItems.Add(item);
    }

    private void EventClickMenu(UIWorldOperateItem obj)
    {
        switch (obj.OperateType)
        {
            case EnumWorldResTP.Move:
                OnClickMove();
                break;
            case EnumWorldResTP.Attack:
                OnClickAttack();
                break;
            case EnumWorldResTP.Guard:
                break;
            case EnumWorldResTP.Infomation:
                break;
            default:
                break;
        }


    }

    private void OnClickMove()
    {


    }

    private void OnClickAttack()
    {

    }
    public void OnRelease()
    {
        for (int cnt = 0; cnt < _listMenuItems.Count; cnt++)
        {
            ItemPoolMgr.AddPool(_listMenuItems[cnt]);
        }
        _listMenuItems.Clear();
    }

}
