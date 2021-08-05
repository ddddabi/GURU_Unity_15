using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    // ���º��� ���ʹ̰� ��Ȳ�� �´� �ൿ�� �ϰ� �ϰ� �ʹ�.
    // 1. ���ʹ� ����
    // 2. ���º� �Լ�
    // 3. switch���� ���� ���¸� üũ�ϰ�, ���º� �Լ��� �����Ѵ�.

    enum EnemyState
    {
        Idle, // ��� ����, Ÿ�� ������ ����Ѵ�.
        Move, // �̵� ����, �÷��̾ Ÿ���� ���� ��, �÷��̾ �����Ǹ� �� ������ �̵��Ѵ�.
        Attack, // ���� ����, �÷��̾ Ÿ���� ���� ��, �÷��̾ �����Ѵ�.
        Return, // ���� ����, �̵� ������ ����� ����ġ�� �����Ѵ�.
        // Chase, // ���� ����, Ÿ���� ������ ���� > 0.5�� �� �÷��̾ �Ѵ´�.
        Damaged, // �ǰ� ����, Ÿ�ٿ��� ������ ����
        Die // �÷��̾�� ����
    }

    // ���ʹ� ���� ����
    EnemyState enemyState;

    // �÷��̾� ���� ������Ʈ
    GameObject player;

    // �÷��̾� �߰� ����
    public float findDistance = 15.0f;

    // �÷��̾� Ʈ������
    // Transform player;

    // ���� ���� ����
    public float attackDistance = 2.0f;

    // �̵��ӵ�
    public float moveSpeed = 5.0f;

    // ���� ĳ���� ��Ʈ�ѷ� ������Ʈ
    CharacterController cc;

    // ���� ���� �ð� ����
    float currentTime = 0;

    // ���� ������ �ð�
    public float attackDelayTime = 2.0f;

    // ���ʹ� ���ݷ�
    public int attackPower = 2;

    // �ִ� ü�� ����
    public int maxHp = 10;

    // ���� ü�� ����
    public int Enemyhp;

    // �����̴� ����
    //public Slider hpSlider3;

    // �ʱ� ��ġ ����� ����
    Vector3 originPos;
    Quaternion originRot;

    // �̵� ������ �Ÿ�
    public float moveDistance = 20.0f;

    // �ִϸ����� ������Ʈ ����
    Animator anim;

    // �׺���̼� �޽� ������Ʈ
    NavMeshAgent smith;

    // Start is called before the first frame update
    void Start()
    {
        // �ʱ� ���´� ��� ����
        enemyState = EnemyState.Idle;

        // �÷��̾� �˻�
        player = GameObject.Find("Player");

        // ĳ���� ��Ʈ�ѷ� �޾ƿ���
        cc = GetComponent<CharacterController>();

        // �ʱ� ��ġ�� ȸ�� �����ϱ�
        originPos = transform.position;
        originRot = transform.rotation;

        // ���� ü�� ����
        Enemyhp = maxHp;

        // �ڽ� ������Ʈ�� �ִϸ����� ������Ʈ�� ��������
        anim = transform.GetComponentInChildren<Animator>();

        // �׺�޽� ������Ʈ ������Ʈ ��������
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
        // ���� �÷��̾� hp�� hp�����̴��� value�� �ݿ��Ѵ�.
        //hpSlider3.value = (float)Enemyhp / maxHp;

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
            enemyState = EnemyState.Move;
            print("������ȯ: Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
    }

    // �̵� �ൿ �Լ�
    void Move()
    {
        // ����, �̵� �Ÿ� ���̶��...
        if (Vector3.Distance(originPos, transform.position) > moveDistance)
        {
            // ���¸� ���� ���·� ��ȯ�Ѵ�.
            enemyState = EnemyState.Return;
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
            smith.SetDestination(player.transform.position);
            smith.stoppingDistance = attackDistance;
        }

        // ���� ���� �ȿ� ������...
        else
        {
            // ���¸� ���� ���·� �����Ѵ�.
            enemyState = EnemyState.Attack;
            print("������ȯ:Move -> Attack");

            anim.SetTrigger("MoveToAttackDelay");

            // ���� ��� �ð��� �̸� ����(�ٷ� ������ �� �ְ�!) 
            currentTime = attackDelayTime;

            // �̵��� ���߰�, Ÿ���� �ʱ�ȭ�Ѵ�.
            smith.isStopped = true;
            smith.ResetPath();
        }
    }

    // ���� �ൿ �Լ�
    void Attack()
    {
        // ����, �÷��̾ ���� ���� �̳����...
        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            // ������ �ð����� �÷��̾ �����Ѵ�,
            currentTime += Time.deltaTime;

            // ����, ������ ��� �ð��� ���� ��� �ð��� �Ѿ�ٸ�...
            if (currentTime >= attackDelayTime)
            {
                // �÷��̾ �����Ѵ�.
                print("����!");
                PlayerMove pm = player.GetComponent<PlayerMove>();
                pm.OnDamage(attackPower);
                currentTime = 0;

                anim.SetTrigger("StartAttack");
            }
        }
        else
        {
            // ���¸� �̵� ���·� ��ȯ�Ѵ�.
            enemyState = EnemyState.Move;
            print("������ȯ: Attack -> Move");
            currentTime = 0;

            anim.SetTrigger("AttackToMove");
        }

        // ���� ���°� ���� �� ���°� �ƴ϶�� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
    }

    //// �÷��̾�� �������� �ִ� �Լ�
    //public void HitEvent()
    //{
    //    PlayerMove pm = player.GetComponent<PlayerMove>();
    //    pm.OnDamage(attackPower);
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

            smith.SetDestination(originPos);
            smith.stoppingDistance = 0;
        }

        // ���� ��ġ�� �����ϸ� ��� ���·� ��ȯ�Ѵ�.
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            // ü���� �ִ�ġ�� ȸ���Ѵ�.
            Enemyhp = maxHp;

            enemyState = EnemyState.Idle;
            print("���� ��ȯ: Return -> Idle");
            anim.SetTrigger("MoveToIdle");
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
        // 2�ʰ� �����Ѵ�.
        yield return new WaitForSeconds(1.0f);

        // ���¸� �̵� ���·� ��ȯ�Ѵ�.
        enemyState = EnemyState.Move;
        print("���� ��ȯ: Damaged -> Move");
    }

    // ��� �ൿ �Լ�
    void Die()
    {
        // ������ ����� �ڷ�ƾ���� ��� ����
        // StopAllCoroutines();

        // ��� �ڷ�ƾ�� �����Ѵ�.
        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        // ĳ���� ��Ʈ�ѷ��� ��Ȱ��ȭ�Ѵ�.
        cc.enabled = false;

        // 2�ʰ� ��ٷȴٰ� ��ü�� �����Ѵ�.
        yield return new WaitForSeconds(1.0f);
        print("���ʹ� �Ҹ�");
        Destroy(gameObject);
    }

    // ������ ó�� �Լ�
    public void HitEnemy(int value)
    {
        // ����, ���� ���°� �ǰ�, ����, ��� ������ ������ �Լ��� �����Ѵ�.
        if (enemyState == EnemyState.Damaged || enemyState == EnemyState.Return || enemyState == EnemyState.Die)
        {
            return;
        }

        Enemyhp -= value;

        // ����, ���� hp�� 0���� ũ�ٸ�...
        if (Enemyhp > 0)
        {
            // ���¸� �ǰ� ���·� ��ȯ�Ѵ�.
            enemyState = EnemyState.Damaged;
            print("���� ��ȯ: Any state -> Damaged");

            anim.SetTrigger("Damaged");
            Damaged();
        }
        // �׷��� �ʴٸ�,
        else
        {
            // ���¸� ��� ���·� ��ȯ�Ѵ�.
            enemyState = EnemyState.Die;
            print("������ȯ: Any state -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }
}
