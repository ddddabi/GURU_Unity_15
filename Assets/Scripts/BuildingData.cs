using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData
{
    // 현재 체력
    private int currentHp;


    public BuildingData(int _HP) // 생성자
    {

        currentHp = _HP;
    }
    public void decreaseHP(int damage) // damage만큼 체력을 깎는다
    {
        currentHp -= damage;

    }

    public int getHP()
    {
        return currentHp;
    }

}