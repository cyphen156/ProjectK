using System;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Attack,
    Reload,
    Die
}

public class PlayerController : MonoBehaviour
{
    [Header("PlayerMovement")]
    [SerializeField] private float currentMoveSpeed; // 현재 움직임 속도
    private float defaultSpeed; // 기본 걷는 속도
    private float runSpeed; // 뛰는 속도
    private PlayerMove playerMove; // 플레이어 무브 클래스
    private float inputHorizontal; // AD 인풋 값
    private float inputVertical; // WS 인풋 값
    private Vector3 mousePosition;
    private Gun playerGun;

    [Header("PlayerAnimation")]
    private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerState currentPlayerState; // 현재 플레이어의 상태

    private void Awake()
    {
        defaultSpeed = 5.0f;
        runSpeed = 8.0f;
        currentMoveSpeed = defaultSpeed;
        playerMove = GetComponent<PlayerMove>();
        playerAnimation = GetComponent<PlayerAnimation>();
        currentPlayerState = PlayerState.Idle;
    }

    private void Start()
    {
        playerGun = GetComponentInChildren<Gun>();
        mousePosition = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        InputMove();
        InputAttack(); // 테스트용
        InputReload(); // 테스트용
        AniConrtrol();
        RotateCharacterOnMousePosition();
    }

    private void InputReload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentPlayerState = PlayerState.Reload;
            playerGun.Reload();
        }
    }

    private void InputAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentPlayerState = PlayerState.Attack;

            playerGun.Fire(transform.forward);
        }
    }

    /// <summary>
    /// 움직임 관련 인풋 함수
    /// </summary>
    private void InputMove()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        if (inputHorizontal == 0 && inputVertical == 0)
        {
            currentPlayerState = PlayerState.Idle;
            return;
        }
        else
        {
            currentPlayerState = PlayerState.Walk; // 움직이는 애니메이션 // 추후 뛰기 추가 예정
        }

        currentMoveSpeed = defaultSpeed; // 기본 움직임 속도
        if (Input.GetKey(KeyCode.LeftShift)) // 달리기
        {
            currentMoveSpeed = runSpeed;
        }

        playerMove.Move(inputHorizontal * Time.deltaTime * currentMoveSpeed, inputVertical * Time.deltaTime * currentMoveSpeed);
    }

    /// <summary>
    /// 현재 플레이어 상태를 애니메이션 스크립트에 넘기는 함수
    /// </summary>
    private void AniConrtrol()
    {
        playerAnimation.AnimationConrtrol(currentPlayerState);
    }

    private void RotateCharacterOnMousePosition()
    {
        mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y; // 혹은 캐릭터까지의 거리

        Vector3 MouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        MouseWorldPosition.y = 0f;

        transform.LookAt(MouseWorldPosition);
    }
}
