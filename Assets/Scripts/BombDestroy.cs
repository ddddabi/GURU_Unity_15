using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDestroy : MonoBehaviour
{
    // 경과된 시간 변수
    float currnetTime = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 생성된 때로부터 2초가 경과되면 사라진다.
        if (currnetTime >= 2)
        {
            Destroy(gameObject);
        }
        currnetTime += Time.deltaTime;
    }
}
