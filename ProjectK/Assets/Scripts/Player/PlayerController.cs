using System;
using System.Collections;
using UnityEngine;

public interface IPlayerInputReceiver
{
    void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical);
    void InputAttack();
    void InputReload();
    void InputMousePosition(Vector3 inMousePosition);
    void InteractDropBox();
    void Dodge();
    void IsAim(bool isAim);
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
    private bool isAimed;
    public Vector3 mouseWorldPosition { get; private set; }
    public MoveType currentMoveType;
    [Header("PlayerSight")]
    private PlayerSight playerSight;

    [Header("Crosshair")]
    [SerializeField] private float defaultCrosshairSize;
    [SerializeField] private float currentCrosshairSize;
    [SerializeField] private float gunCrosshairSize;
    [SerializeField] private float crosshairspreadRadius;
    [SerializeField] private float minCrosshairSize;
    [SerializeField] private float maxCrosshairSize;
    [SerializeField] private float crosshairLerpSpeed;
    public static event Action<float> OnCrosshairSizeChanged;

    [Header("PlayerStateMachine")]
    private PlayerStateMachine playerStateMachine;
    public static event Action<PlayerController, PlayerState> OnPlayerStateChanged;
    [SerializeField] private PlayerState currentPlayerState;
    private PlayerStat playerStat;
    private BoxDetector boxDetector;
    private PlayerInventory playerInventory;
    #endregion

    public static event Action<float> OnChangeHpUI;

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
        currentPlayerState = PlayerState.Idle;
        playerSight = GetComponent<PlayerSight>();
        playerStat =  new PlayerStat();
        playerInventory = GetComponent<PlayerInventory>();
        boxDetector = GetComponentInChildren<BoxDetector>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        defaultCrosshairSize = 30f;
        currentCrosshairSize = defaultCrosshairSize;
        gunCrosshairSize = 0f;
        crosshairspreadRadius = 10f;
        crosshairLerpSpeed = 5f;
        minCrosshairSize = defaultCrosshairSize - gunCrosshairSize;
        maxCrosshairSize = defaultCrosshairSize + gunCrosshairSize;
    }

    private void Start()
    {
        playerGun = GetComponentInChildren<Gun>();
        GameManager.Instance.RegisterAlivePlayer(this, currentPlayerState);
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;
        OnChangeHpUI?.Invoke(playerStat.GetHP());
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
        playerGun.ChangeGunState(GunState.Reload);
    }

    public void InputAttack()
    {
        playerGun.ChangeGunState(GunState.Attack);
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
            currentMoveType = MoveType.Run;
        }
        else
        {
            currentMoveType = MoveType.Walk;
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

    public void IsAim(bool isAim)
    {
        isAimed = isAim;
        currentMoveType = MoveType.Slow;
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
        float targetCrosshairSize = defaultCrosshairSize;
        // 상태에 따라 목표 크기 설정
        if (currentMoveType == MoveType.Run)
        {   
            targetCrosshairSize += crosshairspreadRadius;
        }
        
        if (playerGun != null)
        {
            targetCrosshairSize += playerGun.equiptFocusRegion;

            if (isAimed)
            {
                targetCrosshairSize -= crosshairspreadRadius;
            }

        }

        currentCrosshairSize = Mathf.Lerp(currentCrosshairSize, targetCrosshairSize, Time.deltaTime * crosshairLerpSpeed);

        if (Mathf.Abs(currentCrosshairSize - targetCrosshairSize) < 0.01f)
        {
            currentCrosshairSize = targetCrosshairSize;
        }

        //if (Mathf.Abs(currentCrosshairSize - previousSize) > 0.01f)
        //{
            OnCrosshairSizeChanged?.Invoke(currentCrosshairSize);
        //}
    }

    public void TakeDamage(float inBulletDamage)
    {
        playerStat.ApplyHp(-inBulletDamage);
        float hp = playerStat.GetHP();

        OnChangeHpUI?.Invoke(hp);
        if (hp <= 0)
        {
            Logger.Info("플레이어가 죽음");
            return;
        }
    }

    //아이템픽하는거
    public ItemBase PickItem(ItemBase inPickItem)
    {
        //인벤토리 적용
        ItemBase previousItem = playerInventory.TryAddOrReturnPreviousItem(inPickItem);
        if(inPickItem.itemType == ItemMainType.AttachMent)
        {
            playerGun.EquiptItems(playerInventory.GetGunItmes());
        }
        return previousItem; //있던 슬롯에서 교체될것은 반환
    }
}
