using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;
using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private MapGenerate _mapGenerate = new MapGenerate();
    private DelaunayTriangulation2 _delaunayTriangulation2;

    protected override void OnInit()
    {
        _mapGenerate.RandomSpeed = 2;
        _mapGenerate.GenerateMapInfo();

        _delaunayTriangulation2 = MapHelper.CreateDelaunayTriangulation(_mapGenerate.ListRooms);


        DrawGizmosDelaunay.Instance.TriangulateThePoint(_delaunayTriangulation2);

    }

    protected override void OnRelease()
    {
        //Log.Info(" MapManager OnRelease  ");
    }



    //private List<GameObject> _listObjs = new List<GameObject>();
    //protected override void OnInit()
    //{
    //    SceneVariants.RandomMap(Random.Range(10, 15), Random.Range(10, 15));
    //    CreateMapGameObjects();
    //}

    //private void CreateMapGameObjects()
    //{
    //    for (var i = 0; i < SceneVariants.map.MapWidth(); i++)
    //    {
    //        for (var j = 0; j < SceneVariants.map.MapHeight(); j++)
    //        {
    //            Vector3 pos = new Vector3(i, 0, j);
    //            string strPath = SceneVariants.map.grid[i, j].prefabPath;
    //            GameObject obj = EntityManager.Instance.CreateFromPrefab(strPath, pos, EnumTransParent.Map);
    //            if (obj == null)
    //            {
    //                continue;
    //            }
    //            _listObjs.Add(obj);
    //        }
    //    }
    //}

    //private void ClearsMapItems()
    //{
    //    for (var i = 0; i < _listObjs.Count; i++)
    //    {
    //        GameObject.Destroy(_listObjs[i]);
    //    }
    //    _listObjs.Clear();

    //}


    //protected override void OnRelease()
    //{
    //    ClearsMapItems();
    //    base.OnRelease();
    //}











}

