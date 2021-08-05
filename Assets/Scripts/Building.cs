using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{

    private BuildingData buildingData;
    //private GameManager gm;

    // 현재 체력 변수
    int currentHp = 50;

    // 최대 체력 변수
    public int maxHp = 50;

    // 슬라이더 변수
    public Slider hpSlider2;

    // 건물 파편 변수
    public GameObject obj;

    // 건물 점수
    static int buildingScore = 0;
    static int lastScore = 0;

    // UI 텍스트 변수
    public Text state;

    // 폭발 효과 프리팹 변수
    public GameObject explosion;

    void Start()
    {
        buildingData = new BuildingData(currentHp);

        print(gameObject.name + "의 체력 : " + buildingData.getHP());

    }
    void Update()
    {
        if (buildingData.getHP() <= 0)
        {
            print("파괴!!!!!");
            Destroy(gameObject);
            Instantiate(obj, transform.position, Quaternion.identity);
            GameObject go = Instantiate(explosion);
            go.transform.position = transform.position;
            ScorePlus();
            buildingScoreCount();
            

        }
        // hp 슬라이더의 값에 체력 비율을 적용한다
        hpSlider2.value = (float)buildingData.getHP() / maxHp;
    }

    public int ScorePlus()
    {
        buildingScore++;
        print("빌딩 점수 :"+buildingScore);
        
        return buildingScore;
    }
    
    public void buildingScoreCount()
    {
        if(buildingScore == 4)
        {
            // Stage 2 라는 문구 
            state.text = "미션 클리어";
            // 문구 색상은 주황색
            state.color = new Color32(255, 0, 0, 255);
            // 게임 상태를 게임 오버 상태로 전환한다.
            //gState = GameState.GameOver;
        }
        
    }
   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Granade")
        {
            print("폭탄 충돌");
            buildingData.decreaseHP(10);
            buildingData.getHP();
            print(gameObject.name + "의 현재 체력 : " + buildingData.getHP());

        }
    }

    
}
