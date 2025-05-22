using System;

public class PlayerStat
{
    private const float maxHp = 100;
    private float hp;
    private float maxStemina;
    private float stamina;

    public PlayerStat()
    {
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

        if(hp > maxHp)
        {
            hp = maxHp;
        }
        else if(hp < 0)
        {
            hp = 0;
        }

        Logger.Info("³²Àº HP: " + hp);
    }
}
