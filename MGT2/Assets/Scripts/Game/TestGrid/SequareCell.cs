using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequareCell : MonoBehaviour
{
    public Color Color { get { return color; } }
    private Color color;
    public SequareCoordinates coordinates;
    public RectTransform uiRect;
    public SequareGridChunk chunk;
    private Transform trans;
    public Transform Trans
    {
        get
        {
            if (trans == null)
            {
                trans = transform;
            }
            return trans;
        }
    }
    public Vector3 Position { get { return Trans.localPosition; } }
    /// <summary>
    /// �߶� ����
    /// </summary>
    private int elevation = int.MinValue;
    public int Elevation { get { return elevation; } }

    public int ElevationResult { get { return (int)(Elevation * HexMetrics.elevationStep); } }

    [SerializeField]
    private Dictionary<int, SequareCell> mapNeighbors = new Dictionary<int, SequareCell>();
    public Dictionary<int, SequareCell> MapNeighbors { get { return mapNeighbors; } }
    /// <summary>
    /// �����ھ�
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="cell"></param>
    public void SetNeighbor(EnumDirection direction, SequareCell cell)
    {
        int key = (int)direction;
        mapNeighbors[key] = cell;
    }
    /// <summary>
    /// ���ú��θ߶�
    /// </summary>
    public void SetElevation(int elevation)
    {
        if (Elevation == elevation)
        {
            return;
        }
        this.elevation = elevation;
        Vector3 position = transform.localPosition;

        position = HexMetrics.CalcRealElevation(position, elevation);

        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;

        RefreshRiver();
        Refresh();

    }
    /// <summary>
    /// ������ɫ
    /// </summary>
    public void SetColor(Color color)
    {
        if (Color == color)
        {
            return;
        }
        this.color = color;
        Refresh();
    }
    /// <summary>
    /// ˢ���ھ��б�
    /// </summary>
    private static List<SequareGridChunk> _tempList = new List<SequareGridChunk>();
    public void Refresh()
    {
        if (chunk == null)
        {
            return;
        }
        _tempList.Clear();
        _tempList.Add(chunk);
        foreach (var item in MapNeighbors)
        {
            if (item.Value.chunk == null)
            {
                continue;
            }
            if (!_tempList.Contains(item.Value.chunk))
            {
                _tempList.Add(item.Value.chunk);
            }
        }
        for (int cnt = 0; cnt < _tempList.Count; cnt++)
        {
            _tempList[cnt].Refresh();
        }
    }
    /// <summary>
    /// ˢ������
    /// </summary>
    public void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
    public int GetNeighborDirection(SequareCell cell)
    {
        foreach (var item in MapNeighbors)
        {
            if (item.Value == cell)
            {
                return item.Key;
            }

        }
        return -1;
    }
    public SequareCell GetNeighbor(EnumDirection direction)
    {
        int key = (int)direction;
        return GetNeighbor(key);
    }
    public SequareCell GetNeighbor(int direction)
    {
        if (mapNeighbors.ContainsKey(direction))
        {
            return mapNeighbors[direction];
        }
        return null;
    }

    /// <summary>
    /// ��ȡ��ǰ���ӱ߳� ����
    /// </summary>
    public Vector3 GetCorner(int direction)
    {
        return Position + HexMetrics.GetCorner(direction);
    }
    /// <summary>
    /// ��ȡ���ӱ߳� ����
    /// </summary>
    public Vector3 GetCornerSolid(int direction, float value = HexMetrics.solidFactor)
    {
        return Position + HexMetrics.GetCorner(direction) * value;
    }
    /// <summary>
    /// �����ھӻ�ȡ��ǰ�ĺ�������
    /// </summary>
    public EnumEdgeType GetEdgeType(int direction)
    {
        return HexMetrics.GetEdgeType(Elevation, GetNeighbor(direction).Elevation);
    }

    #region   river  ����
    bool hasIncomingRiver, hasOutgoingRiver;
    /// <summary>
    /// ���뷽��  outgoingRiver�������� �����ĸ�����
    /// </summary>
    EnumDirection incomingRiver, outgoingRiver;
    /// <summary>
    /// �������
    /// </summary>
    public bool HasIncomingRiver { get { return hasIncomingRiver; } }
    /// <summary>
    /// ��������
    /// </summary>
    public bool HasOutgoingRiver { get { return hasOutgoingRiver; } }
    public EnumDirection IncomingRiver { get { return incomingRiver; } }
    public EnumDirection OutgoingRiver { get { return outgoingRiver; } }
    public bool HasRiver { get { return hasIncomingRiver || hasOutgoingRiver; } }
    public bool HasRiverBeginOrEnd { get { return hasIncomingRiver != hasOutgoingRiver; } }

    public void SetHasIncomingRiver(bool value, EnumDirection direction = EnumDirection.East)
    {
        hasIncomingRiver = value;
        incomingRiver = direction;
    }
    public bool HasIncomingRiverDirection(bool value, EnumDirection direction)
    {
        return HasIncomingRiver && IncomingRiver == direction;
    }
    public void SetHasOutgoingRiver(bool value, EnumDirection direction = EnumDirection.East)
    {
        hasOutgoingRiver = value;
        outgoingRiver = direction;
    }
    public bool HasOutgoingRiverDirection(bool value, EnumDirection direction)
    {
        return HasOutgoingRiver && OutgoingRiver == direction;
    }
    public bool HasRiverThroughEdge(int direction)
    {
        return HasRiverThroughEdge((EnumDirection)direction);
    }
    /// <summary>
    /// �к���ͨ��
    /// </summary>
    public bool HasRiverThroughEdge(EnumDirection direction)
    {
        return hasIncomingRiver && incomingRiver == direction
            || hasOutgoingRiver && outgoingRiver == direction;
    }

    private void RefreshRiver()
    {
        if (HasOutgoingRiver)
        {
            if (Elevation < GetNeighbor(OutgoingRiver).Elevation)
            {
                RemoveOutgoingRiver();
            }
        }
        if (HasIncomingRiver)
        {
            if (Elevation > GetNeighbor(IncomingRiver).Elevation)
            {
                RemoveIncomingRiver();
            }
        }

    }
    /// <summary>
    /// �������
    /// </summary>
    public void RemoveOutgoingRiver()
    {
        if (!HasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        SequareCell neighbor = GetNeighbor(OutgoingRiver);
        neighbor.SetHasIncomingRiver(false);
        neighbor.RefreshSelfOnly();

    }
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        SequareCell neighbor = GetNeighbor(incomingRiver);
        neighbor.SetHasOutgoingRiver(false);
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveIncomingRiver();
        RemoveOutgoingRiver();
    }
    /// <summary>
    /// ���ú���
    /// </summary>
    public void SetOutgoingRiver(EnumDirection direction)
    {
        if (HasOutgoingRiverDirection(true, direction))
        {
            return;
        }
        //������ھ� ������� ���ء�
        SequareCell neighbor = GetNeighbor(direction);
        if (neighbor == null || Elevation < neighbor.Elevation)
        {
            return;
        }
        RemoveOutgoingRiver();
        if (HasIncomingRiverDirection(true, direction))
        {
            RemoveIncomingRiver();
        }

        SetHasOutgoingRiver(true, direction);

        RefreshSelfOnly();

        neighbor.RemoveIncomingRiver();
        neighbor.SetHasIncomingRiver(true, HexMetrics.Opposite(direction));
        neighbor.RefreshSelfOnly();

    }

    public float StreamBedY { get { return (elevation + HexMetrics.streamBedElevationOffset) * HexMetrics.elevationStep; } }
    public float RiverSurfaceY { get { return (elevation + HexMetrics.riverSurfaceElevationOffset) * HexMetrics.elevationStep; } }



    #endregion


    public override string ToString()
    {
        return coordinates.ToString();
    }
}
