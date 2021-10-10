using UnityEngine;
using System.Collections;

public class TsEnable : MonoBehaviour
{

    void OnEnable()
    {

        Debug.Log(gameObject.name + " OnEnable   ");

    }
   
    void OnDisable()
    {

        Debug.Log(gameObject.name + "  OnDisable  ");

        
    }

    void Update()
    {

    }
}
