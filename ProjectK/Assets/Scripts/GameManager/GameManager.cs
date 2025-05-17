using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        Ready,
        Play,
        End
    }

    public static GameManager instance;

    [Header("GamePlayTime")]
    [SerializeField] 
    private float PlayTime;
    private float maxPlayTime;
    private float currentTime;
    private float timeAccumulator;
    public static event Action<float> GamePlayTimeChange;

    [Header("PlayerCount")]
    private readonly List<PlayerController> players = new List<PlayerController>();

    public static event Action<int> PlayerCountChange;

    [Header("GameState")]
    private GameState currentGameState;
    private string gameWinner;  
    public static event Action<string> GameEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
        currentGameState = GameState.None;
        PlayTime = 5f;
        maxPlayTime = 60 * PlayTime;  // 분단위
    }

    private void OnEnable()
    {
        ResetGame();
    }

    // 게임 종료 UI 띄우기
    private void ResetGame()
    {
        // 플레이 타임 초기화
        currentTime = maxPlayTime;
        timeAccumulator = 0f;

        // 플레이어 수 초기화
        players.Clear();
        PlayerCountChange?.Invoke(players.Count);
        
        // 승자 초기화
        gameWinner = null;

        // 게임 상태 초기화
        currentGameState = GameState.Ready;
    }

    // 게임 종료 UI 띄우기
    private void EndGame()
    {
        gameWinner = null;
        GameEnd?.Invoke(gameWinner);
        currentGameState = GameState.End;
    }
    private void EndGame(PlayerController inWinner)
    {
        gameWinner = inWinner.name;
        GameEnd?.Invoke(gameWinner);
        currentGameState = GameState.End;
    }

    private void Update()
    {
        // 게임 레디 상태
        if (currentGameState == GameState.Ready)
        {
            // 레디 상태일 때는 아무것도 안할거임
            return;
        }
        
        // 게임 플레이중
        else if (currentGameState == GameState.Play)
        {
            if (currentTime <= 0)
            {
                EndGame();
                return;
            }

            // 그거 아니면 게임을 계속 진행한다
            timeAccumulator += Time.deltaTime;
            while (timeAccumulator >= 1f)
            {
                // 1초 단위로 UI에 업데이트
                currentTime -= 1f;
                timeAccumulator -= 1f;

                GamePlayTimeChange?.Invoke(currentTime);
            }
        }


        // 게임 종료 시
        else if(currentGameState == GameState.End)
        {
            // 종료 상태일때도 아무것도 안할거임   
        }
    }

    public void RegisterAlivePlayer(PlayerController inPlayerController)
    {
        if (!players.Contains(inPlayerController))
        {
            players.Add(inPlayerController);
            PlayerCountChange?.Invoke(players.Count);
        }
    }

    // -> 플레이어 사망시 인보크를 통해 호출로 변경 예정
    public void UnregisterAlivePlayer(PlayerController inPlayerController)
    {
        if (!players.Contains(inPlayerController))
        {
            Debug.LogError("PlayerController is not registered");
            return;
        }    
        else
        {
            players.Remove(inPlayerController);
            PlayerCountChange?.Invoke(players.Count);
            if (players.Count == 1)
            {
                EndGame(players[0]);
            }
            else if (players.Count == 0)
            {
                EndGame();
            }
        }
    }

    // UI상에서 입력 받아서 게임 시작
    public void StartGame()
    {
        // 게임 시작
        currentGameState = GameState.Play;
        GamePlayTimeChange?.Invoke(currentTime);
    }
}
