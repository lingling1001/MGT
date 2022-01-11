using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoomData
{
    public EnumRoomType RoomType;
    public int Id;
    public int Width;
    public int Length;
    public int Area;
    public Rect RoomRect;
    public Vector3 Position;
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

    public void MoveTo(Vector3 inPos)
    {
        RoomRect.center = new Vector2(inPos.x, inPos.z);
        Position = new Vector3(inPos.x, 0, inPos.z);
    }


    public bool Overlap(MapRoom other)
    {
        return this.RoomRect.Overlaps(other.RoomRect, true);
    }
}
