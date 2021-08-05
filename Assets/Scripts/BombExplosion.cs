using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    // 폭발 효과 프리펩 변수
    public GameObject explosion;

    // 폭발 변경 
    public float explosionRadius = 5.0f;

    AudioSource bSource;

    void start()
    {
        bSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = Instantiate(explosion);
        go.transform.position = transform.position;

        // 자신의 위치에서 일정한 반경만큼을 검색한다. 그 범위안에 에너미를 찾는다.
        //Physics.OverlapSphere(transform.position, explosionRadius)

        Destroy(gameObject);
        

    }
}
