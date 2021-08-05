using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    // ����ź ������Ʈ
    public GameObject bombFactory;

    // �߻��� ��ġ
    public Transform firePosition;

    // �߻��� ��
    public float throwPower = 10.0f;

    // �Ѿ� ����Ʈ ���� ������Ʈ
    public GameObject bulletEffect;

    // ��ƼŬ �ý��� ���� ������Ʈ
    ParticleSystem ps;

    // ����� �ҽ� ������Ʈ ����
    AudioSource aSource;

    // �Ѿ� ���ݷ�
    public int attackPower = 2;

    // �ִϸ����� ������Ʈ
    Animator anim;

    // ���� ������ ��������Ʈ ����
    public GameObject weapon01;
    public GameObject weapon02;

    // ũ�ν���� ��������Ʈ ����
    public GameObject crosshair01;
    public GameObject crosshair02;

    // ���콺 ������ ��ư�� Ŭ�� ���� �� ��������Ʈ ����
    public GameObject weapon01_R;
    public GameObject weapon02_R;

    // ���콺 ������ ��ư�� Ŭ��, �ܸ�� ��������Ʈ ����
    public GameObject crosshair02_zoom;

    // ���� ��� ���
    enum WeaponMode
    { 
        Normal,
        Sniper
    }

    WeaponMode wMode;

    // ī�޶� ���� �ܾƿ��� üũ�ϱ� ���� ����
    bool isZoom = false;

    // ���� ��� �ؽ�Ʈ
    public Text weaponText;

    // �ѱ� ����Ʈ �迭
    public GameObject[] eff_Flash;

    // Ÿ�� ���� ������Ʈ ����
    GameObject target;

    // Ÿ��FSM ������Ʈ ����
    TargetFSM tFSM;

    // ���ʹ� ���� ������Ʈ ����
    GameObject enemy;

    // ���ʹ�FSM ������Ʈ ����
    EnemyFSM eFSM;

    void Start()
    {
        // �Ĵ�Ŭ �ý��� ������Ʈ ��������
        ps = bulletEffect.GetComponent<ParticleSystem>();

        // ������ҽ� ������Ʈ ��������
        aSource = GetComponent<AudioSource>();

        // Ÿ�� ������Ʈ�� �˻�
        target = GameObject.Find("Target");

        // Ÿ��FSM ������Ʈ �޾ƿ���
        //tFSM = target.GetComponent<TargetFSM>();

        // ���ʹ� ������Ʈ�� �˻�
        enemy = GameObject.Find("Enemy");

        // ���ʹ�FSM ������Ʈ �޾ƿ���
       // eFSM = enemy.GetComponent<EnemyFSM>();

        // �ڽ� ������Ʈ���� �ִϸ����� ��������
        anim = GetComponentInChildren<Animator>();

        // �ʱ� ���� ���� �Ϲ� ���� �Ѵ�.
        wMode = WeaponMode.Normal;
        weaponText.text = "Normal";
    }


    void Update()
    {
        // ���� ���°� ���� �� ���°� �ƴ϶�� ������Ʈ �Լ��� ����
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // ���콺 ��Ŭ���� �ϸ�, �ü� �������� �Ѿ��� �߻��ϰڴ�.
        if (Input.GetMouseButtonDown(0))
        {
            // 1. ���̸� ����
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 2. ���̿� �ε��� ����� ������ ������ ����
            RaycastHit hitInfo = new RaycastHit();

            // 3. ���̸� �߻��ؼ� �ε��� ����� �ִٸ�...
            if (Physics.Raycast(ray, out hitInfo))
            {
                // �ε��� ����� �̸��� �ܼ�â�� ����Ѵ�.
                //print(hitInfo.transform.name);

                // ����, �ε��� ����� ���̾ ���׹̶��...
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
                {
                    TargetFSM tFSM = hitInfo.transform.GetComponent<TargetFSM>();
                    tFSM.HitTarget(attackPower);
                    print("Ÿ�� ����");
                }

                // ����, �ε��� ����� ���̾ ���׹̶��...
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(attackPower);
                    print("���ʹ� ����");
                }

                // �ε��� ��ġ�� �Ѿ� ����Ʈ ������Ʈ�� ��ġ��Ų��.
                bulletEffect.transform.position = hitInfo.point;

                // �Ѿ� ����Ʈ�� ������ �ε��� ������Ʈ�� ��������� ��ġ��Ų��.
                bulletEffect.transform.forward = hitInfo.normal;

                // �Ѿ� ����Ʈ�� �÷����Ѵ�.
                ps.Play();
            }

            aSource.Play();
            /*
            // ����, ���� Ʈ���� MoveDirection �Ķ������ ���� 0�� ��..
            if (anim.GetFloat("MoveDirection") == 0)
            {
                // �� �߻� �ִϸ��̼��� �÷����Ѵ�.
                anim.SetTrigger("Attack");
            }
            */

            // �ѱ� ����Ʈ �ڷ�ƾ �Լ��� �����Ѵ�.
            StartCoroutine(ShootEffect(0.1f));
        }

        // ����, ���콺 ��Ŭ���� �Ѵٸ�...
        if (Input.GetMouseButtonDown(1))
        {
            // ���� ���� ��尡 ��� �����, ����ź�� ��ô�ϰڴ�.
            // ���� ���� ��尡 �������� �����, ī�޶� ���� �ܾƿ��� �ϰڴ�.
            switch (wMode)
            {
                case WeaponMode.Normal:
                    // ����ź ����
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.position;

                    // ����ź�� ������ �ٵ� ������Ʈ�� �޾ƿ��ڴ�.
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    // �ü� �������� �߻�!
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);

                    break;
                case WeaponMode.Sniper:
                    // ���࿡ �� �ƿ� ���¶�� 
                    // �� �� ���·� �����, ī�޶��� �þ߰��� 15���� �����Ѵ�.

                    if (!isZoom)
                    {
                        isZoom = true;
                        Camera.main.fieldOfView = 15.0f;

                        // �ܸ���� �� ũ�ν��� �����Ѵ�.
                        crosshair02_zoom.SetActive(true);
                        crosshair02.SetActive(false);
                    }
                    // �׷��� �ʴٸ�(�� �� ����)
                    // �� �ƿ� ���·� �����, ī�޶��� �þ߰��� 60���� �����Ѵ�.
                    else
                    {
                        isZoom = false;
                        Camera.main.fieldOfView = 60.0f;

                        // ũ�ν��� �������� ���� �������´�.
                        crosshair02_zoom.SetActive(false);
                        crosshair02.SetActive(true);
                    }
                    break;
            }



        }
        // ���� Ű �Է��� 1���̸� ��ָ��, 2���̸� �������� ���� ��ȯ�ϰڴ�.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            weaponText.text = "Normal Mode";
            // �ܾƿ� ���·� ��ȯ
            Camera.main.fieldOfView = 60.0f;
            isZoom = false;
            // 1�� ��������Ʈ�� Ȱ��ȭ�ǰ�, 2�� ��������Ʈ�� ��Ȱ��ȭ
            weapon01.SetActive(true);
            weapon02.SetActive(false);
            crosshair01.SetActive(true);
            crosshair02.SetActive(false);

            weapon01_R.SetActive(true);
            weapon02_R.SetActive(false);


            // �������� ��忡�� �Ϲ� ��带 ������ ��
            crosshair02_zoom.SetActive(false);

        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;
            weaponText.text = "Sniper Mode";
            // 1�� ��������Ʈ�� ��Ȱ��ȭ, 2�� ��������Ʈ�� Ȱ��ȭ
            weapon01.SetActive(false);
            weapon02.SetActive(true);
            crosshair01.SetActive(false);
            crosshair02.SetActive(true);

            weapon01_R.SetActive(false);
            weapon02_R.SetActive(true);
        }
    }
   // �ѱ� ����Ʈ �ڷ�ƾ �Լ�
   IEnumerator ShootEffect(float duration)
    {
        // �ټ����� ����Ʈ ������Ʈ �߿��� �����ϰ� 1���� ����.
        int num = Random.Range(0, eff_Flash.Length - 1);
        // ���õ� ������Ʈ�� Ȱ��ȭ��Ų��.
        eff_Flash[num].SetActive(true);
        // ���� �ð�(�෹�̼�) ���� ��ٸ���.
        yield return new WaitForSeconds(duration);
        // Ȱ��ȭ�� ������Ʈ�� �ٽ� ��Ȱ��ȭ��Ų��.
        eff_Flash[num].SetActive(false);
    }
}
