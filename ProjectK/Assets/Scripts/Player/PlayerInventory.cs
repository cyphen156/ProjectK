using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;


public class PlayerInventory : MonoBehaviour
{
    public (int, int)[] gunItems;
    public (int, int)[] consumeItems;

    public SubType[] gunFixType;
    public SubType[] consumeFixType;

    private void Awake()
    {
        gunFixType = new SubType[]{SubType.총알, SubType.총구, SubType.레일, SubType.탄창 };
        consumeFixType = new SubType[] { SubType.회복, SubType.회복, SubType.스테미너, SubType.설치, SubType.투척 };
    }

    public Item PickItem(Item inItem)
    {
        Item returnItem = null;

        ItemType itemType = inItem.itemType;
        if(itemType == ItemType.부착물)
        {
            //같은 아이템 차고 있으면 들어온 아이템을 장착하고 기존 아이템 반환
        }
        else if(itemType == ItemType.소모품)
        {
            //
        }


        return returnItem;
    }
}
