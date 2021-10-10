using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequareGridChunk : MonoBehaviour
{
    SequareCell[] cells;

    public SequareMesh terrain, rivers;
    Canvas gridCanvas;
    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        terrain = GetComponentInChildren<SequareMesh>();
        cells = new SequareCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }
    public void AddCell(int index, SequareCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }
    public void Refresh()
    {
        Triangulate();
        enabled = true;
    }
    void LateUpdate()
    {
        if (enabled)
        {
            Triangulate();
            enabled = false;
        }
    }

    public void Triangulate()
    {
        terrain.Clear();
        rivers.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        rivers.Apply();
        terrain.Apply();

    }
    public void Triangulate(SequareCell cell)
    {
        if (cell == null)
        {
            return;
        }
        TriangulateBase(cell);

        TriangulateConnection(cell, (int)EnumDirection.East);
        TriangulateConnection(cell, (int)EnumDirection.North);

        TriangulateGap(cell);
    }

    /// <summary>
    /// 绘制基础格子
    /// </summary>
    private void TriangulateBase(SequareCell cell)
    {
        if (cell.HasRiver)
        {
            TriangulateBaseRiver(cell);
        }
        else
        {
            TriangulateBaseNormal(cell);
        }

    }
    private void TriangulateBaseRiver(SequareCell cell)
    {
        float size = HexMetrics.solidFactor * HexMetrics.riverWidthRate;
        float size2 = size * HexMetrics.riverWidthRate;

        Vector3 center = cell.Position;
        for (int cnt = 0; cnt < HexMetrics.directionNum; cnt++)
        {
            SequareCell neighbor = cell.GetNeighbor(cnt);
            if (neighbor == null)
            {
                continue;
            }
            int nextDir = HexMetrics.Next(cnt);
            int preDir = HexMetrics.Previous(cnt);


            Vector3 v1 = cell.GetCornerSolid(nextDir, size);
            Vector3 bridge = HexMetrics.GetBridge(nextDir);
            Vector3 v2 = v1 + bridge;

            Vector3 v4 = cell.GetCornerSolid(nextDir);
            Vector3 bridge2 = HexMetrics.GetBridge(preDir);
            Vector3 v3 = v4 + bridge2;

            terrain.AddQuad(v1, v2, v3, v4);
            terrain.AddQuadColor(cell.Color);

            Vector3 riverV1 = cell.GetCornerSolid(cnt, size);
            Vector3 riverV3 = cell.GetCornerSolid(cnt) + bridge;

            EdgeVerticesNum e1 = new EdgeVerticesNum(riverV1, v1, 2);
            EdgeVerticesNum e2 = new EdgeVerticesNum(riverV3, v3, 2);

            Vector3 centerV1 = cell.GetCornerSolid(cnt, size);
            Vector3 centerV2 = cell.GetCornerSolid(cnt + 1, size);
            center.y = cell.StreamBedY;
            EdgeVerticesNum eCenter = new EdgeVerticesNum(centerV1, centerV2, 2);

            if (neighbor.HasRiver)
            {
                eCenter.vs[1].y = e1.vs[1].y = e2.vs[1].y = cell.StreamBedY;

            }

            TriangulateEdgeStrip(e1, cell.Color, e2, cell.Color);
            TriangulateEdgeFan(center, eCenter, cell.Color);


            //TriangulateEdgeStrip(e1, cell.Color, e2, cell.Color);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v1 ", v1);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v2 ", v2);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v3 ", v3);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v4 ", v4);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   riverV1 ", riverV1);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   riverV3 ", riverV3);
            /**

            //Vector3 bridge = HexMetrics.GetBridge(cnt);
            //int nextDir = HexMetrics.Next(cnt);
            //int nextDir2 = HexMetrics.Previous(nextDir);

            //Vector3 v1 = cell.GetCornerSolid(nextDir);
            //Vector3 v2 = cell.GetCornerSolid(nextDir2);

            //Vector3 v3 = cell.GetCornerSolid(nextDir, size);
            //Vector3 v4 = cell.GetCornerSolid(nextDir2, size);


            //Vector3 v5 = v3 + bridge;
            //Vector3 v6 = v4 + bridge;

            //Vector3 v7 = cell.Position + bridge;
            //Vector3 v8 = v7 + bridge;
            //terrain.AddTriangle(v1, v3, v5);
            //terrain.AddTriangleColor(cell.Color);

            //terrain.AddTriangle(v6, v4, v2);
            //terrain.AddTriangleColor(cell.Color);

            //if (neighbor.HasRiver)
            //{
            //    v7.y = v8.y = cell.StreamBedY;
            //}
            //terrain.AddQuad(v7, v3, v8, v5);
            //terrain.AddQuadColor(cell.Color);

            //terrain.AddQuad(v4, v7, v6, v8);
            //terrain.AddQuadColor(cell.Color);


            //CreateGameObject(cell.ToString() + "  " + cnt + "   v1 ", v1);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v2 ", v2);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v3 ", v3);

            //CreateGameObject(cell.ToString() + "  " + cnt + "   v4 ", v4);
            //CreateGameObject(cell.ToString() + "  " + cnt + "   v5 ", v3 + bridge);

            **/

        }

    }
    private void TriangulateBaseNormal(SequareCell cell)
    {
        Vector3 center = cell.Position;
        for (int cnt = 0; cnt < HexMetrics.directionNum; cnt++)
        {
            Vector3 v1 = cell.GetCornerSolid(cnt);
            Vector3 v2 = cell.GetCornerSolid(cnt + 1);
            EdgeVertices e = new EdgeVertices(v1, v2);
            TriangulateEdgeFan(center, e, cell.Color);
        }
    }

    /// <summary>
    /// 绘制格子之间链接
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="direction"></param>
    private void TriangulateConnection(SequareCell cell, int direction)
    {
        SequareCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        int nextDiretion = HexMetrics.Next(direction);
        int oppDirection = HexMetrics.Opposite(direction);
        Vector3 v1 = cell.GetCornerSolid(direction);
        Vector3 v2 = cell.GetCornerSolid(nextDiretion);

        Vector3 v3 = neighbor.GetCornerSolid(HexMetrics.Opposite(nextDiretion));
        Vector3 v4 = neighbor.GetCornerSolid(oppDirection);

        EdgeVertices e1 = new EdgeVertices(v1, v2);
        EdgeVertices e2 = new EdgeVertices(v3, v4);

        //CreateGameObject(cell.ToString() + " v1 ", e1.v1);
        //CreateGameObject(cell.ToString() + " v2 ", e1.v5);

        //CreateGameObject(cell.ToString() + " v3 ", e2.v1);
        //CreateGameObject(cell.ToString() + " v4 ", e2.v5);


       

        if (cell.HasRiverThroughEdge(direction))
        {
            e1.v3.y = cell.StreamBedY;
            e2.v3.y = neighbor.StreamBedY;

            //CreateGameObject(cell.ToString() + "  e1.v3 ", e1.v3);
            //CreateGameObject(cell.ToString() + "  e2.v3 ", e2.v3);
          
        }
        if (neighbor.HasRiverThroughEdge(oppDirection))
        {
           

        }
        
        TriangulateEdgeStrip(e1, cell.Color, e2, neighbor.Color);



    }

    /// <summary>
    /// 填补空隙 方形
    /// </summary>
    /// <param name="cell"></param>
    private void TriangulateGap(SequareCell cell)
    {

        SequareCell neighborE = cell.GetNeighbor(EnumDirection.East);
        SequareCell neighborEs = cell.GetNeighbor(EnumDirection.EastSouth);
        SequareCell neighborS = cell.GetNeighbor(EnumDirection.South);

        if (neighborE == null || neighborEs == null || neighborS == null)
        {
            return;
        }
        Vector3 v0 = cell.GetCornerSolid(1);
        Vector3 v1 = neighborS.GetCornerSolid(0);
        Vector3 v2 = neighborE.GetCornerSolid(2);
        Vector3 v3 = neighborEs.GetCornerSolid(3);
        Vector4 v4 = cell.GetCorner(1);




        Color centerColor = (cell.Color + neighborE.Color + neighborEs.Color + neighborS.Color) / 4;
        Vector3[] vers = new Vector3[] { v0, v2, v3, v1, v0 };
        Color[] colors = new Color[] { cell.Color, neighborE.Color, neighborEs.Color, neighborS.Color, cell.Color };
        v4.y = Math.Min((v0.y + v3.y) / 2, (v1.y + v2.y) / 2);


        int indexNext;
        for (int i = 0; i < HexMetrics.directionNum; i++)
        {
            indexNext = i + 1;
            terrain.AddTriangle(v4, vers[i], vers[indexNext]);
            terrain.AddTriangleColor(centerColor, colors[i], colors[indexNext]);
        }

    }



    private void TriangulateRiverQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, float y)
    {
        v1.y = v2.y = v3.y = v4.y = y;
        rivers.AddQuad(v1, v2, v3, v4);
        rivers.AddQuadUV(0f, 1f, 0f, 1f);
    }

    //CreateGameObject(cell.ToString() + " v1 ", v1);
    public void CreateGameObject(string strName, Vector3 pos)
    {
        GameObject objV1 = new GameObject(strName);
        objV1.transform.localPosition = pos;
    }


    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        terrain.AddTriangle(center, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);

        terrain.AddTriangle(center, edge.v2, edge.v3);
        terrain.AddTriangleColor(color);

        terrain.AddTriangle(center, edge.v3, edge.v4);
        terrain.AddTriangleColor(color);

        terrain.AddTriangle(center, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
    }

    void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2)
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);

        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);

        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);

        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);
    }
    void TriangulateEdgeFan(Vector3 center, EdgeVerticesNum edge, Color color)
    {
        int lenght = edge.vs.Length - 1;
        for (int cnt = 0; cnt < lenght; cnt++)
        {
            terrain.AddTriangle(center, edge.vs[cnt], edge.vs[cnt + 1]);
            terrain.AddTriangleColor(color);
        }
    }

    void TriangulateEdgeStrip(EdgeVerticesNum e1, Color c1, EdgeVerticesNum e2, Color c2)
    {
        int lenght = e1.vs.Length - 1;
        for (int cnt = 0; cnt < lenght; cnt++)
        {
            terrain.AddQuad(e1.vs[cnt], e1.vs[cnt + 1], e2.vs[cnt], e2.vs[cnt + 1]);
            terrain.AddQuadColor(c1, c2);
        }
    }
    public override string ToString()
    {
        return base.ToString();
    }
}



