
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DropBox : NetworkBehaviour
{
    [SerializeField] private List<ItemBase> haveItems;
    private float startChance;
    private float stackRate; //찬스가 중첩 될수록 변하는 비율
    private int maxRollCount;
    public static event Action<DropBox> OnOpenBox;
    public static event Action<DropBox> OnCloseBox;
    public static event Action<DropBox> OnChangeBox;
    private uint openPlayerNumber;
    private const uint InValidPlayerNumber = uint.MaxValue;

    private void Awake()
    {
        haveItems = new();
        startChance = 100;
        stackRate = 0.8f;
        maxRollCount = 5;
    }

    private void Start()
    {

        DiceItem();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            DiceItem();
        }
    }

    private void DiceItem()
    {
        if (IsHost == false)
        {
            return;
        }
        ResetItemListRpc();
        startChance = 100;
        for (int i = 1; i <= maxRollCount; i++)
        {
            int rollNum = UnityEngine.Random.Range(1, 101);
            if (startChance < rollNum)
            {
                //아이템 뽑기 실패 했으면 종료
                break;
            }
            int mainTypeNum = UnityEngine.Random.Range(1, 3); //메인아이템 부착물이냐 소모품이냐 50%
            ItemMainType mainType = (ItemMainType)mainTypeNum;
            
            ItemBase rollItem = MasterDataManager.Instance.DropBox.RollDropBox(mainType);
            
            AddDroxBoxItemRpc(rollItem.id, rollItem.amount);

            //뽑았으면 확률 조정
            startChance *= stackRate;

        }
    }

    #region 내용물 동기화
    [Rpc(SendTo.Everyone)]
    private void ResetItemListRpc()
    {
        haveItems.Clear();
        OnChangeBox?.Invoke(this);
    }

    [Rpc(SendTo.Everyone)]
    private void AddDroxBoxItemRpc(int inItemId, int inItemAmount)
    {
        ItemBase addItem = new ItemBase(MasterDataManager.Instance.GetMasterItemData(inItemId), inItemAmount);
        haveItems.Add(addItem);
        OnChangeBox?.Invoke(this);
    }



    [Rpc(SendTo.Everyone)]
    private void RemoveItemRpc(int inSlotIndex)
    {
        haveItems.RemoveAt(inSlotIndex);
        OnChangeBox.Invoke(this);
    }

    [Rpc(SendTo.Everyone)]
    private void ReplaceItemRpc(int inSlotIndex, int inItemId, int inItemAmount)
    {
        ItemBase replaceItem = new ItemBase(MasterDataManager.Instance.GetMasterItemData(inItemId), inItemAmount);
        haveItems[inSlotIndex] = replaceItem;
        OnChangeBox.Invoke(this);
    }
    #endregion

    #region 상자 열고 닫기, 아이템 선택하기
    public void OpenBox(uint inPlayerNumber)
    {
        //Debug.Log("박스를 열었다.");
        OnOpenBox?.Invoke(this);
         openPlayerNumber = inPlayerNumber;
    }

    public void CloseBox()
    {
        //Debug.Log("박스를 닫았다.");
        OnCloseBox?.Invoke(this);
        openPlayerNumber = InValidPlayerNumber;
    }

    public List<ItemBase> GetBoxItemList()
    {
        return haveItems;
    }

    public void SelectItem(int inSlotIndex)
    {
        if (haveItems.Count <= inSlotIndex)
        {
            return;
        }

        ReqItemPickRpc(inSlotIndex, openPlayerNumber);
    }

    [Rpc(SendTo.Server)]
    private void ReqItemPickRpc(int inSlotIndex, uint inOpenPlayerNumber)
    {
        //Debug.Log("아이템 습득 판별은 Server에서만");
        if(inOpenPlayerNumber == InValidPlayerNumber)
        {
            Logger.Log("박스연 캐릭터가 없거나 잘못된 넘버를 가진 캐릭터");
            return;
        }

        ItemBase returnItem = null;
        PlayerController player = GameManager.Instance.GetPlayer(inOpenPlayerNumber);
        returnItem = player.PickItem((haveItems[inSlotIndex]));
       
        //Debug.Log( "반환된거 " + returnItem.itemType + " " +returnItem.name);
        if (returnItem == null || returnItem.itemType == ItemMainType.None)
        {
            RemoveItemRpc(inSlotIndex);
        }
        else
        {
            ReplaceItemRpc(inSlotIndex, returnItem.id, returnItem.amount);
        }
    }

 
    #endregion

    

}

