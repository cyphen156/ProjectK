using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private float maxPlayTime;
    private float currentTime;
    [SerializeField] 
    private float PlayTime;
    private int remainPlayerCount;

    private LayerMask playerLayer;

    public static event Action<int> PlayerCountChange;
    public static event Action<float> GamePlayTimeChange;
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

        PlayTime = 5f;
        maxPlayTime = 60 * PlayTime;  // 분단위
        currentTime = maxPlayTime;

        playerLayer = 1 << 8;
    }

    private void OnEnable()
    {
        InitGame();
    }

    private void InitGame()
    {
        remainPlayerCount = CountRemainPlayer(playerLayer);
        PlayerCountChange?.Invoke(remainPlayerCount);
    }

    // 게임 종료 UI 띄우기
    private void ResetGame()
    {

    }

    private void Update()
    {
        if (currentTime < 0)
        {
            ResetGame();
        }

        if (remainPlayerCount <= 1)
        {
            ResetGame();
        }

        // 그거 아니면 게임을 계속 진행한다
        currentTime -= Time.deltaTime;
        GamePlayTimeChange?.Invoke(currentTime);


    }

    [Obsolete]
    private int CountRemainPlayer(LayerMask playerLayer)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        int count = 0;
        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & playerLayer.value) != 0)
            {
                count++;
            }
        }

        return count;
    }

    public void DiscountPlayerCount()
    {
        remainPlayerCount--;    
        PlayerCountChange?.Invoke(remainPlayerCount);
    }
}
