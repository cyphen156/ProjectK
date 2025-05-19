using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public PlayerStat playerStat;
    private Slider hpSlider;
    private RectTransform hpSliderRect;

    private Transform targetPlayerTransform; // 추적할 대상 (플레이어 Transform)
    private Vector3 offset; // 머리 위 위치 오프셋

    private void Awake()
    {
        offset = new Vector3(0, 3f, 0);
    }

    private void Start()
    {
        PlayerController.OnChangeHpUI += UpdateHpIngameUI;

        hpSlider = transform.Find("InGameHpSlider").GetComponent<Slider>();
        hpSlider.maxValue = 100f;
        hpSliderRect = hpSlider.GetComponent<RectTransform>();

        if (playerStat != null)
        {
            targetPlayerTransform = playerStat.transform;
            playerStat.gameObject.GetComponent<PlayerController>().UpdateHpUI();
        }
    }

    private void Update()
    {
        TrackingPlayer();
    }

    private void TrackingPlayer()
    {
        // 월드 위치 + offset을 화면 좌표로 변환
        Vector3 worldPosition = targetPlayerTransform.position + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        hpSliderRect.position = screenPosition;
    }

    private void UpdateHpIngameUI(float inHp)
    {
        hpSlider.value = inHp;

        if(hpSlider.value <= 0)
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.clear;
        }
        else
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
    }
}
