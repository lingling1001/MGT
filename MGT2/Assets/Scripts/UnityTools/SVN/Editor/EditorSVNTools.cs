using MFrameWork;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 提交工具 
/// 1 必须在环境变量中 添加svn 的路径，保证svn 命令可执行 
/// 2 点击刷新列表 调用 svn status 获取项目路径的变更状态 (SVN刷新)
/// 3 注意：提交如果有新增文件 必须要把文件夹勾选上，不然提交失败。(待优化)
/// 4 勾选文件结束并不想提交 可以点击保存临时记录 保存文件提交信息
/// SAVE：保存筛选信息 便捷按钮 保存后需要点击SVN刷新
/// CLS：清除选中文件 需要点击保存临时记录 才会真正的保存
/// SORT: 简单排序 优先级 选中->脚本 ->其它
///
/// T : 类型对应   详情查看 SVNFileInfo 对于类型
/// 自动选中脚本： 自动选中预设附加的变化脚本
/// 自动刷新： 脚本发生变化后编译完成 自动刷新列表（待删除 ）
/// 筛选过滤 * 号分隔 名字包含则过滤掉 
/// 注意：必须筛选不然太卡
/// CLS_LOG log提示 (预留)
/// __TMP*Editor*Firebase*EasyWater*NGUI*DEMO*Plugins*Resources/Data*GameDebug*GameClientConfig*MeshPaint*WSParticleEffect*TmpResource
/// 保存分组 自定义分组信息，（预留）
/// ExlData->导表工具Ex ->打开配置界面 修改路径信息
/// SVNTools配置 参数 1是否自动刷新  参数2 筛选 参数3 是否反转 (0否1是)
/// 命令行工具类 EditorCommandTools.ProcessCommand2("svn", " status Assets", false);
/// </summary>
public class EditorSVNTools : EditorWindow
{
    public List<SVNFileInfo> AllFiles = new List<SVNFileInfo>();
    /// <summary>
    /// 筛选列表
    /// </summary>
    public List<SVNFileInfo> FileInfosFilters = new List<SVNFileInfo>();

    private Vector2 _viewSize = Vector2.zero;

    private string _strCommitInfo = "更新";
    private bool _isAutoSelectScript = true;
    private bool _isAutoRefresh = true;
    private string _strFilterInfo = string.Empty;
    private string _strFilterInfoInclude = string.Empty;
    private string[] _filterInfos;
    private string[] _filterInfosInclude;
    private EnumSVNFileState _filterType = EnumSVNFileState.All;

    /// <summary>
    /// 输出信息
    /// </summary>
    private string _logInfo = "日志信息";
    private bool _isForce;

    void OnEnable()
    {
        if (_isAutoRefresh && _isForce)
        {
            RefreshFileInfos();
        }
    }
    /// <summary>
    /// 聚焦
    /// </summary>
    void OnFocus()
    {
        _isForce = true;
    }
    void OnLostFocus()
    {
        _isForce = false;
    }
    void OnGUI()
    {

        if (DrawDragContent())
        {
            return;
        }

        DrawTitle();

        DrawContent();

        DrawBottom();



    }



