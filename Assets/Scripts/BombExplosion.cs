using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    // ���� ȿ�� ������ ����
    public GameObject explosion;

    // ���� ���� 
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

        // �ڽ��� ��ġ���� ������ �ݰ游ŭ�� �˻��Ѵ�. �� �����ȿ� ���ʹ̸� ã�´�.
        //Physics.OverlapSphere(transform.position, explosionRadius)

        Destroy(gameObject);
        

    }
}
