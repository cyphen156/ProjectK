using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SystemUIManager : MonoBehaviour
{
    [Header("GameLifeTime")]
    private TextMeshProUGUI GameLifeTimeText;
    private float minutes;
    private float seconds;

    [Header("RestPlayerText")]
    private TextMeshProUGUI RestPlayerText;

    [Header("GameEndPanel")]
    private GameObject gameEndPanel;
    private TextMeshProUGUI winnerInfoText;

    private void Awake()
    {
        minutes = 0f;
        seconds = 0f;

        GameLifeTimeText = transform.Find("GameLifeTimeBackground/GameLifeTimeText").GetComponentInParent<TextMeshProUGUI>();
        RestPlayerText = transform.Find("RestPlayerBackground/RestPlayerText").GetComponentInParent<TextMeshProUGUI>();
        gameEndPanel = GameObject.Find("GameEndPanel");
        winnerInfoText = gameEndPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {

        if (RestPlayerText == null)
        {
            Debug.LogError("RestPlayerText가 Null임");
        }

        if (GameLifeTimeText == null)
        {
            Debug.LogError("GameLifeTimeText가 Null임");
        }

        GameManager.GamePlayTimeChange += UpdateGameLifeTime;
        GameManager.PlayerCountChange += UpdateRestPlayer;
        GameManager.OnWinnerChanged += UpdateLastPlayer;
        GameManager.OnGameStateChanged += UpdateGameState; 
        gameEndPanel.SetActive(false);
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

    private void UpdateLastPlayer(uint inLastPlayerNumber)
    {
        string lastPlayerInfo;
        if (inLastPlayerNumber == 99999999)
        {
            lastPlayerInfo = "All Player has been destroied\n Draw Game";
        }
        else
        {
            lastPlayerInfo = "Winner is Player No." + inLastPlayerNumber;
        }
        winnerInfoText.text = lastPlayerInfo;
    }

    private void UpdateGameState(GameState inGameState)
    {
        if (inGameState == GameState.End)
        {
            gameEndPanel.SetActive(true);
            return;
        }
        gameEndPanel.SetActive(false);
    }
}
