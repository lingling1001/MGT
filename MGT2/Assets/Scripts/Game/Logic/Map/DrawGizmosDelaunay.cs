using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;
using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmosDelaunay : MonoSingleton<DrawGizmosDelaunay>
{
    public bool ShouldDrawNow;

    private DelaunayTriangulation2 _delaunayMesh;
    private List<MapPointLine> _listPoints = new List<MapPointLine>();
    private List<MapPointLine> _listPointsAg = new List<MapPointLine>();

    public void TriangulateThePoint(DelaunayTriangulation2 delaunay)
    {
        _delaunayMesh = delaunay;
        ShouldDrawNow = true;
        _listPointsAg.Clear();
        _listPoints = MapHelper.ConvertListPoints(_delaunayMesh);

        Kruskal kruskal = new Kruskal();

        for (int i = 0; i < _listPoints.Count; i++)
        {
            MapPointLine point = _listPoints[i];

            kruskal.AddEdge(point.GetNameStart(), point.GetNameEnd(), point.Distance);
        }
        List<string[]> listRes = kruskal.GetResult();

        for (int i = 0; i < listRes.Count; i++)
        {
            MapPointLine res = _listPoints.Find(item => item.EqualPoint(listRes[i][0], listRes[i][1]));

            _listPointsAg.Add(res);

        }



    }






    void OnDrawGizmos()
    {
        if (!ShouldDrawNow || _delaunayMesh == null)
        {
            return;
        }
        for (int cnt = 0; cnt < _listPointsAg.Count; cnt++)
        {
            //Gizmos.DrawLine(_listPoints[cnt].PointX + Vector3.one * cnt, _listPoints[cnt].PointZ + Vector3.one * cnt);
            Gizmos.DrawLine(_listPointsAg[cnt].PointStart, _listPointsAg[cnt].PointEnd);
        }

        //for (int cnt = 0; cnt < _listPointsAg.Count; cnt++)
        //{
        //    //Gizmos.DrawLine(_listPoints[cnt].PointX + Vector3.one * cnt, _listPoints[cnt].PointZ + Vector3.one * cnt);
        //    Gizmos.DrawLine(_listPointsAg[cnt].PointStart, _listPointsAg[cnt].PointEnd);
        //}
    }

}

