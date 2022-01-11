using UnityEngine;

public class MapPointLine
{
    public Vector3 PointStart;
    public Vector3 PointEnd;
    public int Distance { get; private set; }
    public MapPointLine(Vector3 x, Vector3 z)
    {
        PointStart = x;
        PointEnd = z;
        Distance = (int)Vector3.Distance(x, z);
    }
    public string GetNameStart()
    {
        return MapHelper.Vector3ToString(PointStart);
    }
    public string GetNameEnd()
    {
        return MapHelper.Vector3ToString(PointEnd);
    }
    public bool EqualPoint(string pointStart, string pointEnd)
    {
        if (GetNameStart() == pointStart && GetNameEnd() == pointEnd)
        {
            return true;
        }
        if (GetNameEnd() == pointStart && GetNameStart() == pointEnd)
        {
            return true;
        }
        return false;

    }
    public bool EqualPoint(Vector3 pointStart, Vector3 pointEnd)
    {
        if (PointStart == pointStart && PointEnd == pointEnd)
        {
            return true;
        }
        if (PointEnd == pointStart && PointStart == pointEnd)
        {
            return true;
        }
        return false;

    }
    public override string ToString()
    {
        return PointStart + " _ " + PointEnd;
    }

}