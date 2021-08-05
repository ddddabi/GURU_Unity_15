using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TargetFSM : MonoBehaviour
{
    // ���º��� Ÿ���� ��Ȳ�� �´� �ൿ�� �ϰ� �ϰ� �ʹ�...
    // Ÿ�� ����
    // ���º� �Լ�?
    // switch???
    // Ÿ����... ��� > ������ �޴´� > ���� > ���ӿ���!(�̰� ���ӸŴ�����ũ��Ʈ����)

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

    // �÷��̾� ���� ������Ʈ
    GameObject player;

    // ���� ����
    public float findDistance = 10.0f;

    // ���� ĳ���� ��Ʈ�ѷ�
    CharacterController cc;

    // �̵��ӵ�
    public float moveSpeed = 5f;

    // ���� ���� ����
    public float attackDistance = 2.0f;

    // ���� ���� �ð� ����
    private float currentTime = 0;

    // ���� ������ �ð�
    public float attackDelayTime = 5.0f;

    // ���ݷ� ����
    public int attackPower = 10;

    // �ʱ� ��ġ ����� ����
    Vector3 originPos;
    Quaternion originRot;

    // �̵� ������ �Ÿ�
    public float moveDistance = 20.0f;

    // �����̴� ����
    public Slider hpSlider;

    // �ִϸ����� ������Ʈ ����
    Animator anim;

    // �ִ� ü�� ����
    public int maxHp = 10;

    // ���� ü�� ����
    public int hp;

    // �׺���̼� �޽� ������Ʈ
    NavMeshAgent Neo;

    // HitEvent ������Ʈ
    HitEvent hEvent;

    // �ǹ� ����
    static int TargetScore = 0;
    static int lastScore = 0;


    // Start is called before the first frame update
    void Start()
    {
        // �ʱ� ���´� ��� ����
        targetState = TargetState.Idle;

        // �÷��̾� �˻�
        player = GameObject.Find("Player");

        // ĳ���� ��Ʈ�ѷ� �޾ƿ���??
        cc = GetComponent<CharacterController>();

        // ���� ü�� ����
        hp = maxHp;

        // �ʱ� ��ġ�� ȸ�� �����ϱ�
        originPos = transform.position;
        originRot = transform.rotation;

        // �ڽ� ������Ʈ�� �ִϸ����� ������Ʈ�� �����´�.
        anim = GetComponentInChildren<Animator>();

        // �׺�޽� ������Ʈ ������Ʈ ��������
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

        // hp�����̴��� ���� ü�� ������ �����Ѵ�.
        hpSlider.value = (float)hp / maxHp;

        // ���� ���°� ���� �� ���°� �ƴ϶�� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
    }


    // ��� �ൿ �Լ�
    void Idle()
    {
        // ����, �÷��̾���� �Ÿ��� ���� ���� �̳����...
        if (Vector3.Distance(player.transform.position, transform.position) <= findDistance)
        {
            // ���¸� �̵����·� �����Ѵ�.
            targetState = TargetState.Move;
            print("������ȯ: Idle -> Move");
            anim.SetTrigger("IdleToMove");

            print("����ִ�!!");
        }
    }

    // �̵� �ൿ �Լ�
    void Move()
    {
        // ����, �̵� �Ÿ� ���̶��...
        if (Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // ���¸� ���� ���·� ��ȯ�Ѵ�.
            targetState = TargetState.Return;
            print("���� ��ȯ: Move -> Return");
        }

        // ����, ���� ���� ���̶��..
        else if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
        {
            // �̵� ������ ���Ѵ�.
            // Vector3 dir = (player.transform.position - transform.position).normalized;

            // ���� ��������� �̵������ ��ġ��Ų��. 
            // transform.forward = dir;

            // ĳ���� ��Ʈ�ѷ��� �̵� ������ �̵��Ѵ�.
            // cc.Move(dir * moveSpeed * Time.deltaTime);

            // �׺�޽� ������Ʈ�� �̿��Ͽ� Ÿ�� �������� �̵��Ѵ�.
            Neo.SetDestination(player.transform.position);
            Neo.stoppingDistance = attackDistance;
        }

        // ���� ���� �ȿ� ������...
        else
        {
            // ���¸� ���� ���·� �����Ѵ�.
            targetState = TargetState.Attack;
            print("������ȯ:Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // ���� ��� �ð��� �̸� ����(�ٷ� ������ �� �ְ�!) 
            currentTime = attackDelayTime;

            // �̵��� ���߰�, Ÿ���� �ʱ�ȭ�Ѵ�.
            Neo.isStopped = true;
            Neo.ResetPath();
        }
    }

    // ���� �ൿ �Լ�
    void Attack()
    {
        // ���� ���°� ���� �� ���°� �ƴ϶�� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // ����, �÷��̾ ���� ���� �̳����...
        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // ����, ������ ��� �ð��� ���� ��� �ð��� �Ѿ�ٸ�...
            if (currentTime >= attackDelayTime)
            {
                currentTime = 0;
                // �÷��̾ �����Ѵ�.
                print("����!");

                PlayerMove pm = player.GetComponent<PlayerMove>();
                pm.OnDamage(attackPower);
                print("�����Ŀ�!!");

                anim.SetTrigger("StartAttack");
                print("���� �ִϸ��̼�");
            }
            else
            {
                // �ð��� �����Ѵ�.
                currentTime += Time.deltaTime;
            }
        }
        else
        {
            // ���¸� �̵� ���·� ��ȯ�Ѵ�.
            targetState = TargetState.Move;
            print("������ȯ: Attack -> Move");

            anim.SetTrigger("AttackToMove");
        }
    }

    // �÷��̾�� �������� �ִ� �Լ�
    //public void HitEvent()
    //{
    //    PlayerMove pm = player.GetComponent<PlayerMove>();
    //    pm.OnDamage(attackPower);

    //    print("������");
    //}

    // ���� �ൿ �Լ�
    void Return()
    {
        // ����, ���� ��ġ�� �������� �ʾҴٸ�, �� �������� �̵��Ѵ�.
        if (Vector3.Distance(originPos, transform.position) > 0.1f)
        {
            // Vector3 dir = (originPos - transform.position).normalized;
            // transform.forward = dir;

            // cc.Move(dir * moveSpeed * Time.deltaTime);

            Neo.SetDestination(originPos);
            Neo.stoppingDistance = 0;
        }

        // ���� ��ġ�� �����ϸ� ��� ���·� ��ȯ�Ѵ�.
        else
        {
            Neo.isStopped = true;
            Neo.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            targetState = TargetState.Idle;
            print("���� ��ȯ: Return -> Idle");
            anim.SetTrigger("MoveToIdle");

            // ü���� �ִ�ġ�� ȸ���Ѵ�.
            // hp = maxHp;
        }
    }

    // �ǰ� �ൿ �Լ�
    void Damaged()
    {
        //�ڸ�ƾ �Լ��� �����Ѵ�.
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        // 1�ʰ� �����Ѵ�.
        yield return new WaitForSeconds(1.0f);

        // ���¸� �̵� ���·� ��ȯ�Ѵ�.
        targetState = TargetState.Move;
        print("���� ��ȯ: Damaged -> Move");
    }

    // ��� �ൿ �Լ�
    void Die()
    {
        // ������ ����� �ڷ�ƾ���� ��� ����
        // StopAllCoroutines();

        // ���ھ� �ø��� �̺�Ʈ ����
        EventManager.RunTargetDiesEvent();

        // ��� �ڷ�ƾ�� �����Ѵ�.
        StartCoroutine(DieProcess());


        //print("����ڷ�ƾ�Լ� ���� ����");
    }

    IEnumerator DieProcess()
    {
        //print("����ڷ�ƾ�Լ� ������ �����Ѵ�.");

        // ĳ���� ��Ʈ�ѷ��� ��Ȱ��ȭ�Ѵ�.
        cc.enabled = false;

        // 2�ʰ� ��ٷȴٰ� ��ü�� �����Ѵ�.
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        //TargetScorePlus();

       //print("����ڷ�ƾ�Լ� ���� ��");
    }
    /*
    public int TargetScorePlus()
    {
        TargetScore++;
        print("Ÿ�� ���� :" + TargetScore);
        return TargetScore;
       
    }*/

    // ������ ó�� �Լ�
    public void HitTarget(int value)
    {
        // ����, ���� ���°� �ǰ�, ����, ��� ������ ������ �Լ��� �����Ѵ�.
        if (targetState == TargetState.Damaged || targetState == TargetState.Return || targetState == TargetState.Die)
        {
            return;
        }

        hp -= value;

        // ����, ���� hp�� 0���� ũ�ٸ�...
        if (hp > 0)
        {
            // ���¸� �ǰ� ���·� ��ȯ�Ѵ�.
            targetState = TargetState.Damaged;
            print("���� ��ȯ: Any state -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        // �׷��� �ʴٸ�,
        else
        {
            // ���¸� ��� ���·� ��ȯ�Ѵ�.
            targetState = TargetState.Die;
            print("������ȯ: Any state -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
