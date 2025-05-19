
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class MasterDataManager :MonoBehaviour
{
    public static MasterDataManager Instance;
    public DropBoxTable DropBox;
    private Dictionary<int, ItemBase> masterItemDictionary;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            MakeMasterData();
            DropBox = new();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void MakeMasterData()
    {
        masterItemDictionary = new();
        string[] lines = File.ReadAllLines("Assets/fileData.csv");
        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            if (values[0][0] == '#')
            {
                continue;
            }
          
            ItemBase parseItem = new ItemBase(values);
            //Debug.Log($"파싱한데이터 {parseItem.name} _ {parseItem.subType} _ {parseItem.stat}");
            masterItemDictionary.Add(parseItem.id, parseItem);
        }
    }

    public Dictionary<int, ItemBase> GetMasterDataDic()
    {
        return masterItemDictionary;
    }

    public ItemBase GetMasterItemData(int inItemID)
    {
        return masterItemDictionary[inItemID];
    }
}

