using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TimeAttack : MonoBehaviour
{
    public static float done = 200.0f;
    public Text timeattack;

    void Update()
    {

        if (done > 0F)
        {

            done -= Time.deltaTime;

            timeattack.text = "남은 시간 : " + (int)done + "sec";

        }

        else
        {

            // 빨간 창을 활성화시킨다.


        }


    }

}