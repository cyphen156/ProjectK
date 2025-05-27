using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumeSlot : MonoBehaviour
{
    [SerializeField] private ItemBase slotItem;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemAmountText;


    private void Start()
    {
        itemImage.gameObject.SetActive(false);
        itemAmountText.gameObject.SetActive(false);
    }

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
        if (slotItem == null || slotItem.itemType == ItemMainType.None)
        {
            return;
        }
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = slotItem.sprite;
        itemAmountText.gameObject.SetActive(true);
        itemAmountText.text = slotItem.amount.ToString();
    }
}
