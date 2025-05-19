using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    private float maxHp;
    [SerializeField] private float hp;
    private float maxStemina;
    [SerializeField] private float stamina;

    public static event Action OnDie;

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

        if(hp <= 0)
        {
            OnDie?.Invoke();
            return;
        }

        Logger.Info("³²Àº HP: " + hp);
    }
}
