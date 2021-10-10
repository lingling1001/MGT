using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SequareMesh : MonoBehaviour
{

    public Mesh hexMesh;
    public bool useCollider = true;
    public bool useColors = true;
    public bool useUVCoordinates = true;

    

    [NonSerialized] List<Vector3> vertices;
    [NonSerialized] List<Color> colors;
    [NonSerialized] List<int> triangles;
    [NonSerialized] List<Vector2> uvs;

    private MeshCollider meshCollider;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "SequareMesh";
        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        if (useColors)
        {
            colors = ListPool<Color>.Get();
        }
        if (useUVCoordinates)
        {
            uvs = ListPool<Vector2>.Get();
        }
        triangles = ListPool<int>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);
        if (useColors)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }
        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);

        if (useCollider)
        {
            meshCollider.sharedMesh = hexMesh;
        }
        if (useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }
        hexMesh.RecalculateNormals();

    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }



    /// <summary>
    /// 在当前格子上绘制边缘信息
    /// </summary>
    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;

        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        vertices.Add(HexMetrics.Perturb(v4));

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);

        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);


    }

  
   


    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
    }

    public void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    public void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);

    }
    public void AddQuadColor(Color c1)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c1);

    }
}


//public void Triangulate(SequareCell[] cells)
//{
//    hexMesh.Clear();
//    vertices.Clear();
//    triangles.Clear();
//    colors.Clear();
//    for (int cnt = 0; cnt < cells.Length; cnt++)
//    {
//        Triangulate(cells[cnt]);
//    }
//    hexMesh.vertices = vertices.ToArray();
//    hexMesh.triangles = triangles.ToArray();
//    hexMesh.colors = colors.ToArray();
//    hexMesh.RecalculateNormals();
//    meshCollider.sharedMesh = hexMesh;
//}
//Vector3 bridge = HexMetrics.GetBridge(direction);
//Vector3 v0 = cell.GetCornerSolid(intDirection);
//Vector3 v1 = cell.GetCornerSolid(intDirection + 1);

//Vector3 v2 = v0 + bridge;
//Vector3 v3 = v1 + bridge;

//v1.y = v0.y = cell.Position.y;
//v2.y = v3.y = neighbor.Position.y;

//Vector3 e1 = Vector3.Lerp(v0, v1, 1f / 3f);
//Vector3 e2 = Vector3.Lerp(v0, v1, 2f / 3f);

//Vector3 e3 = Vector3.Lerp(v2, v3, 1f / 3f);
//Vector3 e4 = Vector3.Lerp(v2, v3, 2f / 3f);

//AddQuad(v0, e1, v2, e3);
//AddQuadColor(cell.Color, neighbor.Color);
//AddQuad(e1, e2, e3, e4);
//AddQuadColor(cell.Color, neighbor.Color);
//AddQuad(e2, v1, e4, v3);
//AddQuadColor(cell.Color, neighbor.Color);

//CreateGameObject(cell.ToString() + " v1 ", v1);
//CreateGameObject(cell.ToString() + " v3 ", v3);
//CreateGameObject(cell.ToString() + " v0 ", v0);
//CreateGameObject(cell.ToString() + " v2 ", v2);
///// <summary>
///// 创建梯田
///// </summary>
//private void TriangulateTerraces(Vector3 v0, Vector3 v1, SequareCell cell, Vector3 v2, Vector3 v3, SequareCell neighbor)
//{
//    Vector3 tempV0 = v0;
//    Vector3 tempV1 = v1;
//    Color tempColorCell = neighbor.Color;
//    Color tempColorCell2 = tempColorCell;
//    int steps = HexMetrics.terraceSteps;
//    for (int cnt = 0; cnt < steps; cnt++)
//    {
//        Vector3 v5 = HexMetrics.TerraceLerp(v0, v2, cnt);
//        Vector3 v6 = HexMetrics.TerraceLerp(v1, v3, cnt);
//        AddQuad(tempV1, v6, tempV0, v5);

//        int num = steps - cnt;
//        tempColorCell2 = HexMetrics.TerraceLerp(neighbor.Color, cell.Color, num);
//        AddQuadColor(tempColorCell, tempColorCell2);

//        tempV0 = v5;
//        tempV1 = v6;
//        tempColorCell = tempColorCell2;

//    }

//    AddQuad(tempV1, v3, tempV0, v2);
//    AddQuadColor(tempColorCell2, neighbor.Color);

//}

//private void TriangulateTerracesGap(SequareCell cell, SequareCell neighborE, SequareCell neighborEs, SequareCell neighborS)
//{

//    Vector3 v0 = cell.GetCornerSolid(1);
//    Vector3 v1 = neighborS.GetCornerSolid(0);
//    Vector3 v2 = neighborE.GetCornerSolid(2);
//    Vector3 v3 = neighborEs.GetCornerSolid(3);
//    Vector4 v4 = cell.GetCorner(1);
//    Color centerColor = (cell.Color + neighborE.Color + neighborEs.Color + neighborS.Color) / 4;
//    Vector3[] vers = new Vector3[] { v0, v2, v3, v1, v0 };
//    Color[] colors = new Color[] { cell.Color, neighborE.Color, neighborEs.Color, neighborS.Color, cell.Color };
//    v4.y = Math.Min((v0.y + v3.y) / 2, (v1.y + v2.y) / 2);