//if (cell.HasRiverThroughEdge((EnumDirection)direction))
//{
//    e.v3.y = cell.StreamBedY;
//    if (cell.HasRiverBeginOrEnd)
//    {
//        //Debug.LogFormat("   起点 Or 终点 11111   " + cell.ToString());
//        TriangulateWithRiverBeginOrEnd(direction, cell, e);
//    }
//    else
//    {
//        TriangulateWithRiverGrid(direction, cell, e);
//    }
//}
//else
//{
//    // Debug.LogFormat("   起点 Or 终点   " + cell.ToString());
//    //TriangulateAdjacentToRiver(direction, cell, e);
//}




///// <summary>
///// 创建河流中心格子
///// </summary>
//private void TriangulateWithRiverCenter(SequareCell cell)
//{
//    Vector3 center = cell.Position;
//    center.y = cell.StreamBedY;
//    //Debug.LogFormat("{0}  {1}   ", cell.Position, cell.ToString());
//    float size = HexMetrics.solidFactor * HexMetrics.riverWidthRate;

//    Vector3 v1 = cell.GetCornerSolid(3, size);
//    Vector3 v2 = cell.GetCornerSolid(2, size);

//    Vector3 v3 = cell.GetCornerSolid(0, size);
//    Vector3 v4 = cell.GetCornerSolid(1, size);

