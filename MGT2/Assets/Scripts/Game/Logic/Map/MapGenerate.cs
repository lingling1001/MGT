using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate
{
    /// <summary>
    /// �����Χ
    /// </summary>
    public float CellCreationRadius = 5;
    /// <summary>
    /// ��������
    /// </summary>
    public int NumberOfCells = 4;
    /// <summary>
    /// �����б�
    /// </summary>
    public List<MapRoom> ListRooms { get; private set; }
    /// <summary>
    /// ��С���
    /// </summary>
    public int MinWidth = 5;
    /// <summary>
    /// �����
    /// </summary>
    public int MaxWidth = 15;
    /// <summary>
    /// ��С����
    /// </summary>
    public int MinLength = 5;
    /// <summary>
    /// ��󳤶�
    /// </summary>
    public int MaxLength = 15;
    /// <summary>
    /// �ص����ƶ�����
    /// </summary>
    public int MoveStep = 3;
    /// <summary>
    /// �������
    /// </summary>
    public int RandomSpeed = -1;
    public void GenerateMapInfo()
    {
        if (RandomSpeed == -1)
        {
            Random.InitState((int)System.DateTime.Now.Ticks);
        }
        else
        {
            Random.InitState(RandomSpeed);
        }
        ListRooms = new List<MapRoom>(NumberOfCells);
        int maxCount = NumberOfCells;
        while (ListRooms.Count < maxCount)
        {
            Vector2 pos = Random.insideUnitCircle * CellCreationRadius;
            GameObject obj = new GameObject();
            MapRoom room = obj.AddMissingComponent<MapRoom>();
            int w = Random.Range(MinWidth, MaxWidth + 1);
            int l = Random.Range(MinLength, MaxLength + 1);
            room.CreateRoom(ListRooms.Count, w, l);
            room.MoveTo(new Vector3Int((int)pos.x, 0, (int)pos.y));

            int index = 0;
            MapRoom first = GetRoomByIndex(index);
            while (first != null)
            {
                if (first == room)
                {
                    index++;
                    first = GetRoomByIndex(index);
                    continue;
                }
                if (CanMovePosition(room, first, 3))
                {
                    index = -1;
                    Vector3Int position = room.Position * MoveStep;
                    room.MoveTo(position);
                    continue;
                }
                index++;
                first = GetRoomByIndex(index);
            }
            ListRooms.Add(room);


            for (int cnt = 0; cnt < ListRooms.Count; cnt++)
            {
                ListRooms[cnt].gameObject.name = "room_" + cnt;
            }

        }
    }

    private bool CanMovePosition(MapRoom mr1, MapRoom mr2, int minDistance)
    {
        if (mr1.Overlap(mr2))
        {
            return true;
        }
        return false;
    }

    private MapRoom GetRoomByIndex(int index)
    {
        if (index > -1 && index < ListRooms.Count)
        {
            return ListRooms[index];
        }
        return null;
    }
}