//    int indexNext;
//    for (int i = 0; i < HexMetrics.directionNum; i++)
//    {
//        indexNext = i + 1;
//        AddTriangle(v4, vers[i], vers[indexNext]);
//        AddTriangleColor(centerColor, colors[i], colors[indexNext]);
//    }

//}

//TriangulateTerracesGap(v4, vers[0], vers[1], centerColor, colors[0], colors[1]);
//TriangulateTerracesGapEast(v4, vers[1], vers[2], centerColor, colors[1], colors[2]);
//TriangulateTerracesGap(v4, vers[0], vers[1], centerColor, colors[0], colors[1]);
//TriangulateTerracesGap(v4, vers[0], vers[1], centerColor, colors[0], colors[1]);
//private void TriangulateTerracesGap(Vector3 v0, Vector3 v1, Vector3 v2, Color c0, Color c1, Color c2)
//{

//    int steps = HexMetrics.terraceSteps;
//    Vector3 v00 = v0;
//    Vector3 v22 = v2;

//    Color c00 = c1;
//    Color c22 = c2;

//    for (int cnt = 0; cnt < steps; cnt++)
//    {
//        Vector3 v11 = HexMetrics.TerraceLerp(v0, v1, cnt);
//        Vector3 v33 = HexMetrics.TerraceLerp(v2, v1, cnt);

//        AddQuad(v00, v22, v11, v33);

//        Color c11 = HexMetrics.TerraceLerp(c0, c1, cnt);
//        Color c33 = HexMetrics.TerraceLerp(c2, c1, cnt);

//        AddQuadColor(c00, c22, c11, c33);

//        v00 = v11;
//        v22 = v33;
//        c00 = c11;
//        c22 = c33;

//    }

//    AddTriangle(v00, v1, v22);
//    AddTriangleColor(c00, c1, c22);

//}

//int indexNext;
//for (int i = 0; i < HexMetrics.directionNum; i++)
//{

//    indexNext = i + 1;           
//    AddTriangle(v4, vers[i], vers[indexNext]);
//    AddTriangleColor(centerColor, colors[i], colors[indexNext]);
//}

//    public void Triangulate(EnumDirection direction, SequareCell cell)
//    {

//        Vector3 center = cell.transform.localPosition;
//        Vector3 cornerV1 = SequareMetrics.GetCorner(direction);
//        Vector3 cornerV2 = SequareMetrics.GetCorner(direction, 1);
//        Vector3 v1 = center + cornerV1 * SequareMetrics.solidFactor;
//        Vector3 v2 = center + cornerV2 * SequareMetrics.solidFactor;
//        Vector3 v3 = center + cornerV1;
//        Vector3 v4 = center + cornerV2;

//        AddTriangle(center, v1, v2);
//        AddTriangleColor(cell.color);

//        TriangulateConnection(direction, cell, v1, v2, v3, v4);





//    }

//    /// <summary>
//    /// 链接格子
//    /// </summary>
//    private void TriangulateConnection(EnumDirection direction, SequareCell cell, Vector3 v1, Vector3 v2, Vector3 v3, Vector4 v4)
//    {

//        SequareCell neighbor = cell.GetNeighbor(direction);
//        if (neighbor == null)
//        {
//            return;
//        }
//        Vector3 bridge = SequareMetrics.GetBridge(direction);
//        Vector3 v5 = v1 + bridge;
//        Vector3 v6 = v2 + bridge;
//        v5.y = v6.y = neighbor.ElevationResult;

//        for (int cnt = 0; cnt < SequareMetrics.terracesPerslope; cnt++)
//        {
//            //Vector3 sv2 = SequareMetrics.TerraceLerp(v2,)




//        }



//        AddTriangle(v2, v5, v6);

//        AddTriangleColor(cell.color, neighbor.color, neighbor.color);

//        if (direction != EnumDirection.East)
//        {
//            return;
//        }
//        EnumDirection nextDirection = direction.Next();

//        SequareCell neighborNext1 = cell.GetNeighbor(nextDirection);
//        SequareCell neighborNext2 = neighbor.GetNeighbor(nextDirection);
//        if (neighborNext1 == null || neighborNext1 == null)
//        {
//            return;
//        }
//        Color colorCenter = (cell.color + neighbor.color + neighborNext1.color + neighborNext2.color) / 4;

//        Vector3 nextBridge = SequareMetrics.GetBridge(nextDirection);
//        Vector3 v7 = v2 + nextBridge;
//        Vector3 v8 = v6 + nextBridge;

//        v2.y = cell.ElevationResult;
//        v7.y = neighborNext1.ElevationResult;
//        v6.y = neighbor.ElevationResult;
//        v8.y = neighborNext2.ElevationResult;

//        v4.y = (v7.y + v2.y + v6.y + v8.y) / 4;

