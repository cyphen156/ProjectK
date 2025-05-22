using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 입력을 해석하고 로컬 플레이어에게만 전달하는 전역 입력 처리기
/// </summary>

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static event Action<PlayerController> OnLocalPlayerRegistered;
    private IPlayerInputReceiver localPlayerController;
    private PlayerState localPlayerState;
    private InGameUIManager inGameUIManager;

    public enum InputReceiver
    {
        None,
        PlayerOnly,
        InGameUIOnly,
        All
    }
    [SerializeField]
    private InputReceiver currentReceiver;

    #region
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
        currentReceiver = InputReceiver.All;
    }

    private void OnEnable()
    {
        GameManager.LocalPlayerState += UpdateLocalPlayerStateChanged;
    }

    private void OnDisable()
    {
        GameManager.LocalPlayerState -= UpdateLocalPlayerStateChanged;
    }

    private void Update()
    {
        //if (localPlayerController == null)
        //{
        //    return;
        //}

        if (currentReceiver == InputReceiver.None)
        {
            return;
        }


        if (localPlayerState == PlayerState.Die)
            //|| currentPlayerState == PlayerState.Dodge
        {
            return;
        }

        if (currentReceiver == InputReceiver.All)
        {
            HandlePlayerInput();
        }
    }
    #endregion

    public void RegisterLocalPlayer(IPlayerInputReceiver inPlayer)
    {
        localPlayerController = inPlayer;
        if (inPlayer is PlayerController controller)
        {
            OnLocalPlayerRegistered?.Invoke(controller);
        }
        currentReceiver = InputReceiver.All;
    }
    public void RegisterUIManager(InGameUIManager inManager)
    {
        inGameUIManager = inManager;
    }
    public void SetInputReceiver(InputReceiver inReceiver)
    {
        currentReceiver = inReceiver;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        return worldPos;
    }

    private void HandlePlayerInput()
    {
        // 마우스 월드 좌표 처리
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        localPlayerController.InputMousePosition(mouseWorldPosition);

        // 이동 처리
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        MoveType moveType = MoveType.Walk;
        bool isAim = false;

        // 달리기
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveType = MoveType.Run;
        }
        // 조준시
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            isAim = true;
            moveType = MoveType.Slow;
        }

        localPlayerController.IsAim(isAim);

        // 이동
        localPlayerController.InputMove(moveType, h, v);

        // 공격
        if (Input.GetKey(KeyCode.Mouse0))
        {
            localPlayerController.InputAttack();
        }

        // 재장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            localPlayerController.InputReload();
        }

        // 드롭박스 상호작용
        if (Input.GetKeyDown(KeyCode.F))
        {
            localPlayerController.InteractDropBox();
        }

        // 구르기
        if (Input.GetKeyDown(KeyCode.V))
        {
            localPlayerController.Dodge();
        }
    }
    private void UpdateLocalPlayerStateChanged(PlayerController inPlayerController, PlayerState inLocalPlayerState)
    {
        if (localPlayerController == null)
        {
            RegisterLocalPlayer(inPlayerController);
        }
        localPlayerState = inLocalPlayerState;
    }
}