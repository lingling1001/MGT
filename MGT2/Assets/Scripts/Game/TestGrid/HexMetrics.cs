using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class HexMetrics
{

    public const float outerToInner = 0.866025404f;
    public const float innerToOuter = 1f / outerToInner;

    public const float outerRadius = 10f;
    public const float innerRadius = 5f;

    public const int directionNum = 4;
    /// <summary>
    /// 格子大小
    /// </summary>
    public const float solidFactor = 0.8f;
    /// <summary>
    /// 格子边缘
    /// </summary>
    public const float blendFactor = 0.2f;
    /// <summary>
    /// 高度比例
    /// </summary>
    public const float elevationStep = 3f;

    public const int chunkSizeX = 5;
    public const int chunkSizeZ = 5;

    /// <summary>
    /// 梯田数量
    /// </summary>
    public const int terracesPerslope = 2;
    /// <summary>
    /// 梯田 
    /// </summary>
    public const int terraceSteps = terracesPerslope * 2 + 1;
    /// <summary>
    /// 梯田 步长
    /// </summary>
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerslope + 1);

    public static Vector3[] corners = {
        new Vector3(innerRadius, 0f, innerRadius),
        new Vector3(innerRadius, 0f, -innerRadius),
        new Vector3(-innerRadius, 0f, -innerRadius),
        new Vector3(-innerRadius, 0f, innerRadius),
        new Vector3(innerRadius, 0f, innerRadius),
    };

    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    public static SequareCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new SequareCoordinates(x, z);
    }
    public static SequareCoordinates FromPosition(Vector3 position)
    {
        int x = Mathf.CeilToInt((position.x + innerRadius) / outerRadius);
        int y = Mathf.CeilToInt((position.z + innerRadius) / outerRadius);
        return new SequareCoordinates(x - 1, y - 1);
    }
    public static int PositionConvertToIndex(SequareCoordinates coordinate, int height)
    {
        return coordinate.X + height * coordinate.Z;
    }
    public static int PositionConvertToIndex(int x, int y, int rowNum)
    {
        return y * rowNum + x;
    }
    public static Vector3 GetCorner(int index)
    {
        return corners[index];
    }
    public static Vector3 GetCorner(int direction, int index = 0)
    {
        return GetCorner((direction + index) % directionNum);
    }

    public static EnumEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return EnumEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return EnumEdgeType.Slope;
        }
        return EnumEdgeType.Cliff;
    }

    public static Vector3 GetSolidEdgeMiddle(int direction)
    {
        return (corners[direction] + corners[direction + 1]) * (0.5f * solidFactor);
    }
    public static Vector3 GetCornerSolid(int direction, int index = 0)
    {
        return GetCorner(direction, index) * solidFactor;
    }
    public static Vector3 GetBridge(int direction)
    {
        return (corners[direction] + corners[direction + 1]) * blendFactor;
    }

    public static EnumDirection Next(this EnumDirection direction, int index = 1)
    {
        return (EnumDirection)(((int)direction + index) % directionNum);
    }

    public static EnumDirection Previous(this EnumDirection direction, int index = 1)
    {
        int res = (int)direction - index;
        return res > -1 ? (EnumDirection)res : (EnumDirection)Mathf.Abs((directionNum + res) % directionNum);

    }
    public static int Opposite(int direction)
    {
        return (int)Opposite((EnumDirection)direction);
    }
    public static EnumDirection Opposite(EnumDirection direction)
    {
        switch (direction)
        {
            case EnumDirection.East:
                return EnumDirection.West;
            case EnumDirection.South:
                return EnumDirection.North;
            case EnumDirection.West:
                return EnumDirection.East;
            case EnumDirection.North:
                return EnumDirection.South;
            case EnumDirection.EastSouth:
            case EnumDirection.EastNorth:
            case EnumDirection.WestNorth:
            case EnumDirection.WestSouth:
                return EnumDirection.East;
        }
        return EnumDirection.East;
    }
    public static int Previous(int direction, int index = 1)
    {
        int res = direction - index;
        if (res > -1)
        {
            return res;
        }
        return Mathf.Abs((directionNum + res) % directionNum);
    }

    public static int Next(int direction, int index = 1)
    {
        return ((direction + index) % directionNum);
    }



}
/// <summary>
/// 噪声图 地图扰动
/// </summary>
public static partial class HexMetrics
{

    /// <summary>
    /// 噪声图
    /// </summary>
    public static Texture2D noiseSource;
    public const float cellPerturbStrength = 0f;
    //public const float cellPerturbStrength = 4f;

    public const float noiseScale = 0.003f;
    public const float elevationPerturbStrength = 1.5f;
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }
    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1) * cellPerturbStrength;
        position.z += (sample.z * 2f - 1) * cellPerturbStrength;
        return position;
    }
    public static Vector3 PerturbElevation(Vector3 position)
    {
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
        return position;
    }

    public static Vector3 CalcRealElevation(Vector3 position, float elevation)
    {
        position.y = elevation * HexMetrics.elevationStep;
        position = PerturbElevation(position);
        return position;
    }



}


/// <summary>
/// River
/// </summary>
public static partial class HexMetrics
{
    public const float streamBedElevationOffset = -1f;
    public const float riverSurfaceElevationOffset = -0.5f;
    public const float riverWidthRate = 0.5f;
}
public enum EnumEdgeType
{
    Flat,
    Slope,
    Cliff,

}
public enum EnumDirection : byte
{
    /// <summary>
    /// 东
    /// </summary>
    East = 0,
    /// <summary>
    /// 南
    /// </summary>
    South = 1,
    /// <summary>
    /// 西
    /// </summary>
    West = 2,
    /// <summary>
    /// 北
    /// </summary>
    North = 3,
    /// <summary>
    /// 东南
    /// </summary>
    EastSouth = 4,
    /// <summary>
    /// 东北
    /// </summary>
    EastNorth = 5,
    /// <summary>
    /// 西北
    /// </summary>
    WestNorth = 6,
    /// <summary>
    ///西南
    /// </summary>
    WestSouth = 7,
}

public enum EnumOptionalToggle
{
    Ignore = 0,
    Yes,
    No,

}