
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
    public static event Action OnCloseBox;
    public static event Action<DropBox> OnChangeBox;
    private Func<ItemBase, ItemBase> itemPickCallBack;

    private void Awake()
    {
        haveItems = new();
        startChance = 100;
        stackRate = 0.8f;
        maxRollCount = 5;
    }

    private void Start()
    {
        if (IsHost)
        {
            DiceItem();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5) && IsHost)
        {
            DiceItem();
        }
    }

    private void DiceItem()
    {
        haveItems.Clear();
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

    [Rpc(SendTo.Everyone)]
    private void AddDroxBoxItemRpc(int inItemId, int inItemAmount)
    {
            Debug.Log(MasterDataManager.Instance.GetMasterItemData(inItemId).name + "을 뽑았다.");
    }

    public void OpenBox(Func<ItemBase, ItemBase> inItemPickCallBack)
    {
        //Debug.Log("박스를 열었다.");
        OnOpenBox.Invoke(this);
         itemPickCallBack = inItemPickCallBack;
    }

    public void CloseBox()
    {
        //Debug.Log("박스를 닫았다.");
        OnCloseBox.Invoke();
        itemPickCallBack = null;
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

        ItemBase returnItem = null;
        if(itemPickCallBack != null)
        {
            returnItem = itemPickCallBack(haveItems[inSlotIndex]);
        }

        //Debug.Log( "반환된거 " + returnItem.itemType + " " +returnItem.name);
        if (returnItem == null || returnItem.itemType == ItemMainType.None)
        {
            haveItems.RemoveAt(inSlotIndex);
        }
        else
        {
            haveItems[inSlotIndex] = returnItem;
        }

        OnChangeBox.Invoke(this);
    }

}

