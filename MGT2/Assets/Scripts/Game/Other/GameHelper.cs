using MFrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper
{



    public static Vector3 GetRangePosition(Vector3 pos, int minX, int maxX,int minZ,int maxZ)
    {
        System.Random random = RandomHelper.GetRandom();
        return new Vector3(pos.x + random.Next(minX, maxX),
            pos.y,
            pos.z + random.Next(minZ, maxZ));
    }

    public static Vector3 GetRangePosition(Vector3 pos, int maxValue)
    {
        System.Random random = RandomHelper.GetRandom();
        return new Vector3(pos.x + random.Next(maxValue),
            pos.y,
            pos.z + random.Next(maxValue));
    }
}