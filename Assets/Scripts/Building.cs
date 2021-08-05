using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{

    private BuildingData buildingData;
    //private GameManager gm;

    // ���� ü�� ����
    int currentHp = 50;

    // �ִ� ü�� ����
    public int maxHp = 50;

    // �����̴� ����
    public Slider hpSlider2;

    // �ǹ� ���� ����
    public GameObject obj;

    // �ǹ� ����
    static int buildingScore = 0;
    static int lastScore = 0;

    // UI �ؽ�Ʈ ����
    public Text state;

    // ���� ȿ�� ������ ����
    public GameObject explosion;

    void Start()
    {
        buildingData = new BuildingData(currentHp);

        print(gameObject.name + "�� ü�� : " + buildingData.getHP());

    }
    void Update()
    {
        if (buildingData.getHP() <= 0)
        {
            print("�ı�!!!!!");
            Destroy(gameObject);
            Instantiate(obj, transform.position, Quaternion.identity);
            GameObject go = Instantiate(explosion);
            go.transform.position = transform.position;
            ScorePlus();
            buildingScoreCount();
            

        }
        // hp �����̴��� ���� ü�� ������ �����Ѵ�
        hpSlider2.value = (float)buildingData.getHP() / maxHp;
    }

    public int ScorePlus()
    {
        buildingScore++;
        print("���� ���� :"+buildingScore);
        
        return buildingScore;
    }
    
    public void buildingScoreCount()
    {
        if(buildingScore == 4)
        {
            // Stage 2 ��� ���� 
            state.text = "�̼� Ŭ����";
            // ���� ������ ��Ȳ��
            state.color = new Color32(255, 0, 0, 255);
            // ���� ���¸� ���� ���� ���·� ��ȯ�Ѵ�.
            //gState = GameState.GameOver;
        }
        
    }
   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Granade")
        {
            print("��ź �浹");
            buildingData.decreaseHP(10);
            buildingData.getHP();
            print(gameObject.name + "�� ���� ü�� : " + buildingData.getHP());

        }
    }

    
}
