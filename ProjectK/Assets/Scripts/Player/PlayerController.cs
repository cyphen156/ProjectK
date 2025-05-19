using System;
using UnityEngine;

public interface IPlayerInputReceiver
{
    void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical);
    void InputAttack();
    void InputReload();
    void InputMousePosition(Vector3 inMousePosition);
    void InteractDropBox();
    void Dodge();
    void IsAim();
    void StopAttack();
}

public enum MoveType
{
    Walk,
    Run,
    Slow
}

public class PlayerController : MonoBehaviour, IPlayerInputReceiver, ITakeDamage
{
    #region variable Scope
    [Header("PlayerMovement")]
    private Vector3 lookDirection;
    [SerializeField] private float currentMoveSpeed; // 현재 움직임 속도
    private float defaultSpeed; // 기본 걷는 속도
    private float slowSpeed; // 느리게 걷는 속도
    private float runSpeed; // 뛰는 속도
    private PlayerMove playerMove; // 플레이어 무브 클래스
    private Gun playerGun;
    private GunState currentGunState;
    public Vector3 mouseWorldPosition { get; private set; }
    public MoveType currentMoveType;
    [Header("PlayerSight")]
    private PlayerSight playerSight;

    [Header("Crosshair")]
    [SerializeField] private float defaultCrosshairSize;
    [SerializeField] private float currentCrosshairSize;
    [SerializeField] private float gunCrosshairSize;
    [SerializeField] private float minCrosshairSize;
    [SerializeField] private float maxCrosshairSize;
    [SerializeField] private float crosshairLerpSpeed;
    public static event Action<float> OnCrosshairSizeChanged;

    [Header("PlayerStateMachine")]
    private PlayerStateMachine playerStateMachine;
    public static event Action<PlayerController, PlayerState> OnPlayerStateChanged;
    [SerializeField] private PlayerState currentPlayerState;
    PlayerStat playerStat;
    private BoxDetector boxDetector;
    private PlayerInventory playerInventory;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        defaultSpeed = 5.0f;
        slowSpeed = 3.0f;
        runSpeed = 8.0f;
        currentMoveSpeed = defaultSpeed;
        mouseWorldPosition = Vector3.zero;
        lookDirection = Vector3.forward;
        playerMove = GetComponent<PlayerMove>();
        currentMoveType = MoveType.Walk;

        playerStateMachine = GetComponent<PlayerStateMachine>();
        currentPlayerState = PlayerState.Idle;
        playerSight = GetComponent<PlayerSight>();
        playerStat = GetComponent<PlayerStat>();
        currentGunState = GunState.None;
        playerInventory = GetComponent<PlayerInventory>();
        boxDetector = GetComponentInChildren<BoxDetector>();

        defaultCrosshairSize = 30f;
        currentCrosshairSize = defaultCrosshairSize;
        gunCrosshairSize = 0f;
        crosshairLerpSpeed = 10f;
        minCrosshairSize = defaultCrosshairSize - gunCrosshairSize;
        maxCrosshairSize = defaultCrosshairSize + gunCrosshairSize;
    }

    private void Start()
    {
        playerGun = GetComponentInChildren<Gun>();
        GameManager.Instance.RegisterAlivePlayer(this, currentPlayerState);
    }

    private void Update()
    {
        lookDirection = CalculateDirectionFromMouseWorldPosition();
        //if (currentPlayerState == PlayerState.Dodge)
        //{
        //    lookDirection = new Vector3(inInputHorizontal, 0, inInputVertical);
        //}
        playerMove.RotateCharacter(lookDirection);
        UpdateCrosshairSize();
    }

    #endregion

    #region Input Methods
    public void InputReload()
    {
        currentGunState = GunState.Reload;
        playerGun.Reload();
    }

    public void InputAttack()
    {
        currentGunState = GunState.Attack;
        Vector3 direction = playerSight.GetRandomSpreadDirection();
        playerGun.Fire(direction);
    }

    public void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical)
    {
        if (inInputHorizontal == 0 && inInputVertical == 0)
        {
            currentPlayerState = playerStateMachine.ChangePlayerState(PlayerState.Idle);
            return;
        }
        else
        {
            currentPlayerState = PlayerState.Walk; // 움직이는 애니메이션
            currentMoveSpeed = defaultSpeed; // 기본 움직임 속도
        }

        if (inMoveType == MoveType.Slow)
        {
            currentMoveSpeed = slowSpeed;
        }
        else if (inMoveType == MoveType.Run)
        {
            currentMoveSpeed = runSpeed;
        }
        currentPlayerState = playerStateMachine.ChangePlayerState(PlayerState.Walk);
        playerMove.Move(inInputHorizontal * Time.deltaTime * currentMoveSpeed, inInputVertical * Time.deltaTime * currentMoveSpeed);
    }

    public void InputMousePosition(Vector3 inMousePosition)
    {
        mouseWorldPosition = inMousePosition;
    }
    
    public void InteractDropBox()
    {
        DropBox box = boxDetector.GetNearestBox();
        if(box == null)
        {
            return;
        }
        box.OpenBox(PickItem);
    }

    public void Dodge()
    {

    }

    public void IsAim()
    {
        currentGunState = GunState.Aim;
    }

    public void StopAttack()
    {

    }

    #endregion

    /// <summary>
    /// 현재 플레이어 상태를 애니메이션 스크립트에 넘기는 함수
    /// </summary>
    private Vector3 CalculateDirectionFromMouseWorldPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = mouseWorldPosition - currentPosition;
        direction.y = 0f;
        return direction;
    }

    private void UpdateCrosshairSize()
    {
        float previousSize = currentCrosshairSize;
        float targetCrosshairSize;
        // 상태에 따라 목표 크기 설정
        switch (currentMoveType)
        {
            case MoveType.Run:
                targetCrosshairSize = maxCrosshairSize;
                break;
            default:
                targetCrosshairSize = defaultCrosshairSize;
                break;
        }
        switch (currentGunState)
        {
            case GunState.Aim:
                targetCrosshairSize = minCrosshairSize;
                break;
            default:
                targetCrosshairSize = defaultCrosshairSize;
                break;
        }
        currentCrosshairSize = Mathf.Lerp(currentCrosshairSize, targetCrosshairSize, Time.deltaTime * crosshairLerpSpeed);

        if ((currentMoveType == MoveType.Run && currentCrosshairSize > targetCrosshairSize) ||
            (currentGunState == GunState.Aim && currentCrosshairSize < targetCrosshairSize) ||
            (currentPlayerState == PlayerState.Idle && Mathf.Abs(currentCrosshairSize - targetCrosshairSize) < 0.01f))
        {
            currentCrosshairSize = targetCrosshairSize;
        }
        if (Mathf.Abs(currentCrosshairSize - previousSize) > 0.01f)
        {
            OnCrosshairSizeChanged?.Invoke(currentCrosshairSize);
        }
    }

    public void TakeDamage(float inBulletDamage)
    {
        playerStat.ApplyHp(-inBulletDamage);
    }

    //아이템픽하는거
    public ItemBase PickItem(ItemBase inPickItem)
    {
      //  Debug.Log("아이템 픽 " + inPickItem.name);
       return playerInventory.PickItem(inPickItem); //있던 슬롯에서 교체될것은 반환
    }
}
