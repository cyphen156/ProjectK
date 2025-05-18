using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None,
        Ready,
        Play,
        End
    }
    
    public static GameManager Instance { get; private set; }

    [Header("GamePlayTime")]
    [SerializeField] private float PlayTime;
    private float maxPlayTime;
    private float currentTime;
    private float timeAccumulator;
    public static event Action<float> GamePlayTimeChange;

    [Header("PlayerCount")]
    private readonly Dictionary<PlayerController, PlayerState> players = new Dictionary<PlayerController, PlayerState>();

    public static event Action<int> PlayerCountChange;

    [Header("GameState")]
    [SerializeField] private GameState currentGameState;
    private string gameWinner;  
    public static event Action<string> GameEnd;

    public static event Action<PlayerController, PlayerState> LocalPlayerState;

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

    private void Update()
    {
        // 게임 레디 상태
        if (currentGameState == GameState.Ready)
        {
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
    #endregion

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
    public void RegisterAlivePlayer(PlayerController inPlayerController, PlayerState inPlayerStat)
    {
        if (!players.ContainsKey(inPlayerController))
        {
            players.Add(inPlayerController, inPlayerStat);
            //if (isLocal)
            {
                LocalPlayerState?.Invoke(inPlayerController, inPlayerStat);
            }
            PlayerCountChange?.Invoke(players.Count);
        }
    }
    public void UpdatePlayerState(PlayerController inPlayerController, PlayerState inPlayerStat)
    {
        if (players.ContainsKey(inPlayerController))
        {
            players[inPlayerController] = inPlayerStat;

            //if (isLocal)
            {
                LocalPlayerState?.Invoke(inPlayerController, inPlayerStat);
            }

            if (inPlayerStat == PlayerState.Die)
            {
                PlayerCountChange?.Invoke(GetAlivePlayerCount());
                CheckGameOver();
            }
        }
    }

    // -> 플레이어 사망시 인보크를 통해 호출로 변경 예정
    public void UnregisterAlivePlayer(PlayerController inPlayerController)
    {
        if (!players.ContainsKey(inPlayerController))
        {
            Debug.LogError("PlayerController is not registered");
            return;
        }    
        else
        {
            players.Remove(inPlayerController);
            PlayerCountChange?.Invoke(players.Count);
        }
    }

    // UI상에서 입력 받아서 게임 시작
    public void StartGame()
    {
        // 게임 시작
        currentGameState = GameState.Play;
        GamePlayTimeChange?.Invoke(currentTime);
        PlayerCountChange?.Invoke(players.Count);
    }

    public int GetAlivePlayerCount()
    {
        return players.Count(pair => pair.Value != PlayerState.Die);
    }

    private void CheckGameOver()
    {
        int aliveCount = 0;
        PlayerController lastAlive = null;

        foreach (var pair in players)
        {
            if (pair.Value != PlayerState.Die)
            {
                aliveCount++;
                lastAlive = pair.Key;

                if (aliveCount > 1)
                {
                    return;
                }
            }
        }

        if (aliveCount == 1)
        {
            EndGame(lastAlive);
        }
        else if (aliveCount == 0)
        {
            EndGame();
        }
    }
}
