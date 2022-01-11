using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Primitives;
using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHelper
{


    public static List<MapRoom> CreateRandomRooms()
    {
        List<MapRoom> list = new List<MapRoom>();


        return list;

    }


    public static DelaunayTriangulation2 CreateDelaunayTriangulation(List<MapRoom> listRooms)
    {
        DelaunayTriangulation2 delaunay = new DelaunayTriangulation2();
        List<Vertex2> listHall = new List<Vertex2>();
        for (int cnt = 0; cnt < listRooms.Count; cnt++)
        {
            float x = listRooms[cnt].RoomRect.x + listRooms[cnt].RoomRect.width / 2;
            float y = listRooms[cnt].RoomRect.y + listRooms[cnt].RoomRect.height / 2;
            listHall.Add(new Vertex2(x, y, cnt));
        }
        delaunay.Generate(listHall, false);
        return delaunay;
    }

    public static List<MapPointLine> ConvertListPoints(DelaunayTriangulation2 delaunay)
    {
        List<MapPointLine> listPointMap = new List<MapPointLine>();

        float x1, x2, x3, z1, z2, z3;//三角形的三个顶点坐标
        Vector3 p1, p2, p3;//三条边
        Gizmos.color = Color.white;
        int numberOfTriangle = delaunay.Cells.Count;//三角形数量
        for (int i = 0; i < numberOfTriangle; i++)
        {

            x1 = delaunay.Cells[i].Simplex.Vertices[0].X;
            z1 = delaunay.Cells[i].Simplex.Vertices[0].Y;

            x2 = delaunay.Cells[i].Simplex.Vertices[1].X;
            z2 = delaunay.Cells[i].Simplex.Vertices[1].Y;

            x3 = delaunay.Cells[i].Simplex.Vertices[2].X;
            z3 = delaunay.Cells[i].Simplex.Vertices[2].Y;


            p1 = new Vector3(x1, 0, z1);
            p2 = new Vector3(x2, 0, z2);
            p3 = new Vector3(x3, 0, z3);

            AddPointToMap(listPointMap, p1, p2);
            AddPointToMap(listPointMap, p2, p3);
            AddPointToMap(listPointMap, p1, p3);



        }


        return listPointMap;

    }



    private static void AddPointToMap(List<MapPointLine> list, Vector3 p1, Vector3 p2)
    {
        MapPointLine data = list.Find(item => item.EqualPoint(p1, p2));
        if (data == null)
        {
            list.Add(new MapPointLine(p1, p2)); ;
        }

    }



    public static string Vector3ToString(Vector3 pos)
    {
        return string.Format("{0},{1},{2}", (int)pos.x, (int)pos.y, (int)pos.z);

    }
}
