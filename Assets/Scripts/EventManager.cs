using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action TargetDiesEvent;

    public static void RunTargetDiesEvent()
    {
        if (TargetDiesEvent != null)
        {
            TargetDiesEvent();
        }
    }
}
