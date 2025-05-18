using System;
using UnityEngine;

public interface IPlayerInputReceiver
{
    void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical);
    void InputAttack();
    void InputReload();
    void RotateCharacterOnMousePosition(Vector3 inDirection);
    void InteractDropBox();
    void Dodge();
    void IsAim();
}
public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Attack,
    Reload,
    Die
}
public enum MoveType
{
    Walk,
    Run,
    Slow
}

public class PlayerController : MonoBehaviour, IPlayerInputReceiver
{
    [Header("PlayerMovement")]
    [SerializeField] private float currentMoveSpeed; // 현재 움직임 속도
    private float defaultSpeed; // 기본 걷는 속도
    private float slowSpeed; // 느리게 걷는 속도
    private float runSpeed; // 뛰는 속도
    private PlayerMove playerMove; // 플레이어 무브 클래스
    private Gun playerGun;

    [Header("PlayerAnimation")]
    private PlayerAnimation playerAnimation;
    public static event Action<PlayerController, PlayerState> OnPlayerStateChanged;
    [SerializeField] private PlayerState currentPlayerState;

    #region Unity Methods
    private void Awake()
    {
        defaultSpeed = 5.0f;
        slowSpeed = 3.0f;
        runSpeed = 8.0f;
        currentMoveSpeed = defaultSpeed;
        playerMove = GetComponent<PlayerMove>();
        playerAnimation = GetComponent<PlayerAnimation>();
        currentPlayerState = PlayerState.Idle;
    }


    private void Start()
    {
        playerGun = GetComponentInChildren<Gun>();
        GameManager.Instance.RegisterAlivePlayer(this, currentPlayerState);
    }

    private void Update()
    {
        AniConrtrol();
    }
    #endregion

    #region Input Methods
    public void InputReload()
    {
        currentPlayerState = PlayerState.Reload;
        playerGun.Reload();
    }

    public void InputAttack()
    {
        currentPlayerState = PlayerState.Attack;
        playerGun.Fire(transform.forward);
    }

    public void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical)
    {
        if (inInputHorizontal == 0 && inInputVertical == 0)
        {
            currentPlayerState = PlayerState.Idle;
            return;
        }
        else
        {
            currentPlayerState = PlayerState.Walk; // 움직이는 애니메이션 // 추후 뛰기 추가 예정
        }

        currentMoveSpeed = defaultSpeed; // 기본 움직임 속도
        if (inMoveType == MoveType.Slow)
        {
            currentMoveSpeed = slowSpeed;
        }
        else if (inMoveType == MoveType.Run)
        {
            currentMoveSpeed = runSpeed;
        }

        playerMove.Move(inInputHorizontal * Time.deltaTime * currentMoveSpeed, inInputVertical * Time.deltaTime * currentMoveSpeed);
    }
    public void RotateCharacterOnMousePosition(Vector3 inMouseWorldPosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = inMouseWorldPosition - currentPosition;
        direction.y = 0f;

        transform.LookAt(transform.position + direction);
    }

    public void InteractDropBox()
    {

    }

    public void Dodge()
    {

    }

    public void IsAim()
    {

    }
    #endregion

    /// <summary>
    /// 현재 플레이어 상태를 애니메이션 스크립트에 넘기는 함수
    /// </summary>
    private void AniConrtrol()
    {
        playerAnimation.AnimationConrtrol(currentPlayerState);
    }
}
