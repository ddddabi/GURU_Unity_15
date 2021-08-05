using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 게임 상태 상수
    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }
    // 게임 상태 변수
    public GameState gState;

    // UI 텍스트 변수
    public Text stateLable;
    // UI 텍스트 변수
    public Text state;

    // 플레이어 게임 오브젝트 변수
    GameObject player;
    GameObject target;
    GameObject building;

    // 플레이어 무브 컴포넌트 변수
    PlayerMove playerM;
    TargetFSM tt;
    Building bb;

    // 싱글턴
    public static GameManager gm;

    // 옵션 메뉴 UI 오브젝트
    public GameObject optionUI;
    static int lastScore = 0;

    private void Awake()
    {
        if(gm == null)
        {
            gm = this;
        }
    }

    void Start()
    {
        // 초기 게임 상태는 준비 상태로 설정한다.
        gState = GameState.Ready;

        // 게임 시작 코루틴 함수를 실행한다.
        StartCoroutine(GameStart());

        // 플레이어 오브젝트를 검색
        player = GameObject.Find("Player");
        target = GameObject.Find("Target");
        building = GameObject.Find("Building");

        playerM = player.GetComponent<PlayerMove>();
        tt = target.GetComponent<TargetFSM>();
        bb = building.GetComponent<Building>();

       

        //timetattack = new TimeAttack(done);
    }

    IEnumerator GameStart()
    {
        // Stage 2 라는 문구 
        stateLable.text = "MISSION";
        // 문구 색상은 주황색
        stateLable.color = new Color32(233, 182, 12, 255);
        // 2초간 대기한다
        yield return new WaitForSeconds(2.0f);
        // 건물을 폭파하라 라는 문구
        stateLable.text = "타켓로봇을 제거하고 건물을 폭파하시오";
        // 2초간 대기
        yield return new WaitForSeconds(3.0f);
        // 문구 지우기
        stateLable.text = "";
        // 게임의 상태를 준비 상태에서 실행 상태로 전환한다.
        gState = GameState.Run;
    }
    
    void Update()
    {
        // 만일, 플레이어의 hp가 0으로 떨어지거나, +++++ !!! 시간이 소진된다면....
        if (playerM.hp <=0 || TimeAttack.done<=0)
        {
            // 게임 오버 문구를 출력한다.
            stateLable.text = "Game Over";

            // 게임 오버 문구의 색상은 붉은색으로 설정한다.
            stateLable.color = new Color32(255, 0, 0, 255);

            // 게임 상태를 게임 오버 상태로 전환한다.
            gState = GameState.GameOver;
        }
        /*
        else if(tt.TargetScorePlus() ==  3 && bb.ScorePlus() == 4)
        {
            // Stage 2 라는 문구 
            state.text = "미션 클리어";
            // 문구 색상은 주황색
            state.color = new Color32(255, 0, 0, 255);
            // 게임 상태를 게임 오버 상태로 전환한다.
            gState = GameState.GameOver;

        }
        */

    }
      

    // 옵션 메뉴 켜기
    public void OpenOptionWindow()
    {
        // 게임 상태를 pause로 변경한다.
        gState = GameState.Pause;
        // 시간을 멈춘다.
        Time.timeScale = 0;
        // 옵션 메뉴 창을 활성화한다.
        optionUI.SetActive(true);
    }

    // 옵션 메뉴 끄기(계속하기)
    
    public void CloseOptionWindow()
    {
        // 게임 상태를 run 상태로 변경한다.
        gState = GameState.Run;
        // 시간을 1배로 되돌린다.
        Time.timeScale = 1.0f;
        // 옵션 메뉴창을 비활성화한다.
        optionUI.SetActive(false);
    }

    // 게임 재시작하기(현재 씬 다시 로드)
    public void GameRestart()
    {
        // 시간을 1배로 되돌린다.
        Time.timeScale = 1.0f;
        // 현재 씬을 다시 로드한다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임을 종료하기
    public void GameQuit()
    {
        // 어플리케이션을 종료한다.
        Application.Quit();
    }
}
