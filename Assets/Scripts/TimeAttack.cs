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

            timeattack.text = "���� �ð� : " + (int)done + "sec";

        }

        else
        {

            // ���� â�� Ȱ��ȭ��Ų��.


        }


    }

}