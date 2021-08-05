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

    // 애니메이터 컴포넌트 변수
    Animator anim;

    // 이펙트 UI 오브젝트
   // public GameObject hitEffect;

    //public Slider TimeBar;

    //시간제한
    /*public Text TimeCount;
    public float TimeCost;*/

    void Start()
    {
        cc = GetComponent<CharacterController>();
        hp = maxHp;
        // 자식 오브젝트의 애니메이터 컴포넌트를 가져온다
        anim = GetComponentInChildren<Animator>();
    }

    
    void Update()
    {
        hpSlider.value = (float)hp / (float)maxHp; 

        // 게임 상태가 게임 중이 아니면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        //imeCost -= TimeCost.deltaTime;
        //TimeCount.text = "남은시간" + TimeCost;


        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 이동 방향을 설정한다.
        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();

        // 이동 방향 벡터의 크기 값을 애니메이터의 이동 블렌드 트리에 전달한다.
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
    // 플레이어 피격 함수
    public void OnDamage(int value)
    {
        // 게임 상태가 게임 중 상태가 아니라면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 타겟에 있는 히트이벤트 컴포넌트 변수 불러와서 적용시키기
        //HitEvent hEvent = target.GetComponent<HitEvent>();
        //hEvent.OnHit();
        //print("플레이어 피격 함수 활성화");

        hp -= value;
        print("온데미지 활성화!!!");

        if (hp < 0)
        {
            hp = 0;
        }
        /*
        // hp가 0보다 큰 경우에는 피격 이펙트 코루틴을 실행한다.
        else
        {
            StartCoroutine(HitEffect());
        }*/
    }
    /*
    IEnumerator HitEffect()
    {
        // 1. 이펙트를 켠다.(활성화시킨다.)
        hitEffect.SetActive(true);

        // 2. 0.3초를 기다린다.
        yield return new WaitForSeconds(0.3f);

        // 3. 이펙트를 끈다.(비활성화시킨다.)
        hitEffect.SetActive(false);
    }*/

    /*IEnumerator GameOver()
    {
        // 만약 시간이 다 되거나, 체력이 0보다 작아지면
        // Ending 을 활성화시킨다.
    }*/
}
