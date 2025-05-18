using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private PlayerController playerController;
    public PlayerStat playerStat;
    [SerializeField] private Transform crosshairTransform;
    private Slider hpSlider;
    private Slider staminaSlider;

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
        playerStat = playerController.GetComponent<PlayerStat>();
        crosshairTransform = transform.Find("Crosshair");
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

    void Update()
    {
        crosshairTransform.position = Camera.main.WorldToScreenPoint(playerController.mouseWorldPosition);
    }
}
