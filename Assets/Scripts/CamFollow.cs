using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform followPosition;

    void Start()
    {
        
    }

    
    void Update()
    {
        transform.position = followPosition.position;
    }
}
