using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : MonoBehaviour
{
    // ����� �ð� ����
    float currnetTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ������ ���κ��� 2�ʰ� ����Ǹ� �������.
        if (currnetTime >= 2)
        {
            Destroy(gameObject);
        }
        currnetTime += Time.deltaTime;
    }
}
