using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SequareCoordinates
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int z;
    public int X { get { return x; } }
    public int Z { get { return z; } }
    public SequareCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }


    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Z.ToString() + ")";
    }

}
