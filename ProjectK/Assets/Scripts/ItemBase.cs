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
    public ItemBase(string[] intParseData)
    {
        int pidIdx = 0;
        int mainTypeIdx = pidIdx + 1;
        int nameIdx = mainTypeIdx + 1;
        int useTypeIdx = nameIdx + 1;
        int subTypeIdx = useTypeIdx + 1;
        int statIdx = subTypeIdx + 1;
        int powerIdx = statIdx + 1;
        int isBuffIdx = powerIdx + 1;
        int lifeTimeIdx = isBuffIdx + 1;
        int damageRangeIdx = lifeTimeIdx + 1;
        int throwIdx = damageRangeIdx + 1;

        id = int.Parse(intParseData[pidIdx]);
        itemType = (ItemMainType)Enum.Parse(typeof(ItemMainType), intParseData[mainTypeIdx]);
        name = intParseData[nameIdx];
        useType = ParseEnum<ItemUseType>(intParseData[useTypeIdx]);
        subType = ParseEnum<ItemSubType>(intParseData[subTypeIdx]);
        stat = ParseEnum<Stat>(intParseData[statIdx]);
        power = int.Parse(intParseData[powerIdx]);
        isBuff = (int.Parse(intParseData[isBuffIdx]) != 0);
        lifeTime = float.Parse(intParseData[lifeTimeIdx]);
        damageRange = float.Parse(intParseData[damageRangeIdx]);
        throwMaxReach = float.Parse(intParseData[throwIdx]);
    }

    private T ParseEnum<T>(string inEnumStr) where T : Enum
    {
        T parseEnum = (T)Enum.Parse(typeof(T), inEnumStr);
        return parseEnum;
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
