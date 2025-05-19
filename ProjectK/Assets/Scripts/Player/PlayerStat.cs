using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    private float maxHp;
    [SerializeField] private float hp;
    private float maxStemina;
    [SerializeField] private float stamina;

    private void Awake()
    {
        maxHp = 100;
        hp = maxHp;

        maxStemina = 100;
        stamina = maxStemina;
    }

    public float GetHP()
    {
        return hp;
    }

    public float GetStamina()
    {
        return stamina;
    }

    public void ApplyHp(float inApplyHpValue)
    {
        hp += inApplyHpValue;

        if(CheckDie() == true)
        {
            return;
        }

        Logger.Info("남은 HP: " + hp);
    }

    public bool CheckDie()
    {
        if (hp <= 0)
        {
            Logger.Info("플레이어가 죽음");
            return true;
        }

        return false;
    }
}
