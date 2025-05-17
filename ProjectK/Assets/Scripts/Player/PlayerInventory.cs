using System;
using System.Collections.Generic;
using UnityEngine;



public class PlayerInventory : MonoBehaviour
{
    [SerializeField] 
    private ItemBase[] gunItems;
    [SerializeField] 
    private ItemBase[] consumeItems;

    private ItemSubType[] gunFixType;
    private ItemSubType[] consumeFixType;

    const int NO_INDEX_VALUE = -1;

    private void Awake()
    {
        //총 인벤토리에서 고정위치
        gunFixType = new ItemSubType[]{ItemSubType.Bullet, ItemSubType.Muzzle, ItemSubType.Rail, ItemSubType.Magazine };
        gunItems = new ItemBase[gunFixType.Length];
        //소비 인벤토리에서 고정위치 
        consumeFixType = new ItemSubType[] { ItemSubType.Recovery, ItemSubType.Recovery, ItemSubType.Stamina, ItemSubType.Deploy, ItemSubType.Throw, ItemSubType.Throw };
        consumeItems = new ItemBase[consumeFixType.Length];
    }

    private void Update()
    {
        //테스트
        if (Input.GetKeyDown(KeyCode.Y))
        {
            MasterDataManager.Instance.GetMasterDataDic();
            foreach (KeyValuePair<int, ItemBase> pair in MasterDataManager.Instance.GetMasterDataDic())
            {
                ItemBase copy = new ItemBase(pair.Value);
                ItemBase newItem = new ItemBase(copy, 1);
                PickItem(newItem);
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            MasterDataManager.Instance.GetMasterDataDic();
            foreach (KeyValuePair<int, ItemBase> pair in MasterDataManager.Instance.GetMasterDataDic())
            {
                ItemBase copy = new ItemBase(pair.Value);
                ItemBase newItem = new ItemBase(copy, 1);
                newItem.id = 10;
                PickItem(newItem);
            }
        }
    }

    public ItemBase PickItem(ItemBase inItem)
    {
        ItemBase returnItem = null;

        ItemMainType itemType = inItem.itemType;
        if(itemType == ItemMainType.AttachMent)
        {
            //같은 아이템 차고 있으면 들어온 아이템을 장착하고 기존 아이템 반환
            if(EquiptItem(inItem, out returnItem) == false)
            {
                Debug.LogError("아이템Data의 SubType이 존재하지 않습니다.");
                return returnItem;
            }
        }
        else if(itemType == ItemMainType.Expendables)
        {
            if(AquireConsumeItem(inItem, out returnItem) == false){
                Debug.LogError("아이템Data의 SubType이 존재하지 않습니다.");
                return returnItem;
            }
        }

        return returnItem;
    }

    public void UseItem(int inConsumeSlot)
    {
        //슬롯 번호가 들어오면 해당 슬롯의 아이템을 사용
    }

    private bool EquiptItem(ItemBase inEquipItem, out ItemBase returnItem)
    {
        //장비타입 슬롯 인덱스 찾기
        ItemSubType subType = inEquipItem.subType;
        int slotIdx = FindSlotIndex(gunFixType, subType);
        if(slotIdx == NO_INDEX_VALUE)
        {
            returnItem = null;
            return false;
        }

        returnItem = gunItems[slotIdx]; //착용중이던 아이템
        gunItems[slotIdx] = inEquipItem; //새로 장착

        return true;
    }

    private bool AquireConsumeItem(ItemBase inConsumeItem, out ItemBase returnItem)
    {
        //장비타입 슬롯 인덱스 찾기
        ItemSubType subType = inConsumeItem.subType;
        int slotIdx = FindSlotIndex(consumeFixType, subType);
        if (slotIdx == NO_INDEX_VALUE)
        {
            returnItem = null;
            return false;
        }
        
        //각 서브타입에 따라 획득 방식 적용
        if(subType == ItemSubType.Recovery || subType == ItemSubType.Throw)
        {
            //해당 타입 1번 슬롯이 비어있거나 같은경우
            if (AddConsumeItem(slotIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return true;
            }

            int nextIdx = slotIdx + 1;
            //해당 타입 2번 슬롯이 비어있거나 같은경우
            if (AddConsumeItem(nextIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return true;
            }

            //두 슬롯 다 다른 아이템이 있는경우
            returnItem = ChangeConsumeItem(nextIdx, inConsumeItem);
            return true;
        }
        else if(subType == ItemSubType.Stamina || subType == ItemSubType.Deploy)
        {
            //빈 슬롯이거나 같은 아이템인 경우 추가
            if(AddConsumeItem(slotIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return true;
            }

            //다른 아이템이 있는 경우 교환
            returnItem = ChangeConsumeItem(slotIdx, inConsumeItem);
            return true;
        }
 
        returnItem = null;
        return true;
    }

    private int FindSlotIndex(ItemSubType[] inSlotType, ItemSubType inSubType)
    {
        for (int i = 0; i < inSlotType.Length; i++)
        {
            if (inSlotType[i] == inSubType)
            {
                return i;
            }
        }
        return NO_INDEX_VALUE;
    }

    private bool AddConsumeItem(int inSlotIdx, ItemBase inConsumeItem)
    {
        //해당 슬롯에 새로 넣거나 추가 되는경우

        if (consumeItems[inSlotIdx] == null)
        {
            consumeItems[inSlotIdx] = inConsumeItem; //그 물건 수량 그대로 적립
            return true;
        }
        //투척 1번 슬롯에 아이템이 같은게 있으면 수량 추가
        if (consumeItems[inSlotIdx].IsSameItem(inConsumeItem))
        {
            consumeItems[inSlotIdx].AddItem(inConsumeItem.amount);
            return true;
        }

        return false;
    }

    private ItemBase ChangeConsumeItem(int inSlotIdx, ItemBase inConsumeItem)
    {
        ItemBase returnItem = consumeItems[inSlotIdx]; //기존 아이템은 반환하고
        consumeItems[inSlotIdx] = inConsumeItem; //새로 획득하는 아이템을 추가
        return returnItem;
    }
}
