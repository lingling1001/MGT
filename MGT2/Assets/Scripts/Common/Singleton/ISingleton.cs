using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ISingleton<T>
{
    void On_Init();
    void On_Release();
}

