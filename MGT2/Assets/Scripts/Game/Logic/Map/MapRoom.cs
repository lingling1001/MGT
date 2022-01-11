using HullDelaunayVoronoi.Primitives;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom : MonoBehaviour
{
    public EnumRoomType RoomType;
    public int Id;
    public int Width;
    public int Length;
    public int Area;
    public Rect RoomRect;
    public Vector3Int Position;
    public void CreateRoom(int id, int w, int l)
    {
        Id = id;
        Width = w;
        Length = l;
        Area = w * l;
        RoomRect.width = w;
        RoomRect.height = l;
        RoomType = EnumRoomType.None;
    }

    public void MoveTo(Vector3Int inPos)
    {

        Position = inPos;
        RoomRect.center = new Vector2(inPos.x, inPos.z);
        transform.position = Position;
    }


    public bool Overlap(MapRoom other)
    {
        return this.RoomRect.Overlaps(other.RoomRect, true);
    }

    public override string ToString()
    {
        return "Room Id : " + Id.ToString();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, 0.2f, Length));
    }

}



public enum EnumRoomType
{
    None,
    Hall,
    Path,
}

public struct DelaunayEdge
{
    public Vertex2 StartPoint;
    public Vertex2 EndPoint;
    public float Length;

    public DelaunayEdge(Vertex2 start, Vertex2 end)
    {
        StartPoint = start;
        EndPoint = end;
        float dx = start.X - end.X;
        float dy = start.Y - end.Y;
        Length = dx * dx + dy * dy;
    }


}

