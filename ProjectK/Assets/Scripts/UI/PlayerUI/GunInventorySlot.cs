using UnityEngine;
using UnityEngine.UI;

public class GunInventorySlot : MonoBehaviour
{
    [SerializeField] private ItemBase slotItem;
    [SerializeField] private Image gunIcon;

    private void Start()
    {
        gunIcon.gameObject.SetActive(false);
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
        gunIcon.gameObject.SetActive(false);
    }

    private void SetInfo()
    {
        if (slotItem == null)
        {
            return;
        }
        gunIcon.gameObject.SetActive(true);
    }
}
