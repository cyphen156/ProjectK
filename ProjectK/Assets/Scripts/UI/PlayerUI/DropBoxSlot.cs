using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DropBoxSlot : MonoBehaviour
{
    [SerializeField] private ItemBase slotItem;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemAmountText;


    public void SetSlot(ItemBase inItemBase)
    {
        Reset();
        slotItem = inItemBase;
        SetInfo();
    }

    public void Reset()
    {
        slotItem = null;
        itemImage.gameObject.SetActive(false);
        itemAmountText.gameObject.SetActive(false);
    }

    private void SetInfo()
    {
        if(slotItem == null)
        {
            return;
        }
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = slotItem.sprite;
        itemAmountText.gameObject.SetActive(true);
        itemAmountText.text = slotItem.amount.ToString();

    }
}

