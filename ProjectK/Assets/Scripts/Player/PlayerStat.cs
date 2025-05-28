using System;
using UnityEngine;

public class PlayerStat
{
    private const float maxHp = 100;
    private float hp;
    private float maxStemina = 100;
    private float stamina;

    public PlayerStat()
    {
        hp = maxHp;

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

        if(hp > maxHp)
        {
            hp = maxHp;
        }
        else if(hp < 0)
        {
            hp = 0;
        }
    }

    public void ApplyStamina(float inApplyStaminaValue)
    {
        stamina += Mathf.Clamp(inApplyStaminaValue, 0, maxStemina);
    }
}
