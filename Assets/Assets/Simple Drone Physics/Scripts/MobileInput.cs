using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    DroneController dc;
    void Start()
    {
        dc = GetComponent<DroneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount>0)
        {
            dc.rpm +=Time.deltaTime * 500;
        }
        
    }
}
