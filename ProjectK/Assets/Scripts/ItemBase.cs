using System;


public enum ItemMainType
{
    None,
    AttachMent,
    Expendables
}

public enum ItemSubType
{
    None,
    Bullet,
    Muzzle,
    Rail,
    Magazine,
    Recovery,
    Stamina,
    Deploy,
    Throw
}

public enum ItemUseType
{
    None,
    Equipt,
    Recover,
    Stamina,
    Deploy,
    Throw
}

public enum Stat
{
    None,
    Damage,
    Rps,
    Hp,
    Stamina,
    Focus,
    ReloadTime,
    AmmoSize
}

[Serializable]
public class ItemBase
{
    public int id;
    public ItemMainType itemType;
    public ItemSubType subType;
    public ItemUseType useType;
    public string name;
    public Stat stat;
    public int power;
    public bool isBuff;
    public float lifeTime;
    public float damageRange;
    public float throwMaxReach;
    public int amount;

    public ItemBase()
    {

    }

    //마스터 데이터 생성용
    public ItemBase(int inId, ItemMainType inItemType, ItemSubType inSubType, ItemUseType inUseType, string inName, Stat inStat, int inPower, bool inIsBuff = false, float inLifeTime = 0f, float inDamageRange = 0f, float inThrowMaxReach = 0f)
    {
        id = inId;
        itemType = inItemType;
        subType = inSubType;
        useType = inUseType;
        name = inName;
        stat = inStat;
        power = inPower;
        isBuff = inIsBuff;
        lifeTime = inLifeTime;
        damageRange = inDamageRange;
        throwMaxReach = inThrowMaxReach;
        amount = 0;
    }

    //깊은복사
    public ItemBase(ItemBase inOrigin)
    {
        id = inOrigin.id;
        itemType = inOrigin.itemType;
        subType = inOrigin.subType;
        useType = inOrigin.useType;
        name = inOrigin.name;
        stat = inOrigin.stat; 
        power = inOrigin.power;
        isBuff = inOrigin.isBuff;
        lifeTime = inOrigin.lifeTime;
        damageRange = inOrigin.damageRange;
        throwMaxReach = inOrigin.throwMaxReach;
        amount = inOrigin.amount;
    }

    //깊은복사, 수량 별도
    public ItemBase(ItemBase inOrigin, int inAmount):this(inOrigin)
    {
        amount = inAmount;
    }

    public void AddItem(int inAddAmount = 1)
    {
        amount += inAddAmount;
    }

    public void ReduceItem(int inRemoveAmount = 1)
    {
        amount -= inRemoveAmount;
    }

    public bool IsSameItem(ItemBase inItem)
    {
        if (id != inItem.id)
        {
            return false;
        }
        return true;
    }

}
