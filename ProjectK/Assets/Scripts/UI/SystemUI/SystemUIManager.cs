using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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


        GameManager.currentTime.OnValueChanged += UpdateGameLifeTime;
        GameManager.alivePlayCount.OnValueChanged += UpdateRestPlayer;
        GameManager.OnWinnerChanged += UpdateLastPlayer;
        GameManager.OnGameStateChanged += UpdateGameState; 
        gameEndPanel.SetActive(false);

        GameManager.LocalPlayerState += UpdatePlayerDie; // 플레이어 사망시 UI (재훈)
        PlayerDiePanel.SetActive(false); // 플레이어 사망시 UI (재훈)

        GameManager.OnHideLobbyUIRequested += HideLobbyPanel;

        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    private void UpdateGameLifeTime(float pre, float inCurrentTime)
    {
        minutes = inCurrentTime / 60;
        seconds = inCurrentTime % 60;

        GameLifeTimeText.text = $"{(int)minutes} : {(int)seconds}";
    }

    private void UpdateRestPlayer(int pre, int inCurrentPlayer)
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

    private void UpdatePlayerDie(PlayerController inPlayerController, PlayerState inState)
    {
        //inPlayercontroller 케릭터가 죽었을때, 해당 케릭터 주인의 systemUIManger가 패널이 떠야한다. 
        if (inState == PlayerState.Die)
        {
            PlayerDie(inPlayerController.GetNetworkNumber());
        }
    }

    private void PlayerDie(uint inPlayerNumber)
    {
      // playerNumber를 매길때는 1부터, Localclient는 0번부터 시작인데, SetNumber도 문제고
      // LocalClientId 를 쓰면 좋은데 얘 값은 uLong 이걸 쓰는 변수가 uInt로 너무많은 변수가 있어서 로컬클라에 +1 
        if(inPlayerNumber == (NetworkManager.Singleton.LocalClientId + 1))
        {
            PlayerDieText.text = "플레이어" + (inPlayerNumber) + "사망";

            PlayerDiePanel.SetActive(true);
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
        string Address = ipInputField.text;
        if(Address == "")
        {
            Address = "127.0.0.1";
        }
        unityTransport.ConnectionData.Address = Address;
        NetworkManager.Singleton.StartClient();
        loginPanel.SetActive(false);
    }
    #endregion
}
