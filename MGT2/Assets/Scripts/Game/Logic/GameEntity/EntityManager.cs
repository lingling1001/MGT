using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MFrameWork.Singleton<EntityManager>
{
    private Dictionary<int, GameObject> _mapEntities = new Dictionary<int, GameObject>();


    public GameObject CreateFromPrefab(string prefabPath,Vector3 pos, EnumTransParent parent)
    {
        return CreateFromPrefab(prefabPath, pos, 0, parent);
    }
    public GameObject CreateFromPrefab(string prefabPath, EnumTransParent parent)
    {
        return CreateFromPrefab(prefabPath, Vector3.zero, 0, parent);
    }
    public GameObject CreateFromPrefab(string prefabPath, Vector3 position, float rotation, EnumTransParent parent)
    {
        Quaternion quaternion = Quaternion.identity;
        if (rotation != 0)
        {
            quaternion = Quaternion.Euler(0, rotation, 0);
        }
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + prefabPath);
        GameObject go = GameObject.Instantiate<GameObject>(prefab, position, quaternion);
        go.transform.SetParent(TransparentNode.GetTransParent(parent));
        return go;
    }


}
