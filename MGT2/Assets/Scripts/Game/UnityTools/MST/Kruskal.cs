using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kruskal
{
    private Dictionary<string, int> _mapStrNames = new Dictionary<string, int>();
    private Dictionary<int, string> _mapStrNamesKey = new Dictionary<int, string>();
    private List<Edge> _listEdges = new List<Edge>();
    public void AddEdge(string strStart, string strEnd, int weith)
    {
       
        if (!_mapStrNames.ContainsKey(strStart))
        {
            int key = _mapStrNames.Count;
            _mapStrNames.Add(strStart, key);
            _mapStrNamesKey.Add(key, strStart);
        }
        if (!_mapStrNames.ContainsKey(strEnd))
        {
            int key = _mapStrNames.Count;
            _mapStrNames.Add(strEnd, key);
            _mapStrNamesKey.Add(key, strEnd);
        }
        _listEdges.Add(new Edge(_mapStrNames[strStart], _mapStrNames[strEnd], weith));
    }

    public List<string[]> GetResult()
    {
        List<Edge> edges = Execute();
        List<string[]> res = new List<string[]>();
        for (int i = 0; i < edges.Count; i++)
        {
            string[] strs = new string[2];

            strs[0] = _mapStrNamesKey[edges[i].x];
            strs[1] = _mapStrNamesKey[edges[i].y];
            res.Add(strs);
        }
        return res;
    }

    public List<Edge> Execute()
    {
        List<Edge> list = _listEdges;

        list.Sort((x, y) => x.w.CompareTo(y.w));
        List<Edge> target = new List<Edge>();
        DisjointSet dsu = new DisjointSet(_listEdges.Count);
        for (int i = 0; i < list.Count; i++)
        {
            Edge e = list[i];
            int x = e.x;
            int y = e.y;
            if (dsu.Find(x) != dsu.Find(y))
            {
                dsu.Union(x, y);
                target.Add(e);
            }
        }
        //for (int i = 0; i < target.Count; i++)
        //{
        //    Debug.LogError(target[i].ToString());
        //}
        return target;
    }

}
