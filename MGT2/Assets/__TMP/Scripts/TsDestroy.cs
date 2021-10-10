using UnityEngine;
using System.Collections;

public class TsDestroy : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log(gameObject.name + "Start  OnDestroy  ");
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnDestroy()
    {

        Debug.Log(gameObject.name + " OnDestroy OnDestroy  ");
    }
}
