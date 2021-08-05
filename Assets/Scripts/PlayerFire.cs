using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{
    // 수류탄 오브젝트
    public GameObject bombFactory;

    // 발사할 위치
    public Transform firePosition;

    // 발사할 힘
    public float throwPower = 10.0f;

    // 총알 이펙트 게임 오브젝트
    public GameObject bulletEffect;

    // 파티클 시스템 게임 오브젝트
    ParticleSystem ps;

    // 오디오 소스 컴포넌트 변수
    AudioSource aSource;

    // 총알 공격력
    public int attackPower = 2;

    // 애니메이터 컴포넌트
    Animator anim;

    // 무기 아이콘 스프라이트 변수
    public GameObject weapon01;
    public GameObject weapon02;

    // 크로스헤어 스프라이트 변수
    public GameObject crosshair01;
    public GameObject crosshair02;

    // 마우스 오른쪽 버튼을 클릭 했을 때 스프라이트 변수
    public GameObject weapon01_R;
    public GameObject weapon02_R;

    // 마우스 오른쪽 버튼을 클릭, 줌모드 스프라이트 변수
    public GameObject crosshair02_zoom;

    // 게임 모드 상수
    enum WeaponMode
    { 
        Normal,
        Sniper
    }

    WeaponMode wMode;

    // 카메라 줌인 줌아웃을 체크하기 위한 변수
    bool isZoom = false;

    // 무기 모드 텍스트
    public Text weaponText;

    // 총구 이펙트 배열
    public GameObject[] eff_Flash;

    // 타겟 게임 오브젝트 변수
    GameObject target;

    // 타겟FSM 컴포넌트 변수
    TargetFSM tFSM;

    // 에너미 게임 오브젝트 변수
    GameObject enemy;

    // 에너미FSM 컴포넌트 변수
    EnemyFSM eFSM;

    void Start()
    {
        // 파니클 시스템 컴포넌트 가져오기
        ps = bulletEffect.GetComponent<ParticleSystem>();

        // 오디오소스 컴포넌트 가져오기
        aSource = GetComponent<AudioSource>();

        // 타겟 오브젝트를 검색
        target = GameObject.Find("Target");

        // 타겟FSM 컴포넌트 받아오기
        //tFSM = target.GetComponent<TargetFSM>();

        // 에너미 오브젝트를 검색
        enemy = GameObject.Find("Enemy");

        // 에너미FSM 컴포넌트 받아오기
       // eFSM = enemy.GetComponent<EnemyFSM>();

        // 자식 오브젝트에서 애니메이터 가져오기
        anim = GetComponentInChildren<Animator>();

        // 초기 무기 모드는 일반 모드로 한다.
        wMode = WeaponMode.Normal;
        weaponText.text = "Normal";
    }


    void Update()
    {
        // 게임 상태가 게임 중 상태가 아니라면 업데이트 함수를 종료
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }

        // 마우스 좌클릭을 하면, 시선 방향으로 총알을 발사하겠다.
        if (Input.GetMouseButtonDown(0))
        {
            // 1. 레이를 생성
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            // 2. 레이에 부딪힌 대상의 정보를 저장할 변수
            RaycastHit hitInfo = new RaycastHit();

            // 3. 레이를 발사해서 부딪힌 대상이 있다면...
            if (Physics.Raycast(ray, out hitInfo))
            {
                // 부딪힌 대상의 이름을 콘솔창에 출력한다.
                //print(hitInfo.transform.name);

                // 만일, 부딪힌 대상의 레이어가 에네미라면...
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
                {
                    TargetFSM tFSM = hitInfo.transform.GetComponent<TargetFSM>();
                    tFSM.HitTarget(attackPower);
                    print("타겟 공격");
                }

                // 만일, 부딪힌 대상의 레이어가 에네미라면...
                if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    EnemyFSM eFSM = hitInfo.transform.GetComponent<EnemyFSM>();
                    eFSM.HitEnemy(attackPower);
                    print("에너미 공격");
                }

                // 부딪힌 위치에 총알 이펙트 오브젝트를 위치시킨다.
                bulletEffect.transform.position = hitInfo.point;

                // 총알 이펙트의 방향을 부딪힌 오브젝트의 수직방향과 일치시킨다.
                bulletEffect.transform.forward = hitInfo.normal;

                // 총알 이펙트를 플레이한다.
                ps.Play();
            }

            aSource.Play();
            /*
            // 만약, 블랜드 트리의 MoveDirection 파라메터의 값이 0일 때..
            if (anim.GetFloat("MoveDirection") == 0)
            {
                // 총 발사 애니메이션을 플레이한다.
                anim.SetTrigger("Attack");
            }
            */

            // 총구 이펙트 코루틴 함수를 실행한다.
            StartCoroutine(ShootEffect(0.1f));
        }

        // 만일, 마우스 우클릭을 한다면...
        if (Input.GetMouseButtonDown(1))
        {
            // 만일 무기 모드가 노멀 모드라면, 수류탄을 투척하겠다.
            // 만일 무기 모드가 스나이퍼 모드라면, 카메라 줌인 줌아웃을 하겠다.
            switch (wMode)
            {
                case WeaponMode.Normal:
                    // 수류탄 생성
                    GameObject bomb = Instantiate(bombFactory);
                    bomb.transform.position = firePosition.position;

                    // 수류탄의 리지드 바디 컴포넌트를 받아오겠다.
                    Rigidbody rb = bomb.GetComponent<Rigidbody>();

                    // 시선 방향으로 발사!
                    rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);

                    break;
                case WeaponMode.Sniper:
                    // 만약에 줌 아웃 상태라면 
                    // 줌 인 상태로 만들고, 카메라의 시야각을 15도로 변경한다.

                    if (!isZoom)
                    {
                        isZoom = true;
                        Camera.main.fieldOfView = 15.0f;

                        // 줌모드일 때 크로스헤어를 변경한다.
                        crosshair02_zoom.SetActive(true);
                        crosshair02.SetActive(false);
                    }
                    // 그렇지 않다면(줌 인 상태)
                    // 줌 아웃 상태로 만들고, 카메라의 시야각을 60도로 변경한다.
                    else
                    {
                        isZoom = false;
                        Camera.main.fieldOfView = 60.0f;

                        // 크로스헤어를 스나이퍼 모드로 돌려놓는다.
                        crosshair02_zoom.SetActive(false);
                        crosshair02.SetActive(true);
                    }
                    break;
            }



        }
        // 숫자 키 입력이 1번이면 노멀모드, 2번이면 스나이퍼 모드로 전환하겠다.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            wMode = WeaponMode.Normal;
            weaponText.text = "Normal Mode";
            // 줌아웃 상태로 전환
            Camera.main.fieldOfView = 60.0f;
            isZoom = false;
            // 1번 스프라이트는 활성화되고, 2번 스프라이트는 비활성화
            weapon01.SetActive(true);
            weapon02.SetActive(false);
            crosshair01.SetActive(true);
            crosshair02.SetActive(false);

            weapon01_R.SetActive(true);
            weapon02_R.SetActive(false);


            // 스나이퍼 모드에서 일반 모드를 눌렀을 때
            crosshair02_zoom.SetActive(false);

        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            wMode = WeaponMode.Sniper;
            weaponText.text = "Sniper Mode";
            // 1번 스프라이트는 비활성화, 2번 스프라이트는 활성화
            weapon01.SetActive(false);
            weapon02.SetActive(true);
            crosshair01.SetActive(false);
            crosshair02.SetActive(true);

            weapon01_R.SetActive(false);
            weapon02_R.SetActive(true);
        }
    }
   // 총구 이펙트 코루틴 함수
   IEnumerator ShootEffect(float duration)
    {
        // 다섯개의 이펙트 오브젝트 중에서 랜덤하게 1개를 고른다.
        int num = Random.Range(0, eff_Flash.Length - 1);
        // 선택된 오브젝트를 활성화시킨다.
        eff_Flash[num].SetActive(true);
        // 일정 시간(듀레이션) 동안 기다린다.
        yield return new WaitForSeconds(duration);
        // 활성화된 오브젝트를 다시 비활성화시킨다.
        eff_Flash[num].SetActive(false);
    }
}
