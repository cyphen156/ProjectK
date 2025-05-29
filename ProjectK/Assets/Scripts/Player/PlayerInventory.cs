using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] 
    private ItemBase[] gunItems;
    [SerializeField] 
    private ItemBase[] consumeItems;

    public static event Action<ItemBase[]> OnChangeGunItems;
    public static event Action<ItemBase[]> OnChangeConsumeItems;
    public static readonly ItemSubType[] GunSlotInputType = { ItemSubType.Bullet, ItemSubType.Muzzle, ItemSubType.Rail, ItemSubType.Magazine };
    public static readonly ItemSubType[] ConsumeSlotInputType = { ItemSubType.Recovery, ItemSubType.Recovery, ItemSubType.Stamina, ItemSubType.Deploy, ItemSubType.Throw, ItemSubType.Throw };

    const int NO_INDEX_VALUE = -1;

    private void Awake()
    {
        //총 인벤토리에서 고정위치
        gunItems = new ItemBase[GunSlotInputType.Length];

        //소비 인벤토리에서 고정위치 
        consumeItems = new ItemBase[ConsumeSlotInputType.Length];
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
                TryAddOrReturnPreviousItem(newItem, null);
            }
        }
    }
    public ItemBase TryAddOrReturnPreviousItem(ItemBase inItem, Gun inPlayerGun)
    {
        ItemBase returnItem = null;

        ItemMainType itemType = inItem.itemType;
        if(itemType == ItemMainType.AttachMent)
        {
            //같은 아이템 차고 있으면 들어온 아이템을 장착하고 기존 아이템 반환
            EquiptItem(inItem, out returnItem);
            inPlayerGun?.EquiptItems(gunItems);
            OnGunItemChange(); 
        }
        else if(itemType == ItemMainType.Expendables)
        {
            AquireConsumeItem(inItem, out returnItem);
            OnConsumeItemChange();
        }

        return returnItem;
    }

    private void OnGunItemChange()
    {
        if (IsOwner)
        {
            OnChangeGunItems?.Invoke(gunItems);
        }
    }

    private void OnConsumeItemChange()
    {
        if (IsOwner)
        {
            OnChangeConsumeItems?.Invoke(consumeItems);
        }
    }

    #region 습득
    private void EquiptItem(ItemBase inEquipItem, out ItemBase returnItem)
    {
        //장비타입 슬롯 인덱스 찾기
        ItemSubType subType = inEquipItem.subType;
        int slotIdx = FindSlotIndex(GunSlotInputType, subType);
        if (slotIdx == NO_INDEX_VALUE)
        {
            returnItem = null;
            return;
        }

        returnItem = gunItems[slotIdx]; //착용중이던 아이템
        gunItems[slotIdx] = inEquipItem; //새로 장착
    }

    private void AquireConsumeItem(ItemBase inConsumeItem, out ItemBase returnItem)
    {
        //장비타입 슬롯 인덱스 찾기
        ItemSubType subType = inConsumeItem.subType;
        int slotIdx = FindSlotIndex(ConsumeSlotInputType, subType);
        if (slotIdx == NO_INDEX_VALUE)
        {
            returnItem = null;
            return;
        }

        //각 서브타입에 따라 획득 방식 적용
        if (subType == ItemSubType.Recovery || subType == ItemSubType.Throw)
        {
            //해당 타입 1번 슬롯이 비어있거나 같은경우
            if (AddConsumeItem(slotIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return;
            }

            int nextIdx = slotIdx + 1;
            //해당 타입 2번 슬롯이 비어있거나 같은경우
            if (AddConsumeItem(nextIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return;
            }

            //두 슬롯 다 다른 아이템이 있는경우
            returnItem = ChangeConsumeItem(nextIdx, inConsumeItem);
            return;
        }
        else if (subType == ItemSubType.Stamina || subType == ItemSubType.Deploy)
        {
            //빈 슬롯이거나 같은 아이템인 경우 추가
            if (AddConsumeItem(slotIdx, inConsumeItem) == true)
            {
                returnItem = null;
                return;
            }

            //다른 아이템이 있는 경우 교환
            returnItem = ChangeConsumeItem(slotIdx, inConsumeItem);
            return;
        }

        returnItem = null;
        return;
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

        if (consumeItems[inSlotIdx] == null || consumeItems[inSlotIdx].itemType == ItemMainType.None)
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
    #endregion

    #region 소모
    public ItemBase GetItem(int inConsumeSlot)
    {
        return consumeItems[inConsumeSlot-1];
    }

    // 여기 작업 필요
    public bool HasItem(int inConsumeSlot)
    {
        int slotIdx = inConsumeSlot - 1;
        if(consumeItems[slotIdx] ==null || consumeItems[slotIdx].itemType == ItemMainType.None)
        {
            return false;
        }

        if (consumeItems[slotIdx].amount == 0)
        {
            return false;
        }
        return true;
    }

    public void UseItem(int inConsumeSlot, int inAmount = 1)
    {
        //사용한 슬롯 아이템 수량 감소
        UseItemRpc(inConsumeSlot, inAmount); //수량 동기화
    }

    [Rpc(SendTo.Everyone)]
    public void UseItemRpc(int inInputNumber, int amount)
    {
        int slotIdx = inInputNumber - 1;
        consumeItems[slotIdx].ReduceItem(1);
        if (consumeItems[slotIdx].amount == 0)
        {
            consumeItems[slotIdx] = null;
        }
        OnConsumeItemChange();
    }

    #endregion

}
