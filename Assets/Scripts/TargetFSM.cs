using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TargetFSM : MonoBehaviour
{
    // 상태별로 타겟이 상황에 맞는 행동을 하게 하고 싶다...
    // 타겟 상태
    // 상태별 함수?
    // switch???
    // 타겟은... 대기 > 공격을 받는다 > 죽음 > 게임오버!(이건 게임매니저스크립트에서)

    enum TargetState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    TargetState targetState;

    // 플레이어 게임 오브젝트
    GameObject player;

    // 감지 범위
    public float findDistance = 10.0f;

    // 나의 캐릭터 컨트롤러
    CharacterController cc;

    // 이동속도
    public float moveSpeed = 5f;

    // 공격 가능 범위
    public float attackDistance = 2.0f;

    // 현재 누적 시간 변수
    private float currentTime = 0;

    // 공격 딜레이 시간
    public float attackDelayTime = 5.0f;

    // 공격력 변수
    public int attackPower = 10;

    // 초기 위치 저장용 변수
    Vector3 originPos;
    Quaternion originRot;

    // 이동 가능한 거리
    public float moveDistance = 20.0f;

    // 슬라이더 변수
    public Slider hpSlider;

    // 애니메이터 컴포넌트 변수
    Animator anim;

    // 최대 체력 변수
    public int maxHp = 10;

    // 현재 체력 변수
    public int hp;

    // 네비게이션 메쉬 에이전트
    NavMeshAgent Neo;

    // HitEvent 컴포넌트
    HitEvent hEvent;

    // 건물 점수
    static int TargetScore = 0;
    static int lastScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        // 초기 상태는 대기 상태
        targetState = TargetState.Idle;

        // 플레이어 검색
        player = GameObject.Find("Player");

        // 캐릭터 컨트롤러 받아오기??
        cc = GetComponent<CharacterController>();

        // 현재 체력 설정
        hp = maxHp;

        // 초기 위치와 회전 저장하기
        originPos = transform.position;
        originRot = transform.rotation;

        // 자식 오브젝트의 애니메이터 컴포넌트를 가져온다.
        anim = GetComponentInChildren<Animator>();

        // 네브메쉬 에이전트 컴포넌트 가져오기
        Neo = GetComponent<NavMeshAgent>();
        Neo.speed = moveSpeed;
        Neo.stoppingDistance = attackDistance;


    }

    // Update is called once per frame
    void Update()
    {
        switch (targetState)
        {
            case TargetState.Idle:
                Idle();
                break;
            case TargetState.Move:
                Move();
                break;
            case TargetState.Attack:
                Attack();
                break;
            case TargetState.Return:
                Return();
                break;
            case TargetState.Damaged:
                Damaged();
                break;
            case TargetState.Die:
                Die();
                break;
        }

        // hp슬라이더의 값에 체력 비율을 적용한다.
        hpSlider.value = (float)hp / maxHp;

        // 게임 상태가 게임 중 상태가 아니라면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
    }


    // 대기 행동 함수
    void Idle()
    {
        // 만일, 플레이어와의 거리가 감지 범위 이내라면...
        if (Vector3.Distance(player.transform.position, transform.position) <= findDistance)
        {
            // 상태를 이동상태로 변경한다.
            targetState = TargetState.Move;
            print("상태전환: Idle -> Move");
            anim.SetTrigger("IdleToMove");

            print("무브애니!!");
        }
    }

    // 이동 행동 함수
    void Move()
    {
        // 만일, 이동 거리 밖이라면...
        if (Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // 상태를 복귀 상태로 전환한다.
            targetState = TargetState.Return;
            print("상태 전환: Move -> Return");
        }

        // 만일, 공격 범위 밖이라면..
        else if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
        {
            // 이동 방향을 구한다.
            // Vector3 dir = (player.transform.position - transform.position).normalized;

            // 나의 전방방향을 이동방향과 일치시킨다. 
            // transform.forward = dir;

            // 캐릭터 컨트롤러로 이동 방향을 이동한다.
            // cc.Move(dir * moveSpeed * Time.deltaTime);

            // 네브메쉬 에이전트를 이용하여 타켓 방향으로 이동한다.
            Neo.SetDestination(player.transform.position);
            Neo.stoppingDistance = attackDistance;
        }

        // 공격 범위 안에 들어오면...
        else
        {
            // 상태를 공격 상태로 변경한다.
            targetState = TargetState.Attack;
            print("상태전환:Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // 공격 대기 시간을 미리 누적(바로 공격할 수 있게!) 
            currentTime = attackDelayTime;

            // 이동을 멈추고, 타겟을 초기화한다.
            Neo.isStopped = true;
            Neo.ResetPath();
        }
    }

    // 공격 행동 함수
    void Attack()
    {
        // 게임 상태가 게임 중 상태가 아니라면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 만일, 플레이어가 공격 범위 이내라면...
        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // 만일, 현재의 대기 시간이 공격 대기 시간을 넘어갔다면...
            if (currentTime >= attackDelayTime)
            {
                currentTime = 0;
                // 플레이어를 공격한다.
                print("공격!");

                PlayerMove pm = player.GetComponent<PlayerMove>();
                pm.OnDamage(attackPower);
                print("어택파워!!");

                anim.SetTrigger("StartAttack");
                print("어택 애니메이션");
            }
            else
            {
                // 시간을 누적한다.
                currentTime += Time.deltaTime;
            }
        }
        else
        {
            // 상태를 이동 상태로 전환한다.
            targetState = TargetState.Move;
            print("상태전환: Attack -> Move");

            anim.SetTrigger("AttackToMove");
        }
    }

    // 플레이어에게 데미지를 주는 함수
    //public void HitEvent()
    //{
    //    PlayerMove pm = player.GetComponent<PlayerMove>();
    //    pm.OnDamage(attackPower);

    //    print("데미지");
    //}

    // 복귀 행동 함수
    void Return()
    {
        // 만일, 원래 위치에 도달하지 않았다면, 그 방향으로 이동한다.
        if (Vector3.Distance(originPos, transform.position) > 0.1f)
        {
            // Vector3 dir = (originPos - transform.position).normalized;
            // transform.forward = dir;

            // cc.Move(dir * moveSpeed * Time.deltaTime);

            Neo.SetDestination(originPos);
            Neo.stoppingDistance = 0;
        }

        // 원래 위치에 도달하면 대기 상태로 전환한다.
        else
        {
            Neo.isStopped = true;
            Neo.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            targetState = TargetState.Idle;
            print("상태 전환: Return -> Idle");
            anim.SetTrigger("MoveToIdle");

            // 체력을 최대치로 회복한다.
            // hp = maxHp;
        }
    }

    // 피격 행동 함수
    void Damaged()
    {
        //코르틴 함수를 실행한다.
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        // 1초간 정지한다.
        yield return new WaitForSeconds(1.0f);

        // 상태를 이동 상태로 전환한다.
        targetState = TargetState.Move;
        print("상태 전환: Damaged -> Move");
    }

    // 사망 행동 함수
    void Die()
    {
        // 기존의 예약된 코루틴들을 모두 종료
        // StopAllCoroutines();

        // 스코어 올리는 이벤트 실행
        EventManager.RunTargetDiesEvent();

        // 사망 코루틴을 시작한다.
        StartCoroutine(DieProcess());


        //print("사망코루틴함수 실행 시작");
    }

    IEnumerator DieProcess()
    {
        //print("사망코루틴함수 실행을 시작한다.");

        // 캐릭터 컨트롤러를 비활성화한다.
        cc.enabled = false;

        // 2초간 기다렸다가 몸체를 제거한다.
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        //TargetScorePlus();

       //print("사망코루틴함수 실행 중");
    }
    /*
    public int TargetScorePlus()
    {
        TargetScore++;
        print("타겟 점수 :" + TargetScore);
        return TargetScore;
       
    }*/

    // 데미지 처리 함수
    public void HitTarget(int value)
    {
        // 만일, 나의 상태가 피격, 복귀, 사망 상태일 때에는 함수를 종료한다.
        if (targetState == TargetState.Damaged || targetState == TargetState.Return || targetState == TargetState.Die)
        {
            return;
        }

        hp -= value;

        // 만일, 남은 hp가 0보다 크다면...
        if (hp > 0)
        {
            // 상태를 피격 상태로 전환한다.
            targetState = TargetState.Damaged;
            print("상태 전환: Any state -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        // 그렇지 않다면,
        else
        {
            // 상태를 사망 상태로 전환한다.
            targetState = TargetState.Die;
            print("상태전환: Any state -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
