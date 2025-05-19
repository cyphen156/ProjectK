using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerController playerController;
    public PlayerStat playerStat;
    [SerializeField] private Transform crosshairTransform;
    [SerializeField] private DropBoxSlot[] DropboxSlots;
    [SerializeField] private Button[] DropboxSlotButtons;
    
    private Slider hpSlider;
    private Slider staminaSlider;
    private GameObject dropBoxPanelObj;
    private DropBox openedDropBox;
    private bool isOnDropBoxPanel;

    private void Awake()
    {
        isOnDropBoxPanel = false;
        openedDropBox = null;
        DropboxSlots = GetComponentsInChildren<DropBoxSlot>();
    }

    private void OnEnable()
    {
        InputManager.OnLocalPlayerRegistered += SetPlayerController;
    }

    private void OnDisable()
    {
        InputManager.OnLocalPlayerRegistered -= SetPlayerController;
    }

    private void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }
    private void Start()
    {
        //DropBox.OnOpenBox += OnOpenDropBox;
        DropBox.OnCloseBox += OnCloseDropBox;
        DropBox.OnChangeBox += OnChangeDropBox;

        crosshairTransform = transform.Find("Crosshair");

        StartSettingHUDUI();

        StartSettingDropBoxUI();
    }

    private void Update()
    {
        crosshairTransform.position = Camera.main.WorldToScreenPoint(playerController.mouseWorldPosition);
    }

    private void StartSettingHUDUI()
    {
        hpSlider = transform.Find("PlayerHUDPanel/HpSlider").GetComponent<Slider>();
        staminaSlider = transform.Find("PlayerHUDPanel/StaminaSlider").GetComponent<Slider>();

        // hp바, 스테미나바 초기화
        hpSlider.maxValue = 100f;
        staminaSlider.maxValue = 100f;
        if (playerStat != null)
        {
            hpSlider.value = playerStat.GetHP();
            staminaSlider.value = playerStat.GetStamina();
        }
        else
        {
            Debug.LogError("playerStat을 변수에 넣었는지 인스펙터에서 확인 필요!");
        }
    }

    private void StartSettingDropBoxUI()
    {
        // 버튼에 리스너 등록
        for (int i = 0; i < DropboxSlotButtons.Length; i++)
        {
            int index = i;
            Button button = DropboxSlotButtons[i];
            button.onClick.AddListener(delegate {
                DropBoxSlotClick(index, button);
            });
        }

        dropBoxPanelObj = transform.Find("DropBoxPanel").gameObject;
        dropBoxPanelObj.SetActive(false);
    }

    private void OnOpenDropBox(DropBox inDropBox)
    {
        openedDropBox = inDropBox;
        dropBoxPanelObj.SetActive(true);
        SetDropBoxUI();
    }

    private void OnChangeDropBox(DropBox inChangedBox)
    {
        if(openedDropBox == false)
        {
            return;
        }
       
        SetDropBoxUI();
    }

    private void OnCloseDropBox()
    {
        openedDropBox = null;
        dropBoxPanelObj.SetActive(false);
    }

    private void SetDropBoxUI()
    {
        List<ItemBase> haveItemList = openedDropBox.GetBoxItemList();
        for (int i = 0; i < haveItemList.Count; i++)
        {
            DropboxSlots[i].SetSlot(haveItemList[i]);
        }
        for (int i = haveItemList.Count; i < DropboxSlots.Length; i++)
        {
            DropboxSlots[i].Reset();
        }
    }

    private void DropBoxSlotClick(int inIndex, Button inDropBoxIndex)
    {
        Debug.Log($"[{inIndex}]번 슬롯 클릭됨");
        Debug.Log("클릭한 오브젝트 이름: " + inDropBoxIndex.gameObject.name);
        openedDropBox.SelectItem(inIndex);
    }
}
