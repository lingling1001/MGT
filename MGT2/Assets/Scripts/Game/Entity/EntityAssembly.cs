using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAssembly
{
    public int EntityId { get; private set; }

    public void SetEntityId(int id)
    {
        EntityId = id;
    }
}
