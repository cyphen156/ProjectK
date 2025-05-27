using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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

    [Header("DieUI")]
    private GameObject PlayerDiePanel; // 플레이어 사망시 UI (재훈)
    private TextMeshProUGUI PlayerDieText; // 플레이어 사망시 UI (재훈)

    [Header("GamePlayUI")]
    private GameObject lobbyPanel;

    [Header("Login")]
    private GameObject loginPanel;
    private UnityTransport unityTransport;
    [SerializeField] private TMP_InputField ipInputField;

    private void Awake()
    {
        minutes = 0f;
        seconds = 0f;

        GameLifeTimeText = transform.Find("GameLifeTimeBackground/GameLifeTimeText").GetComponentInParent<TextMeshProUGUI>();
        RestPlayerText = transform.Find("RestPlayerBackground/RestPlayerText").GetComponentInParent<TextMeshProUGUI>();
        gameEndPanel = GameObject.Find("GameEndPanel");
        winnerInfoText = gameEndPanel.GetComponentInChildren<TextMeshProUGUI>();

        PlayerDiePanel = GameObject.Find("PlayerDiePanel"); // 플레이어 사망시 UI (재훈)
        PlayerDieText = PlayerDiePanel.GetComponentInChildren<TextMeshProUGUI>(); // 플레이어 사망시 UI (재훈)

        lobbyPanel = GameObject.Find("LobbyPanel");

        loginPanel = GameObject.Find("LoginPanel");
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

        GameManager.LocalPlayerState += UpdateLocalPlayerState; // 플레이어 사망시 UI (재훈)
        PlayerDiePanel.SetActive(false); // 플레이어 사망시 UI (재훈)

        GameManager.OnHideLobbyUIRequested += HideLobbyPanel;

        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
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

    private void UpdateLocalPlayerState(PlayerController inPlayerController, PlayerState inState)
    {
        if (inState == PlayerState.Die)
        {
            PlayerDieText.text = "플레이어" + inPlayerController.GetNetworkNumber() + "사망";

            PlayerDiePanel.SetActive(true);
        }
        else
        {
            PlayerDiePanel.SetActive(false);
        }
    }

    #region 로비 UI
    public void OnClickPlayButton()
    {
        GameManager.Instance.RequestStartGame();
    }

    private void HideLobbyPanel()
    {
        lobbyPanel.SetActive(false);
    }
    #endregion

    #region 로그인 UI
    public void OnClickButtonCreateHostRoom()
    {
        NetworkManager.Singleton.StartHost();
        loginPanel.SetActive(false);
    }


    public void OnClickButtonJoinClient()
    {
        unityTransport.ConnectionData.Address = ipInputField.text;
        NetworkManager.Singleton.StartClient();
        loginPanel.SetActive(false);
    }
    #endregion
}
