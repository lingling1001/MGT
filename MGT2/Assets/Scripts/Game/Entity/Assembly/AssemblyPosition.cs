using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyPosition : AssemblyBase
{
    public Vector3 Position;
    public void SetValue(Vector3 value)
    {
        Position = value;
    }
}