    #region  拖动区域功能逻辑 

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
            ResetLastSelect();
        }
        //第一次拖动
        if (isDrag != _isDraging)
        {
            _isDraging = isDrag;
            //改变鼠标的外表  
            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
            {
                return DragObjectInfos(DragAndDrop.objectReferences, _isAutoSelectScript);
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
                DragObjectInfo(obj, _isAutoSelectScript);
            }
            SortFileInfosFilters();
            SaveMapInfo();
            _lastFileName = DragAndDrop.paths[0];
            return true;
        }
        return false;
    }

    private bool DragObjectInfos(Object[] objs, bool isSelectScript)
    {
        if (_lastFileObj != objs[0])
        {
            string strLog = "\n";
            string strSucceed = "\n";
            bool res = false;
            for (int cnt = 0; cnt < objs.Length; cnt++)
            {
                if (DragObjectInfo(objs[cnt], isSelectScript))
                {
                    res = true;
                    strSucceed = strSucceed + objs[cnt].name + "\n";
                }
                else
                {
                    strLog = strLog + objs[cnt].name + "\n";
                }
            }
            if (res)
            {
                SetLogInfo(" Change File Info  " + strSucceed);
                RefreshFilterList();
                SaveMapInfo();
            }
            else
            {
                SetLogInfo("Not Change Files " + strLog);
            }
            _lastFileObj = DragAndDrop.objectReferences[0];
            return true;
        }
        return false;
    }
    private bool DragObjectInfo(Object obj, bool isSelectScript)
    {
        bool res = false;
#if UNITY_2018_1_OR_NEWER
        Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(obj);
#elif UNITY_5_6
        Object parentObject = PrefabUtility.GetPrefabParent(obj);
#endif
        string path;
        if (parentObject != null)
        {
            path = AssetDatabase.GetAssetPath(parentObject);
        }
        else
        {
            path = AssetDatabase.GetAssetPath(obj);
        }
        SVNFileInfo info = GetFileDataByName(path);
        if (info != null)
        {
            info.SetIsSelect(true);
            res = true;
            if (Directory.Exists(info.Name))//选中的是文件夹
            {
                for (int cnt = 0; cnt < AllFiles.Count; cnt++)
                {
                    if (AllFiles[cnt] == info)
                    {
                        continue;
                    }
                    if (AllFiles[cnt].Name.Contains(info.Name))
                    {
                        AllFiles[cnt].SetIsSelect(true);
                    }
                }
            }
        }
        if (isSelectScript)
        {
            List<SVNFileInfo> references = DragObjectReferenceInfo(obj);
            if (references != null && references.Count > 0)
            {
                for (int cnt = 0; cnt < references.Count; cnt++)
                {
                    references[cnt].SetIsSelect(true);
                    res = true;
                }
            }
        }
        return res;
    }
    private List<SVNFileInfo> DragObjectReferenceInfo(Object obj)
    {
        GameObject trans = obj as GameObject;
        if (trans == null)
        {
            return null;
        }
        Component[] coms = trans.GetComponents<Component>();
        if (coms == null)
        {
            return null;
        }
        List<SVNFileInfo> list = new List<SVNFileInfo>();
        for (int cnt = 0; cnt < coms.Length; cnt++)
        {
            string strName = coms[cnt].GetType().Name + ".cs";
            SVNFileInfo svnInfo = GetFileDataByContainName(strName);
            if (svnInfo != null)
            {
                list.Add(svnInfo);
            }
        }
        return list;
    }


    private void ResetLastSelect()
    {
        _lastFileName = string.Empty;
        _lastFileObj = null;
    }

    #endregion

    private void DrawTitle()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("包含列表", GUILayout.Width(65)))
        {
            EditorSelectFolder.OpenEditorSelectFolder(_filterInfosInclude, EventRefreshIncludePath);
        }
        if (GUILayout.Button("移除列表", GUILayout.Width(65)))
        {
            EditorSelectFolder.OpenEditorSelectFolder(_filterInfos, EventRefreshSelectPath);
        }

        //反向选择
        GUILayout.Label("筛选类型:", GUILayout.Width(55));
        EnumSVNFileState type = (EnumSVNFileState)EditorGUILayout.EnumPopup(_filterType);
        if (type != _filterType)
        {
            _filterType = type;
            RefreshFilterList();
        }

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("T", GUI.skin.label, GUILayout.Width(15)))
        {
            SortListByValue();
        }
        GUILayout.Label(FileInfosFilters.Count + "/" + AllFiles.Count);
        GUILayout.Label("預設自動選中脚本 ：", GUILayout.Width(80));
        _isAutoSelectScript = EditorGUILayout.Toggle(_isAutoSelectScript);
        GUILayout.Label("自動刷新 ：", GUILayout.Width(50));
        bool isAutoRefresh = EditorGUILayout.Toggle(_isAutoRefresh);
        if (isAutoRefresh != _isAutoRefresh)
        {
            string saveValue = isAutoRefresh ? "1" : "0";
            _isAutoRefresh = isAutoRefresh;
        }
        if (GUILayout.Button("SAVE", GUILayout.Width(38)))
        {
            SaveConfig();
        }
        if (GUILayout.Button("Sort", GUILayout.Width(38)))
        {
            SortFileInfosFilters();

        }
        if (GUILayout.Button("CLS", GUILayout.Width(38)))
        {
            for (int cnt = 0; cnt < AllFiles.Count; cnt++)
            {
                AllFiles[cnt].SetIsSelect(false);
            }
            RefreshFilterList();
        }

        EditorGUILayout.EndHorizontal();


    }
    private void EventRefreshIncludePath(List<string> obj)
    {
        ConvertToSavePath(obj, ref _strFilterInfoInclude, ref _filterInfosInclude);
        SaveConfig();
        RefreshFilterList();
        this.Repaint();
    }

    private void EventRefreshSelectPath(List<string> obj)
    {
        ConvertToSavePath(obj, ref _strFilterInfo, ref _filterInfos);
        SaveConfig();
        RefreshFilterList();
        this.Repaint();
    }
    private void ConvertToSavePath(List<string> obj, ref string filter, ref string[] filterInfos)
    {
        filterInfos = obj.ToArray();
        filter = string.Empty;
        if (filterInfos.Length > 0)
        {
            filter = filterInfos[0];
            for (int cnt = 1; cnt < filterInfos.Length; cnt++)
            {
                filter = filter + "*" + filterInfos[cnt];
            }
        }
    }

    private void DrawContent()
    {
        _isLeftShift = Event.current.shift;
        _viewSize = EditorGUILayout.BeginScrollView(_viewSize, GUILayout.Height(350));
        for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
        {
            GUILayout.BeginHorizontal();

            SVNFileInfo fileData = FileInfosFilters[cnt];
            string strType = fileData.Flag;

            if (fileData.State == EnumSVNFileState.Add)
            {
                GUI.color = Color.yellow;
            }
            else
            {
                GUI.color = Color.cyan;
            }
            if (GUILayout.Button(strType, GUI.skin.label, GUILayout.Width(16)))
            {
                CheckClickShift(cnt);
            }
            GUI.color = fileData.IsSelect ? Color.gray : Color.white;
            SetDataSelect(fileData, EditorGUILayout.Toggle(fileData.IsSelect, GUILayout.Width(20)));
            if (GUILayout.Button(fileData.ToString(), GUI.skin.label, GUILayout.Width(500)))
            {
                CheckClickName(cnt);
                //SetDataSelect(fileData, !fileData.IsSelect);
            }
            if (GUILayout.Button("对比", GUILayout.Width(38)))
            {
                string commond = " /command:diff  /path:" + fileData.Name;
                EditorCommandTools.ProcessCommand("TortoiseProc.exe", commond);
                //SetDataSelect(fileData, !fileData.IsSelect);
            }
            if (GUILayout.Button("日志", GUILayout.Width(38)))
            {
                string commond = " /command:log   /path:" + fileData.Name;
                EditorCommandTools.ProcessCommand("TortoiseProc.exe", commond);
                //SetDataSelect(fileData, !fileData.IsSelect);
            }
            if (GUILayout.Button("还原", GUILayout.Width(38)))
            {
                string commond = " /command:revert  /path:" + fileData.Name;
                EditorCommandTools.ProcessCommand("TortoiseProc.exe", commond);
                if (!_isAutoRefresh)
                {
                    RefreshFileInfos();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.color = Color.white;
        EditorGUILayout.EndScrollView();
    }
    private bool _isLeftShift = false;
    private int _shiftIdx = 0;
    private void CheckClickShift(int idx)
    {
        if (!_isLeftShift)
        {
            _shiftIdx = idx;
            return;
        }
        if (_shiftIdx == idx)
        {
            return;
        }
        List<SVNFileInfo> list = null;
        if (_shiftIdx > idx)
        {
            list = this.FileInfosFilters.FindAll(item => item.Index <= _shiftIdx && item.Index >= idx);
        }
        else
        {
            list = this.FileInfosFilters.FindAll(item => item.Index >= _shiftIdx && item.Index <= idx);
        }
        for (int cnt = 0; cnt < list.Count; cnt++)
        {
            SetDataSelect(list[cnt], !list[cnt].IsSelect, false);
        }

    }


    private void CheckClickName(int cnt)
    {
        SVNFileInfo fileInfo = FileInfosFilters[cnt];
        Debug.Log(" Click Name : " + fileInfo.ToString());
        if (fileInfo.Object == null)
        {
            fileInfo.Object = AssetDatabase.LoadAssetAtPath(fileInfo.Name, typeof(Object));
        }
        if (fileInfo.Object == null)
        {
            Debug.LogError(" Is Null Object " + fileInfo);
        }
        SetDataSelect(fileInfo, !fileInfo.IsSelect);
        Selection.activeObject = null;
        Selection.activeObject = fileInfo.Object;

    }
    private string _saveGroup = "1";
    private void DrawBottom()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("提交记录信息 ：", GUILayout.Width(80));
        _strCommitInfo = EditorGUILayout.TextField(_strCommitInfo, GUILayout.Width(200));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("CLS_LOG ", GUILayout.Width(80)))
        {
            _logInfo = string.Empty;
        }
        GUILayout.Label("保存分组:", GUILayout.Width(50));
        string group = EditorGUILayout.TextField(_saveGroup, GUILayout.Width(50));
        if (_saveGroup != group)
        {
            _saveGroup = group;
            RefreshSaveSelect();
            SortFileInfosFilters();
        }
        if (GUILayout.Button("保存临时记录"))
        {
            SaveMapInfo();
        }
        //if (GUILayout.Button("清空记录"))
        //{
        //    ClearMapInfo();
        //    LoadMapInfo();
        //}

        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SVN 添加"))
        {
            //FileInfoSVNToMiss();
            FileInfoSVNToAdd();
        }
        if (GUILayout.Button("SVN 提交"))
        {
            FileInfoCommitToSVN();
        }
        if (GUILayout.Button("SVN 刷新"))
        {
            RefreshFileInfos();
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        GUI.color = Color.red;
        GUILayout.Label(_logInfo);

        GUILayout.EndHorizontal();

    }
    private void InitialConfit()
    {
        string strConfit = EditorConfitTools.GetSavePath(EnumLoadSettingPath.SVNConfigTools);
        string[] strs = Utility.Xml.ParseString<string>(strConfit, Utility.Xml.SplitComma);
        if (strs == null)
        {
            return;
        }
        if (strs.Length > 0)
        {
            _isAutoRefresh = strs[0] == "1";//自动刷新
        }
        if (strs.Length > 1)
        {
            _strFilterInfo = strs[1];//文件筛选信息
            _filterInfos = Utility.Xml.ParseString<string>(_strFilterInfo, Utility.Xml.SplitAsterisk);
        }
        if (strs.Length > 2)
        {

        }
        if (strs.Length > 3)
        {
            _strFilterInfoInclude = strs[3];
            _filterInfosInclude = Utility.Xml.ParseString<string>(_strFilterInfoInclude, Utility.Xml.SplitAsterisk);
        }

    }
    private void SaveConfig()
    {
        string strContent = _isAutoRefresh ? "1" : "0";
        strContent = strContent + "," + _strFilterInfo;
        strContent = strContent + "," + (true ? "1" : "0");
        strContent = strContent + "," + _strFilterInfoInclude;

        EditorConfitTools.SaveConfigByType(EnumLoadSettingPath.SVNConfigTools, strContent);
        EditorConfitTools.SaveConfig();

    }
    /// <summary>
    /// 刷新 全部信息 
    /// </summary>
    private void RefreshFileInfos()
    {
        //初始化配置
        InitialConfit();
        //读取所有变更文件信息
        InitialAllFiles();
        //加载保存路径信息
        LoadMapInfo();
        //刷新筛选列表
        RefreshFilterList();

    }
    private void SetLogInfo(string strLog)
    {
        _logInfo = strLog;
    }

    /// <summary>
    /// 执行命令 检测变更文件
    /// </summary>
    private void InitialAllFiles()
    {

        //TortoiseProc.exe /command:repostatus /path:"D:\Project\WorkSpace\Client\Project\BattleWarships20160330\Assets"

        //string str = EditorCommandTools.ProcessCommand("cmd.exe", "/k svn status Assets", false);
        string str = EditorCommandTools.ProcessCommand("svn", " status Assets", false);

        FileInfosFilters.Clear();

        ReleaseFileInfo(AllFiles);
        //分隔svn 状态结果
        string[] pathFileInfos = Utility.Xml.ParseString<string>(str, new char[] { '\n' });
        if (pathFileInfos == null)
        {
            Debug.Log("  No Files  ");
            return;
        }
        for (int cnt = 0; cnt < pathFileInfos.Length; cnt++)
        {
            //"?""       "
            string strInfo = pathFileInfos[cnt];
            if (string.IsNullOrEmpty(strInfo))
            {
                continue;
            }
            string[] paths = strInfo.Split(new string[] { "       " }, System.StringSplitOptions.RemoveEmptyEntries);
            if (paths == null || paths.Length != 2)
            {
                continue;
            }
            //[0] 标识  [1] 文件具体路径
            string strName = paths[1].Trim().Replace("\\", "/");
            if (string.IsNullOrEmpty(strName))
            {
                Debug.LogError("  File Info Error " + paths[1]);
                continue;
            }
            AddSVNFileInfo(strName, paths[0]);
            if (paths[0] == "?")//未识别文件
            {
                List<string> files = new List<string>();
                GetDirectorFiles(strName, files);
                for (int i = 0; i < files.Count; i++)
                {
                    AddSVNFileInfo(files[i], paths[0]);
                }
            }
        }
    }
    /// <summary>
    /// 添加文件到列表中
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="flag">标识? A M</param>
    private void AddSVNFileInfo(string path, string flag)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(" Is null " + flag);
            return;
        }
        SVNFileInfo tempInfo = CreateFileInfos();
        tempInfo.SetName(path, flag);
        AllFiles.Add(tempInfo);
    }

    /// <summary>
    /// 根据目录路径 获取所有文件路径
    /// </summary>
    private void GetDirectorFiles(string path, List<string> listPaths)
    {
        if (Directory.Exists(path))
        {
            string[] diArr = Directory.GetDirectories(path);
            if (diArr.Length > 0)
            {
                for (int cnt = 0; cnt < diArr.Length; cnt++)
                {
                    string strNewPath = diArr[cnt].Replace("\\", "/");
                    listPaths.Add(strNewPath);
                    GetDirectorFiles(strNewPath, listPaths);
                }
            }
            string[] files = Directory.GetFiles(path);
            if (files.Length > 0)
            {
                for (int cnt = 0; cnt < files.Length; cnt++)
                {
                    string strNewPath = files[cnt].Replace("\\", "/");
                    listPaths.Add(strNewPath);
                }
            }
        }
        //else
        //{
        //    listPaths.Add(path);
        //}
    }


    private SVNFileInfo GetFileDataByName(string name)
    {
        return AllFiles.Find(item => item.Name == name);
    }
    private SVNFileInfo GetFileDataByContainName(string name)
    {
        return AllFiles.Find(item => item.Name.Contains(name));
    }


    private void RefreshFilterList(int sortType = 1)
    {
        FileInfosFilters.Clear();
        for (int cnt = 0; cnt < AllFiles.Count; cnt++)
        {
            if (IsValidFilter(AllFiles[cnt]))
            {
                FileInfosFilters.Add(AllFiles[cnt]);
            }
        }
        SortFileInfosFilters(sortType);

    }
    private void SortFileInfosFilters(int sortType = 1)
    {
        if (sortType == 1)
        {
            FileInfosFilters.Sort(SortData);
            for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
            {
                FileInfosFilters[cnt].Index = cnt;
            }
        }
    }
    private void ResetFilterIndex()
    {
        for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
        {
            FileInfosFilters[cnt].Index = cnt;
        }
    }


    private int _tempClickIndex = 0;
    private void SortListByValue()
    {
        EnumSVNFileState state = (EnumSVNFileState)_tempClickIndex;
        for (int cnt = 0; cnt < AllFiles.Count; cnt++)
        {
            if (AllFiles[cnt].State == state)
            {
                AllFiles[cnt].SetSortValue(AllFiles[cnt].SortValue + 10);
            }
        }
        _tempClickIndex++;
        if (_tempClickIndex > 3)
        {
            ResetSortListValue();
        }
        AllFiles.Sort(SortDataValue);
        RefreshFilterList(0);
        ResetFilterIndex();
        SaveMapInfo();
    }
    /// <summary>
    /// 重置sort value
    /// </summary>
    private void ResetSortListValue()
    {
        for (int cnt = 0; cnt < AllFiles.Count; cnt++)
        {
            AllFiles[cnt].ResetSortValue();
        }
        _tempClickIndex = 0;
    }

    private void SetDataSelect(SVNFileInfo data, bool isSelect, bool isAutoSelectMeta = true)
    {
        if (isSelect != data.IsSelect)
        {
            data.SetIsSelect(isSelect);
            if (!isAutoSelectMeta)
            {
                return;
            }
            string metaName = data.Name + ".meta";
            SVNFileInfo metaInfo = AllFiles.Find(item => item.Name == metaName);
            if (metaInfo != null)
            {
                metaInfo.SetIsSelect(isSelect);
            }
        }

    }
    private int SortData(SVNFileInfo x, SVNFileInfo y)
    {
        if (x.IsSelect != y.IsSelect)
        {
            return y.IsSelect.CompareTo(x.IsSelect);
        }
        bool xCs = x.Name.Contains(".cs");
        bool yCs = y.Name.Contains(".cs");
        if (xCs && !yCs)
        {
            return -1;
        }
        if (!xCs && yCs)
        {
            return 1;
        }

        return x.Name.CompareTo(y.Name);
    }
    private int SortDataValue(SVNFileInfo x, SVNFileInfo y)
    {
        if (x.IsSelect != y.IsSelect)
        {
            return y.IsSelect.CompareTo(x.IsSelect);
        }
        if (y.SortValue != x.SortValue)
        {
            return x.SortValue.CompareTo(y.SortValue);
        }
        return SortData(x, y);
    }
    /// <summary>
    /// 筛选
    /// </summary>
    private bool IsValidFilter(SVNFileInfo data)
    {

        bool isSelect = !IsContainName(_filterInfos, data.Name);
        if (!isSelect)
        {
            isSelect = IsContainName(_filterInfosInclude, data.Name);
        }
        return isSelect;
    }

    private bool IsContainName(string[] list, string strName)
    {
        if (list == null)
        {
            return false;
        }
        for (int cnt = 0; cnt < list.Length; cnt++)
        {
            if (strName.Contains(list[cnt]))
            {
                return true;
            }
        }
        return false;
    }

    private bool FileInfoSVNToAdd()
    {
        StringBuilder sbAdd = new StringBuilder();
        //不带控制台
        sbAdd.Append("/c svn add --parents ");
        bool hasAdd = false;
        for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
        {
            if (FileInfosFilters[cnt].IsSelect)
            {
                if (FileInfosFilters[cnt].State == EnumSVNFileState.None)
                {
                    sbAdd.Append(FileInfosFilters[cnt].Name);
                    sbAdd.Append(" ");
                    hasAdd = true;
                }
            }
        }
        if (hasAdd)
        {
            Debug.Log(sbAdd.ToString());
            EditorCommandTools.ProcessCommand("cmd.exe", sbAdd.ToString());
        }
        return hasAdd;
    }

    private bool FileInfoSVNToMiss()
    {
        StringBuilder sbAdd = new StringBuilder();
        //不带控制台
        sbAdd.Append("/c svn delete  ");
        bool hasMiss = false;
        for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
        {
            if (FileInfosFilters[cnt].IsSelect)
            {
                if (FileInfosFilters[cnt].State == EnumSVNFileState.Miss)
                {
                    sbAdd.Append(FileInfosFilters[cnt].Name);
                    sbAdd.Append(" ");
                    hasMiss = true;
                }
            }
        }
        if (hasMiss)
        {
            Debug.Log(sbAdd.ToString());
            EditorCommandTools.ProcessCommand("cmd.exe", sbAdd.ToString());
        }
        return hasMiss;
    }

    private void FileInfoCommitToSVN()
    {
        if (_strCommitInfo.Length < 5)
        {
            Debug.LogError(" 提交记录信息太短 ");
            return;
        }
        bool hasAdd = FileInfoSVNToAdd();
        bool hasMiss = FileInfoSVNToMiss();

        StringBuilder sbMod = new StringBuilder();
        //带控制台
        sbMod.Append("/K svn ci -m ");
        sbMod.Append("\"");
        sbMod.Append(_strCommitInfo);
        sbMod.Append("\" ");
        bool hasContent = false;
        for (int cnt = 0; cnt < FileInfosFilters.Count; cnt++)
        {
            if (FileInfosFilters[cnt].IsSelect)
            {
                sbMod.Append(FileInfosFilters[cnt].Name);
                sbMod.Append(" ");
                hasContent = true;
            }
        }
        if (hasContent || hasAdd || hasMiss)
        {
            EditorCommandTools.ProcessCommand("cmd.exe", sbMod.ToString());
            Debug.Log(sbMod.ToString());
            RefreshFileInfos();
        }
        else
        {
            SetLogInfo(" 未选择文件。");
        }
    }


    private List<SVNFileInfo> _cacheFiles = new List<SVNFileInfo>();
    private SVNFileInfo CreateFileInfos()
    {
        SVNFileInfo data;
        if (_cacheFiles.Count > 0)
        {
            data = _cacheFiles[0];
            data.Object = null;
            _cacheFiles.RemoveAt(0);
        }
        return new SVNFileInfo();
    }
    private void ReleaseFileInfo(List<SVNFileInfo> list)
    {
        _cacheFiles.AddRange(list);
        list.Clear();
    }


    private Dictionary<string, List<string>> _mapSaveHash = new Dictionary<string, List<string>>();
    private void LoadMapInfo()
    {
        _mapSaveHash.Clear();
        Dictionary<string, string> map = new Dictionary<string, string>();
        LoadLocalMapHelper.InitialMap(GetSavePath(), ref map);
        foreach (var item in map)
        {
            string[] strs = Utility.Xml.ParseString<string>(item.Value, Utility.Xml.SplitComma);
            _mapSaveHash.Add(item.Key, new List<string>(strs));
        }
        RefreshSaveSelect();
    }
    private void RefreshSaveSelect()
    {
        for (int cnt = 0; cnt < AllFiles.Count; cnt++)
        {
            if (_mapSaveHash.ContainsKey(AllFiles[cnt].Name))
            {
                if (_mapSaveHash[AllFiles[cnt].Name].Contains(_saveGroup))
                {
                    AllFiles[cnt].SetIsSelect(true);
                    continue;
                }
            }
            AllFiles[cnt].SetIsSelect(false);
        }

    }
    private void SaveMapInfo()
    {
        if (AllFiles.Count == 0)
        {
            return;
        }
        for (int cnt = 0; cnt < AllFiles.Count; cnt++)
        {
            if (AllFiles[cnt].IsSelect)
            {
                if (!_mapSaveHash.ContainsKey(AllFiles[cnt].Name))
                {
                    _mapSaveHash.Add(AllFiles[cnt].Name, new List<string>());
                    _mapSaveHash[AllFiles[cnt].Name].Add(_saveGroup);
                }
                else
                {
                    if (!_mapSaveHash[AllFiles[cnt].Name].Contains(_saveGroup))
                    {
                        _mapSaveHash[AllFiles[cnt].Name].Add(_saveGroup);
                    }
                }
            }
            else
            {
                if (_mapSaveHash.ContainsKey(AllFiles[cnt].Name))
                {
                    if (_mapSaveHash[AllFiles[cnt].Name].Contains(_saveGroup))
                    {
                        _mapSaveHash[AllFiles[cnt].Name].Remove(_saveGroup);
                    }
                }
            }

        }
        Dictionary<string, string> map = new Dictionary<string, string>();
        foreach (var item in _mapSaveHash)
        {
            if (item.Value.Count > 0)
            {
                string str = item.Value[0];
                for (int cnt = 1; cnt < item.Value.Count; cnt++)
                {
                    str = str + "," + item.Value[cnt];
                }
                map.Add(item.Key, str);
            }
        }
        LoadLocalMapHelper.SaveFile(GetSavePath(), map);
    }


    public static string GetSavePath()
    {
        string strPath = EditorConfitTools.GetSavePath(EnumLoadSettingPath.SVNTempSave);
        if (string.IsNullOrEmpty(strPath))
        {
            // return "";
        }
        return strPath + "/SVNData.txt";
    }


    /// <summary>
    /// 配置设置界面
    /// </summary>
    [MenuItem("MGTools/SVNTools")]
    public static void OpenSVNTools()
    {
        EditorSVNTools myWindow =
           (EditorSVNTools)EditorWindow.GetWindow(typeof(EditorSVNTools), false, "EditorSVNTools", true);//创建窗口

        myWindow.Show();
    }
}
public enum EnumSVNFileState
{
    None = 0,
    /// <summary>
    /// 待添加 未提交状态
    /// </summary>
    Add,
    /// <summary>
    /// 发生变更文件
    /// </summary>
    Mod,
    /// <summary>
    /// 待删除 未提交状态
    /// </summary>
    Del,
    /// <summary>
    /// 该项目已遗失 (被非 svn 命令所删除) 或是不完整
    /// </summary>
    Miss,
    Select,
    All,
}


