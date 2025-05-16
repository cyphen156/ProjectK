using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class MasterDataManager :MonoBehaviour
{
    public static MasterDataManager Instance;
    private Dictionary<int, Item> masterItemDictionary;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            MakeMasterData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void MakeMasterData()
    {
        masterItemDictionary = new();
        Item compensator = new Item(1121, ItemType.부착물, SubType.총구, UseType.장착, "보정기", Stat.탄퍼짐, 30);
        masterItemDictionary.Add(compensator.id, compensator);
        Item treeBox = new Item(2472, ItemType.소모품, SubType.설치, UseType.설치, "나무상자", Stat.Hp, 100);
        masterItemDictionary.Add(treeBox.id, treeBox);
    }

    public Dictionary<int, Item> GetMasterDataDic()
    {
        return masterItemDictionary;
    }

    public Item GetMasterItemData(int inId)
    {
        return masterItemDictionary[inId];
        List<(int, Item)> ff = new();

    }
}

class HaveItem
{
    Item item;
    int count; 
}

