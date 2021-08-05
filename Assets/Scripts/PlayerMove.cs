using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float gravity = -20.0f;

    public float jumpPower = 10.0f;

    public int maxJump = 2;

    int jumpCount = 0;

    float yVelocity = 0;

    public float moveSpeed = 7.0f;

    CharacterController cc;

    public int hp;

    public int maxHp = 10;

    public Slider hpSlider;

    // �ִϸ����� ������Ʈ ����
    Animator anim;

    // ����Ʈ UI ������Ʈ
   // public GameObject hitEffect;

    //public Slider TimeBar;

    //�ð�����
    /*public Text TimeCount;
    public float TimeCost;*/

    void Start()
    {
        cc = GetComponent<CharacterController>();
        hp = maxHp;
        // �ڽ� ������Ʈ�� �ִϸ����� ������Ʈ�� �����´�
        anim = GetComponentInChildren<Animator>();
    }

    
    void Update()
    {
        hpSlider.value = (float)hp / (float)maxHp; 

        // ���� ���°� ���� ���� �ƴϸ� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        //imeCost -= TimeCost.deltaTime;
        //TimeCount.text = "�����ð�" + TimeCost;


        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // �̵� ������ �����Ѵ�.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        // �̵� ���� ������ ũ�� ���� �ִϸ������� �̵� ���� Ʈ���� �����Ѵ�.
        //anim.SetFloat("MoveDirection", dir.magnitude);

        dir = Camera.main.transform.TransformDirection(dir);

        if(cc.collisionFlags == CollisionFlags.Below)
        {
            jumpCount = 0;
            yVelocity = 0;
        }

        if(Input.GetButtonDown("Jump") && jumpCount < maxJump)
        {
            jumpCount++;
            yVelocity = jumpPower;
        }

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        cc.Move(dir * moveSpeed * Time.deltaTime);

       
        //TimeBar.value = (float)hp / (float)maxHp;

    }
    // �÷��̾� �ǰ� �Լ�
    public void OnDamage(int value)
    {
        // ���� ���°� ���� �� ���°� �ƴ϶�� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // Ÿ�ٿ� �ִ� ��Ʈ�̺�Ʈ ������Ʈ ���� �ҷ��ͼ� �����Ű��
        //HitEvent hEvent = target.GetComponent<HitEvent>();
        //hEvent.OnHit();
        //print("�÷��̾� �ǰ� �Լ� Ȱ��ȭ");

        hp -= value;
        print("�µ����� Ȱ��ȭ!!!");

        if (hp < 0)
        {
            hp = 0;
        }
        /*
        // hp�� 0���� ū ��쿡�� �ǰ� ����Ʈ �ڷ�ƾ�� �����Ѵ�.
        else
        {
            StartCoroutine(HitEffect());
        }*/
    }
    /*
    IEnumerator HitEffect()
    {
        // 1. ����Ʈ�� �Ҵ�.(Ȱ��ȭ��Ų��.)
        hitEffect.SetActive(true);

        // 2. 0.3�ʸ� ��ٸ���.
        yield return new WaitForSeconds(0.3f);

        // 3. ����Ʈ�� ����.(��Ȱ��ȭ��Ų��.)
        hitEffect.SetActive(false);
    }*/

    /*IEnumerator GameOver()
    {
        // ���� �ð��� �� �ǰų�, ü���� 0���� �۾�����
        // Ending �� Ȱ��ȭ��Ų��.
    }*/
}
