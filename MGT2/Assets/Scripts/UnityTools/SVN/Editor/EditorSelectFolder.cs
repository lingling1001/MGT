using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorSelectFolder : EditorWindow
{
    private List<string> _listSelects = new List<string>();
    private System.Action<List<string>> _callback;
    private void Initial(string[] list, System.Action<List<string>> callback)
    {
        _listSelects.Clear();
        if (list != null)
        {
            _listSelects.AddRange(list);
        }
        _callback = callback;
    }
    void OnGUI()
    {
        if (DrawDragContent())
        {
            return;
        }
        GUILayout.Label("---------------------------------------");
        DrawFolderList();

    }
    private Vector2 _vbar;
    private void DrawFolderList()
    {
        _vbar = EditorGUILayout.BeginScrollView(_vbar);
        EditorGUILayout.BeginVertical();

        for (int cnt = 0; cnt < _listSelects.Count; cnt++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(_listSelects[cnt]))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath(_listSelects[cnt], typeof(Object));
            }
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                RemoveToSelectList(_listSelects[cnt]);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

    }

    private void AddToSelectList(string strPath)
    {
        RemoveToSelectList(strPath, false);
        _listSelects.Add(strPath);
        _callback.InvokeGracefully(_listSelects);
    }
    private void RemoveToSelectList(string strPath, bool isSendMsg = true)
    {
        if (_listSelects.Contains(strPath))
        {
            _listSelects.Remove(strPath);
        }
        if (isSendMsg)
        {
            _callback.InvokeGracefully(_listSelects);
        }
    }

    private string _lastFileName;
    private Object _lastFileObj;
    private Rect _rectDrag;
    private bool _isDraging;
    private bool DrawDragContent()
    {
        GUILayout.Label(" 将文件拖入下方空白区域");
        _rectDrag = EditorGUILayout.GetControlRect(GUILayout.Width(500), GUILayout.Height(50));
        bool isDrag = false;
        if (Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            isDrag = true;
        }
        else if (Event.current.type == EventType.DragExited)
        {
            isDrag = false;
            _isDraging = false;
        }
        //第一次拖动
        if (isDrag != _isDraging)
        {
            _isDraging = isDrag;
            //改变鼠标的外表  
            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
            {
                return DragObjectInfos(DragAndDrop.objectReferences);
            }
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                return DragObjectInfos(DragAndDrop.paths);
            }
        }
        return false;
    }
    private bool DragObjectInfos(string[] paths)
    {
        if (_lastFileName != DragAndDrop.paths[0])
        {
            for (int cnt = 0; cnt < paths.Length; cnt++)
            {
                Object obj = AssetDatabase.LoadAssetAtPath(paths[cnt], typeof(Object));
                DragObjectInfo(obj);
            }
            _lastFileName = DragAndDrop.paths[0];
            return true;
        }
        return false;
    }

    private bool DragObjectInfos(Object[] objs)
    {
        if (_lastFileObj != objs[0])
        {
            string strLog = "\n";
            string strSucceed = "\n";
            for (int cnt = 0; cnt < objs.Length; cnt++)
            {
                if (DragObjectInfo(objs[cnt]))
                {
                    strSucceed = strSucceed + objs[cnt].name + "\n";
                }
                else
                {
                    strLog = strLog + objs[cnt].name + "\n";
                }
            }
            _lastFileObj = DragAndDrop.objectReferences[0];
            return true;
        }
        return false;
    }

    private bool DragObjectInfo(Object obj)
    {
        if (obj != null)
        {
            AddToSelectList(AssetDatabase.GetAssetPath(obj));
        }
        return true;
    }


    public static void OpenEditorSelectFolder(string[] list, System.Action<List<string>> callback)
    {
        EditorSelectFolder test = new EditorSelectFolder();
        //创建窗口
        EditorSelectFolder myWindow =
            (EditorSelectFolder)EditorWindow.GetWindow(typeof(EditorSelectFolder), false, "EditorSelectFolder", true);
        myWindow.Show();//展示
        myWindow.Initial(list, callback);
    }

}
