using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
public enum GameState
{
    None,
    Ready,
    Play,
    End
}

public class GameManager : NetworkBehaviour
{
    public SpawnAssginer spawnAssigner;
    public static GameManager Instance { get; private set; }

    [Header("GamePlayTime")]
    [SerializeField] private float PlayTime;
    private float maxPlayTime;
    private float currentTime;
    private float timeAccumulator;
    public static event Action<float> GamePlayTimeChange;

    [Header("PlayerCount")]
    private const uint INVALID_PLAYER_NUMBER = 99999999;
    private readonly Dictionary<PlayerController, PlayerState> players = new Dictionary<PlayerController, PlayerState>();

    public static event Action<int> PlayerCountChange;

    [Header("GameState")]
    [SerializeField] private NetworkVariable<GameState> currentGameState = new NetworkVariable<GameState>(GameState.None);

    private uint gameWinner;
    public static event Action<uint> OnWinnerChanged;
    public static event Action<GameState> OnGameStateChanged;

    public static event Action<PlayerController, PlayerState> LocalPlayerState;

    public static event Action OnHideLobbyUIRequested;

    [Header("DropBox")]
    [SerializeField] private GameObject dropboxPrefab;
    public List<Transform> dropboxSpawnTransform;


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
        PlayTime = 5f;
        maxPlayTime = 60 * PlayTime;  // 분단위
    }

    //private void OnEnable()
    //{
    //    ResetGame();
    //}

    private void Update()
    {
        // 게임 레디 상태에서 시작하기
        if (currentGameState.Value == GameState.Ready && IsHost && Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
            return;
        }

        // 게임 플레이중
        else if (currentGameState.Value == GameState.Play)
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
        else if (currentGameState.Value == GameState.End)
        {
            // 종료 상태일때도 아무것도 안할거임   
        }
    }
    #endregion
    private void ChangeGameState(GameState inGameState)
    {
        if (!IsHost)
        {
            return; 
        }

        if (currentGameState.Value == inGameState)
        {
            return;
        }
        currentGameState.Value = inGameState;
        OnGameStateChanged?.Invoke(inGameState);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            ResetGame();
        }
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
        gameWinner = INVALID_PLAYER_NUMBER;

        // 게임 상태 초기화
        ChangeGameState(GameState.Ready);
    }

    // 게임 종료 UI 띄우기
    private void EndGame()
    {
        OnWinnerChanged?.Invoke(gameWinner);
        ChangeGameState(GameState.End);
    }
    private void EndGame(PlayerController inWinner)
    {
        gameWinner = inWinner.GetNetworkNumber();
        OnWinnerChanged?.Invoke(gameWinner);
        ChangeGameState(GameState.End);
        StartCoroutine(GameEnd(10f));
    }

    private IEnumerator GameEnd(float inTime)
    {
        yield return new WaitForSeconds(inTime);
        ResetGame();
    }
    public void RegisterAlivePlayer(PlayerController inPlayerController, PlayerState inPlayerStat, bool inIsOwner = true)
    {
        if (!players.ContainsKey(inPlayerController))
        {
            players.Add(inPlayerController, inPlayerStat);
            if (inIsOwner) //심볼이 Multi 면 오너값이 제대로 들어옴
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
        AllocatePlayerNomber();
        SpawnDropBox();
        AssignPlayerPosition();
        ChangeGameState(GameState.Play);
        ApplyStartUIRpc();
    }

    public void RequestStartGame()
    {
        if (IsHost)
        {
            StartGame();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ApplyStartUIRpc()
    {
        OnHideLobbyUIRequested?.Invoke();
        GamePlayTimeChange?.Invoke(currentTime);
        PlayerCountChange?.Invoke(players.Count);
    }

    private void SpawnDropBox()
    {
        for (int i = 0; i < dropboxSpawnTransform.Count; i++)
        {
            GameObject dropBox = Instantiate(dropboxPrefab, dropboxSpawnTransform[i].position, Quaternion.identity);
            dropBox.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void AssignPlayerPosition()
    {
        int i = 0;
        List<Transform> spawnTransformList = spawnAssigner.GetSpawnPositionList();
        foreach (var item in players)
        {
            //플레이어 스폰 위치 리스트가 섞여서 왔다고 가정
            item.Key.SetSpawnPositionRpc(spawnTransformList[i].position);
            i++;
        }
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

    private void AllocatePlayerNomber()
    {
        uint number = 1;
        foreach (var player in players.Keys.OrderBy(p => p.OwnerClientId))
        {
            player.SetNetworkNumber(number++);
        }
    }

    public PlayerController GetPlayer(uint inPlayerNumber)
    {
        foreach(var player in players)
        {
            if(player.Key.GetNetworkNumber() == inPlayerNumber)
            {
                return player.Key;
            }
        }
        return null;
    }
}
