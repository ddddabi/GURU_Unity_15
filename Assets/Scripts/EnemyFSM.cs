using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    // 상태별로 에너미가 상황에 맞는 행동을 하게 하고 싶다.
    // 1. 에너미 상태
    // 2. 상태별 함수
    // 3. switch문을 통해 상태를 체크하고, 상태별 함수를 실행한다.

    enum EnemyState
    {
        Idle, // 대기 상태, 타겟 주위를 경계한다.
        Move, // 이동 상태, 플레이어가 타겟을 공격 전, 플레이어가 감지되면 그 쪽으로 이동한다.
        Attack, // 공격 상태, 플레이어가 타겟을 공격 전, 플레이어를 공격한다.
        Return, // 복귀 상태, 이동 범위가 벗어나면 원위치로 복귀한다.
        // Chase, // 추적 상태, 타겟이 공격을 받음 > 0.5초 후 플레이어를 쫓는다.
        Damaged, // 피격 상태, 타겟에게 공격을 받음
        Die // 플레이어에게 죽음
    }

    // 에너미 상태 변수
    EnemyState enemyState;

    // 플레이어 게임 오브젝트
    GameObject player;

    // 플레이어 발견 범위
    public float findDistance = 15.0f;

    // 플레이어 트랜스폼
    // Transform player;

    // 공격 가능 범위
    public float attackDistance = 2.0f;

    // 이동속도
    public float moveSpeed = 5.0f;

    // 나의 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;

    // 현재 누적 시간 변수
    float currentTime = 0;

    // 공격 딜레이 시간
    public float attackDelayTime = 2.0f;

    // 에너미 공격력
    public int attackPower = 2;

    // 최대 체력 변수
    public int maxHp = 10;

    // 현재 체력 변수
    public int Enemyhp;

    // 슬라이더 변수
    //public Slider hpSlider3;

    // 초기 위치 저장용 변수
    Vector3 originPos;
    Quaternion originRot;

    // 이동 가능한 거리
    public float moveDistance = 20.0f;

    // 애니메이터 컴포넌트 변수
    Animator anim;

    // 네비게이션 메쉬 에이전트
    NavMeshAgent smith;

    // Start is called before the first frame update
    void Start()
    {
        // 초기 상태는 대기 상태
        enemyState = EnemyState.Idle;

        // 플레이어 검색
        player = GameObject.Find("Player");

        // 캐릭터 컨트롤러 받아오기
        cc = GetComponent<CharacterController>();

        // 초기 위치와 회전 저장하기
        originPos = transform.position;
        originRot = transform.rotation;

        // 현재 체력 설정
        Enemyhp = maxHp;

        // 자식 오브젝트의 애니메이터 컴포넌트를 가져오기
        anim = transform.GetComponentInChildren<Animator>();

        // 네브메쉬 에이전트 컴포넌트 가져오기
        smith = GetComponent<NavMeshAgent>();
        smith.speed = moveSpeed;
        smith.stoppingDistance = attackDistance;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                Damaged();
                break;
            case EnemyState.Die:
                Die();
                break;

        }
        // 현재 플레이어 hp를 hp슬라이더의 value에 반영한다.
        //hpSlider3.value = (float)Enemyhp / maxHp;

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
            enemyState = EnemyState.Move;
            print("상태전환: Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }

    // 이동 행동 함수
    void Move()
    {
        // 만일, 이동 거리 밖이라면...
        if (Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // 상태를 복귀 상태로 전환한다.
            enemyState = EnemyState.Return;
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
            smith.SetDestination(player.transform.position);
            smith.stoppingDistance = attackDistance;
        }

        // 공격 범위 안에 들어오면...
        else
        {
            // 상태를 공격 상태로 변경한다.
            enemyState = EnemyState.Attack;
            print("상태전환:Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // 공격 대기 시간을 미리 누적(바로 공격할 수 있게!) 
            currentTime = attackDelayTime;

            // 이동을 멈추고, 타겟을 초기화한다.
            smith.isStopped = true;
            smith.ResetPath();
        }
    }

    // 공격 행동 함수
    void Attack()
    {
        // 만일, 플레이어가 공격 범위 이내라면...
        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // 일정한 시간마다 플레이어를 공격한다,
            currentTime += Time.deltaTime;

            // 만일, 현재의 대기 시간이 공격 대기 시간을 넘어갔다면...
            if (currentTime >= attackDelayTime)
            {
                // 플레이어를 공격한다.
                print("공격!");
                PlayerMove pm = player.GetComponent<PlayerMove>();
                pm.OnDamage(attackPower);
                currentTime = 0;

                anim.SetTrigger("StartAttack");
            }
        }
        else
        {
            // 상태를 이동 상태로 전환한다.
            enemyState = EnemyState.Move;
            print("상태전환: Attack -> Move");
            currentTime = 0;

            anim.SetTrigger("AttackToMove");
        }

        // 게임 상태가 게임 중 상태가 아니라면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
    }

    //// 플레이어에게 데미지를 주는 함수
    //public void HitEvent()
    //{
    //    PlayerMove pm = player.GetComponent<PlayerMove>();
    //    pm.OnDamage(attackPower);
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

            smith.SetDestination(originPos);
            smith.stoppingDistance = 0;
        }

        // 원래 위치에 도달하면 대기 상태로 전환한다.
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            // 체력을 최대치로 회복한다.
            Enemyhp = maxHp;

            enemyState = EnemyState.Idle;
            print("상태 전환: Return -> Idle");
            anim.SetTrigger("MoveToIdle");
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
        // 2초간 정지한다.
        yield return new WaitForSeconds(1.0f);

        // 상태를 이동 상태로 전환한다.
        enemyState = EnemyState.Move;
        print("상태 전환: Damaged -> Move");
    }

    // 사망 행동 함수
    void Die()
    {
        // 기존의 예약된 코루틴들을 모두 종료
        // StopAllCoroutines();

        // 사망 코루틴을 시작한다.
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // 캐릭터 컨트롤러를 비활성화한다.
        cc.enabled = false;

        // 2초간 기다렸다가 몸체를 제거한다.
        yield return new WaitForSeconds(1.0f);
        print("에너미 소멸");
        Destroy(gameObject);
    }

    // 데미지 처리 함수
    public void HitEnemy(int value)
    {
        // 만일, 나의 상태가 피격, 복귀, 사망 상태일 때에는 함수를 종료한다.
        if (enemyState == EnemyState.Damaged || enemyState == EnemyState.Return || enemyState == EnemyState.Die)
        {
            return;
        }

        Enemyhp -= value;

        // 만일, 남은 hp가 0보다 크다면...
        if (Enemyhp > 0)
        {
            // 상태를 피격 상태로 전환한다.
            enemyState = EnemyState.Damaged;
            print("상태 전환: Any state -> Damaged");

            anim.SetTrigger("Damaged");
            Damaged();
        }
        // 그렇지 않다면,
        else
        {
            // 상태를 사망 상태로 전환한다.
            enemyState = EnemyState.Die;
            print("상태전환: Any state -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
