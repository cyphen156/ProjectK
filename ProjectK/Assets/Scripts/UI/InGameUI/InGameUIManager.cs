using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    private PlayerStat playerStat;
    private Slider hpSlider;

    private void Start()
    {
        playerStat = GetComponentInParent<PlayerStat>();
        hpSlider = transform.Find("InGameHpSlider").GetComponent<Slider>();

        // hp바 초기화
        hpSlider.maxValue = 100f;
        if (playerStat != null)
        {
            hpSlider.value = playerStat.GetHP();
        }

        // 카메라 방향으로 바라보게 하는 코드
        transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
    }

    private void Update()
    {
        // 카메라 방향으로 바라보게 하는 코드
        transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
    }
}
