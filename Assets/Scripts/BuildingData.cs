using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData
{
    // ���� ü��
    private int currentHp;


    public BuildingData(int _HP) // ������
    {

        currentHp = _HP;
    }
    public void decreaseHP(int damage) // damage��ŭ ü���� ��´�
    {
        currentHp -= damage;

    }

    public int getHP()
    {
        return currentHp;
    }

}