//        AddTriangle(v7, v2, v4);
//        AddTriangleColor(neighborNext1.color, cell.color, colorCenter);

//        AddTriangle(v2, v6, v4);
//        AddTriangleColor(cell.color, neighbor.color, colorCenter);

//        AddTriangle(v6, v8, v4);
//        AddTriangleColor(neighbor.color, neighborNext2.color, colorCenter);

//        AddTriangle(v8, v7, v4);

//        AddTriangleColor(neighborNext2.color, neighborNext1.color, colorCenter);

//    }





//}

//SequareCell neigbbor = cell.GetNeighbor(direction);
//if (neigbbor == null)
//{
//    return;
//}
//Vector3 bridge = SequareMetrics.GetBridge(direction);
//Vector3 v3 = v1 + bridge;
//AddTriangle(v2, v1, v3);
//AddTriangleColor(cell.color, cell.color, neigbbor.color);

//Vector3 v3 = center + SequareMetrics.GetCorner(direction);
//Vector3 v4 = center + SequareMetrics.GetCorner(direction, 1);

//AddQual(v2, v4, v1, v3);
//AddQualColor(cell.color, Color.white, cell.color, Color.white);

//AddTriangle(v2, v1, v4);
//AddTriangleColor(cell.color, cell.color, neighbor.color);

//AddTriangle(v4, v1, v3);
//AddTriangleColor(cell.color, neighbor.color, neighbor.color);

//AddQual(v2, v4, v1, v3);
//Color edgeColor = (neighbor.color + cell.color) / 2;
//AddQualColor(cell.color, edgeColor, cell.color, edgeColor);
//SequareCell neighbor = cell.GetNeighbor(direction);
//if (neighbor == null)
//{
//    return;
//}

//Vector3 bridge = SequareMetrics.GetBridge(direction);
//Vector3 v5 = v1 + bridge;
//Vector3 v6 = v2 + bridge;
//AddTriangle(v2, v5, v6);
//AddTriangleColor(cell.color, neighbor.color, neighbor.color);
////AddTriangle(v4, v2, v6);
////AddTriangleColor(cell.color, cell.color, neighbor.color);
//if (direction != EnumDirection.East)
//{
//    return;
//}
//EnumDirection nextDirection = direction.Next();
//SequareCell nextNeighbor1 = cell.GetNeighbor(nextDirection);
//SequareCell nextNeighbor2 = neighbor.GetNeighbor(direction.Next());

//if (nextNeighbor1 == null || nextNeighbor2 == null)
//{
//    return;
//}
//Vector3 nextBridge = SequareMetrics.GetBridge(nextDirection);
//Vector3 v7 = v2 + nextBridge;
//Vector3 v8 = v6 + nextBridge;
////CreateGameObject(cell.ToString() + "v2", v2);
////CreateGameObject(cell.ToString() + "v6", v6);
////CreateGameObject(cell.ToString() + "v7", v7);
////CreateGameObject(cell.ToString() + "v8", v8);

//AddTriangle(v7, v2, v8);
//AddTriangleColor(nextNeighbor1.color, cell.color, nextNeighbor2.color);

//AddTriangle(v2, v6, v8);
//AddTriangleColor(cell.color, neighbor.color, nextNeighbor2.color);

////AddQual(v7, v8, v2, v6);
////Color c1 = (neighbor.color + nextNeighbor1.color + nextNeighbor2.color) / 3;

////AddQualColor(nextNeighbor1.color, c1, cell.color, c1);
//AddTriangle(v2, realV5, realV6);

//AddTriangleColor(cell.color, neighbor.color, neighbor.color);

//if (direction != EnumDirection.East)
//{
//    return;
//}
//EnumDirection nextDirection = direction.Next();

//SequareCell neighborNext1 = cell.GetNeighbor(nextDirection);
//SequareCell neighborNext2 = neighbor.GetNeighbor(nextDirection);
//if (neighborNext1 == null || neighborNext1 == null)
//{
//    return;
//}
//Color colorCenter = (cell.color + neighbor.color + neighborNext1.color + neighborNext2.color) / 4;

//Vector3 nextBridge = SequareMetrics.GetBridge(nextDirection);
//Vector3 v7 = v2 + nextBridge;
//Vector3 v8 = v6 + nextBridge;

//v2.y = cell.Elevation;
//v7.y = neighborNext1.Elevation;
//v6.y = neighbor.Elevation;
//v8.y = neighborNext2.Elevation;

//v4.y = (v7.y + v2.y + v6.y + v8.y) / 4;

//AddTriangle(v7, v2, v4);
//AddTriangleColor(neighborNext1.color, cell.color, colorCenter);

//AddTriangle(v2, v6, v4);
//AddTriangleColor(cell.color, neighbor.color, colorCenter);

//AddTriangle(v6, v8, v4);
//AddTriangleColor(neighbor.color, neighborNext2.color, colorCenter);

//AddTriangle(v8, v7, v4);

//AddTriangleColor(neighborNext2.color, neighborNext1.color, colorCenter);