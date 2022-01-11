using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public int x;
    public int y;
    public int w;

    public Edge(int x, int y, int w)
    {
        this.x = x;
        this.y = y;
        this.w = w;
    }

    public override string ToString()
    {
        return string.Format("{0}-->{1} : {2}", x, y, w);
    }
}
