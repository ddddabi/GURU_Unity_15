using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ���� ���� ���
    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }
    // ���� ���� ����
    public GameState gState;

    // UI �ؽ�Ʈ ����
    public Text stateLable;
    // UI �ؽ�Ʈ ����
    public Text state;

    // �÷��̾� ���� ������Ʈ ����
    GameObject player;
    GameObject target;
    GameObject building;

    // �÷��̾� ���� ������Ʈ ����
    PlayerMove playerM;
    TargetFSM tt;
    Building bb;

    // �̱���
    public static GameManager gm;

    // �ɼ� �޴� UI ������Ʈ
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
        // �ʱ� ���� ���´� �غ� ���·� �����Ѵ�.
        gState = GameState.Ready;

        // ���� ���� �ڷ�ƾ �Լ��� �����Ѵ�.
        StartCoroutine(GameStart());

        // �÷��̾� ������Ʈ�� �˻�
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
        // Stage 2 ��� ���� 
        stateLable.text = "MISSION";
        // ���� ������ ��Ȳ��
        stateLable.color = new Color32(233, 182, 12, 255);
        // 2�ʰ� ����Ѵ�
        yield return new WaitForSeconds(2.0f);
        // �ǹ��� �����϶� ��� ����
        stateLable.text = "Ÿ�Ϸκ��� �����ϰ� �ǹ��� �����Ͻÿ�";
        // 2�ʰ� ���
        yield return new WaitForSeconds(3.0f);
        // ���� �����
        stateLable.text = "";
        // ������ ���¸� �غ� ���¿��� ���� ���·� ��ȯ�Ѵ�.
        gState = GameState.Run;
    }
    
    void Update()
    {
        // ����, �÷��̾��� hp�� 0���� �������ų�, +++++ !!! �ð��� �����ȴٸ�....
        if (playerM.hp <=0 || TimeAttack.done<=0)
        {
            // ���� ���� ������ ����Ѵ�.
            stateLable.text = "Game Over";

            // ���� ���� ������ ������ ���������� �����Ѵ�.
            stateLable.color = new Color32(255, 0, 0, 255);

            // ���� ���¸� ���� ���� ���·� ��ȯ�Ѵ�.
            gState = GameState.GameOver;
        }
        /*
        else if(tt.TargetScorePlus() ==  3 && bb.ScorePlus() == 4)
        {
            // Stage 2 ��� ���� 
            state.text = "�̼� Ŭ����";
            // ���� ������ ��Ȳ��
            state.color = new Color32(255, 0, 0, 255);
            // ���� ���¸� ���� ���� ���·� ��ȯ�Ѵ�.
            gState = GameState.GameOver;

        }
        */

    }
      

    // �ɼ� �޴� �ѱ�
    public void OpenOptionWindow()
    {
        // ���� ���¸� pause�� �����Ѵ�.
        gState = GameState.Pause;
        // �ð��� �����.
        Time.timeScale = 0;
        // �ɼ� �޴� â�� Ȱ��ȭ�Ѵ�.
        optionUI.SetActive(true);
    }

    // �ɼ� �޴� ����(����ϱ�)
    
    public void CloseOptionWindow()
    {
        // ���� ���¸� run ���·� �����Ѵ�.
        gState = GameState.Run;
        // �ð��� 1��� �ǵ�����.
        Time.timeScale = 1.0f;
        // �ɼ� �޴�â�� ��Ȱ��ȭ�Ѵ�.
        optionUI.SetActive(false);
    }

    // ���� ������ϱ�(���� �� �ٽ� �ε�)
    public void GameRestart()
    {
        // �ð��� 1��� �ǵ�����.
        Time.timeScale = 1.0f;
        // ���� ���� �ٽ� �ε��Ѵ�.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ������ �����ϱ�
    public void GameQuit()
    {
        // ���ø����̼��� �����Ѵ�.
        Application.Quit();
    }
}
