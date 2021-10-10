using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFrameWork
{
    public interface ISingleton<T>
    {
        void On_Init();
        void On_Release();
    }

}