//    EdgeVerticesHalf e1 = new EdgeVerticesHalf(v1, v2);
//    EdgeVerticesHalf e2 = new EdgeVerticesHalf(v3, v4);

//    TriangulateEdgeStrip(e1, cell.Color, e2, cell.Color);


//Vector3 bridge = HexMetrics.GetBridge(1);
//Vector3 v5 = v1 + bridge;
//Vector3 v6 = v3 + bridge;
//v5.y = v6.y = cell.StreamBedY;

//terrain.AddQuad(v1, v5, v3, v6);
//terrain.AddQuadColor(cell.Color);

//terrain.AddQuad(v5, v2, v6, v4);
//terrain.AddQuadColor(cell.Color);



//    CreateGameObject(cell.ToString() + "  " + 0 + "   v1 ", v1);
//    CreateGameObject(cell.ToString() + "  " + 1 + "   v2 ", v2);
//    CreateGameObject(cell.ToString() + "  " + 2 + "   v3 ", v3);
//    CreateGameObject(cell.ToString() + "  " + 3 + "   v4 ", v4);

//    //CreateGameObject(cell.ToString() + "  " + 5 + "   v5 ", v1);

//}

///// <summary>
///// 创建河流格子
///// </summary>
//private void TriangulateWithRiverBridge(SequareCell cell)
//{




//}
///// <summary>
///// 河流开始 or 结束
///// </summary>
//private void TriangulateWithRiverBeginOrEnd(int direction, SequareCell cell, EdgeVertices e)
//{
//    Vector3 center = cell.Position;
//    if (cell.HasRiverThroughEdge(HexMetrics.Next(direction)))
//    {
//        if (cell.HasRiverThroughEdge(HexMetrics.Previous(direction)))
//        {
//            center += HexMetrics.GetSolidEdgeMiddle(direction) * (HexMetrics.innerToOuter * 0.5f);
//        }
//        else if (cell.HasRiverThroughEdge(HexMetrics.Previous(direction, 2)))
//        {
//            center += HexMetrics.GetCornerSolid(direction) * 0.25f;
//        }
//        else if (cell.HasRiverThroughEdge(HexMetrics.Previous(direction)) && cell.HasRiverThroughEdge(HexMetrics.Next(direction, 2)))
//        {
//            center += HexMetrics.GetCornerSolid(direction) * 0.25f;
//        }
//    }

//    EdgeVertices m = new EdgeVertices(Vector3.Lerp(center, e.v1, 0.5f), Vector3.Lerp(center, e.v5, 0.5f));
//    m.v3.y = e.v3.y;
//    TriangulateEdgeStrip(m, cell.Color, e, cell.Color);
//    TriangulateEdgeFan(center, m, cell.Color);

//}