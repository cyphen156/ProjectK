using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Profiling;


public enum ItemType
{
    None,
    부착물,
    소모품
}

public enum SubType
{
    None,
    총알,
    총구,
    레일,
    탄창,
    회복,
    스테미너,
    설치,
    투척
}

public enum UseType
{
    None,
    장착,
    회복,
    스테미너,
    설치,
    투척
}

public enum Stat
{
    None,
    Damage,
    Rps,
    Hp,
    Stemina,
    탄퍼짐,
    ReloadTime,
    AmmoSize
}

public class Item
{
    public int id;
    public ItemType itemType;
    public SubType subType;
    public UseType useType;
    public string name;
    public Stat stat;
    public int power;
    public bool isBuff;
    public float lifeTime;
    public float damageRange;
    public float throwMaxReach;

    public Item(int inId, ItemType inItemType, SubType inSubType, UseType inUseType, string inName, Stat inStat, int inPower, bool inIsBuff = false, float inLifeTime = 0f, float inDamageRange = 0f, float inThrowMaxReach = 0f)
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
    }

    //깊은복사
    public Item(Item inOrigin)
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
    }

}
