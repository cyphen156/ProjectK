using TMPro;
using UnityEngine;

public class SystemUIManager : MonoBehaviour
{
    private TextMeshProUGUI GameLifeTimeText;
    private float minutes;
    private float seconds;

    private TextMeshProUGUI RestPlayerText;

    private void Awake()
    {
        minutes = 0f;
        seconds = 0f;
    }

    private void Start()
    {
        GameLifeTimeText = transform.Find("GameLifeTimeBackground/GameLifeTimeText").GetComponentInParent<TextMeshProUGUI>();
        RestPlayerText = transform.Find("RestPlayerBackground/RestPlayerText").GetComponentInParent<TextMeshProUGUI>();

        if(RestPlayerText == null)
        {
            Debug.LogError("RestPlayerText가 Null임");
        }

        if (GameLifeTimeText == null)
        {
            Debug.LogError("GameLifeTimeText가 Null임");
        }

        GameManager.GamePlayTimeChange += UpdateGameLifeTime;
        GameManager.PlayerCountChange += UpdateRestPlayer;
    }

    private void UpdateGameLifeTime(float inCurrentTime)
    {
        minutes = inCurrentTime / 60;
        seconds = inCurrentTime % 60;

        GameLifeTimeText.text = $"{(int)minutes} : {(int)seconds}";
    }

    private void UpdateRestPlayer(int inCurrentPlayer)
    {
        RestPlayerText.text = $" Rest : {inCurrentPlayer}";
    }
}
