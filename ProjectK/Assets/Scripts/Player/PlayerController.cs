using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

public interface IPlayerInputReceiver
{
    void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical);
    void InputAttack();
    void InputReload();
    void InputMousePosition(Vector3 inMousePosition);
    void InteractDropBox();
    void Dodge();
    void IsAim(bool isAim);
    void UseItem(int number);
    void UseGranade();

}

public enum MoveType
{
    Walk,
    Run,
    Slow
}

public class PlayerController : NetworkBehaviour, IPlayerInputReceiver, ITakeDamage
{
    #region variable Scope
    private NetworkVariable<uint> myNetworkNumber;

    [Header("PlayerMovement")]
    private Vector3 lookDirection;
    [SerializeField] private float currentMoveSpeed; // 현재 움직임 속도
    private float defaultSpeed; // 기본 걷는 속도
    private float slowSpeed; // 느리게 걷는 속도
    private float runSpeed; // 뛰는 속도
    private PlayerMove playerMove; // 플레이어 무브 클래스
    private Gun playerGun;
    private bool isAimed;
    public Vector3 mouseWorldPosition;
    public static event Action<Vector3> OnMousePositionUpdated;

    public MoveType currentMoveType;
    [Header("PlayerSight")]
    private PlayerSight playerSight;

    [Header("Crosshair")]
    [SerializeField] private float defaultCrosshairSize;
    [SerializeField] private float currentCrosshairSize;
    [SerializeField] private float gunCrosshairSize;
    [SerializeField] private float crosshairspreadRadius;
    [SerializeField] private float crosshairLerpSpeed;
    public static event Action<float> OnCrosshairSizeChanged;

    [Header("PlayerStateMachine")]
    private PlayerStateMachine playerStateMachine;
    public static event Action<PlayerController, PlayerState> OnPlayerStateChanged;
    [SerializeField] private PlayerState currentPlayerState;
    private NetworkVariable<PlayerState> netCurrentPlayerState; 
    private PlayerStat playerStat;
    private BoxDetector boxDetector;
    private PlayerInventory playerInventory;

    public static event Action<float> OnChangeHpUI;

    private float lastInputHorizontal = 0f;
    private float lastInputVertical = 0f;

    [Header("Granande")]
    [SerializeField] private Granade granadePrefab;
    #endregion



    #region Unity Methods
    private void Awake()
    {
        myNetworkNumber = new NetworkVariable<uint>();
        defaultSpeed = 5.0f;
        slowSpeed = 3.0f;
        runSpeed = 8.0f;
        currentMoveSpeed = defaultSpeed;
        mouseWorldPosition = Vector3.zero;
        lookDirection = Vector3.forward;
        playerMove = GetComponent<PlayerMove>();
        currentPlayerState = PlayerState.Idle;
        playerSight = GetComponent<PlayerSight>();
        playerStat = new PlayerStat();
        playerInventory = GetComponent<PlayerInventory>();
        boxDetector = GetComponentInChildren<BoxDetector>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        defaultCrosshairSize = 30f;
        currentCrosshairSize = defaultCrosshairSize;
        gunCrosshairSize = 0f;
        crosshairspreadRadius = 10f;
        crosshairLerpSpeed = 5f;

        netCurrentPlayerState = new NetworkVariable<PlayerState>(PlayerState.Idle);
        
    }

    private void Start()
    {
        playerGun = GetComponentInChildren<Gun>();
        GameManager.Instance.RegisterAlivePlayer(this, netCurrentPlayerState.Value, IsOwner);

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
        ChangeGunStateServerRpc(GunState.Reload);
    }

    public void InputAttack()
    {
        ChangeGunStateServerRpc(GunState.Attack);

        Vector3 direction = playerSight.GetRandomSpreadDirection();
        playerGun.Fire(direction);
    }


    [ServerRpc]
    private void ChangeGunStateServerRpc(GunState inState)
    {
        playerGun.ChangeGunState(inState);
    }


    public void InputMove(MoveType inMoveType, float inInputHorizontal, float inInputVertical)
    {
        lastInputHorizontal = inInputHorizontal;
        lastInputVertical = inInputVertical;
        
        if (inInputHorizontal == 0 && inInputVertical == 0)
        {
            ChangeStateServerRpc(PlayerState.Idle, NetworkManager.Singleton.LocalClientId);
            return;
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
            currentMoveSpeed = defaultSpeed; // 기본 움직임 속도
            currentMoveType = MoveType.Walk;
        }

        ChangeStateServerRpc(PlayerState.Walk, NetworkManager.Singleton.LocalClientId);
        // inState

        playerMove.Move(inInputHorizontal * Time.deltaTime * currentMoveSpeed, inInputVertical * Time.deltaTime * currentMoveSpeed);
    }

    [ServerRpc]
    private void ChangeStateServerRpc(PlayerState inState, ulong inId)
    {
        netCurrentPlayerState.Value = playerStateMachine.ChangePlayerState(inState);
    }

    public void InputMousePosition(Vector3 inMousePosition)
    {
        mouseWorldPosition = inMousePosition;
        OnMousePositionUpdated?.Invoke(mouseWorldPosition);
    }

    public void InteractDropBox()
    {
        if (IsOwner)
        {
            DropBox box = boxDetector.GetNearestBox();
            if (box == null)
            {
                return;
            }
            box.OpenBox(myNetworkNumber.Value);
        }
    }

    public void Dodge()
    {
        //1. 구르기 애니메이션 재생
        currentPlayerState = playerStateMachine.ChangePlayerState(PlayerState.Dodge);
        //2. 구르기 방향 결정
        Vector3 dodgeDirection = new Vector3(lastInputHorizontal, 0, lastInputVertical).normalized;
        if(dodgeDirection == Vector3.zero)
        {
            dodgeDirection = transform.forward; // 입력이 없으면 정면
        }
        float dodgeDistance = 5.0f; // 구르기 거리
        playerMove.Move(dodgeDirection.x * dodgeDistance, dodgeDirection.z * dodgeDistance);
        //4. 방향도 맞춰주고 싶으면
        playerMove.RotateCharacter(dodgeDirection);
    }

    public void IsAim(bool isAim)
    {
        isAimed = isAim;
        currentMoveType = MoveType.Slow;
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
        //호스트에서만 총알, 대상이 충돌 체크를 하므로 해당 함수는 호스트에서만 발동됨
        TakeDamageRpc(inBulletDamage);
    }

    [Rpc(SendTo.Everyone)]
    private void TakeDamageRpc(float inBulletDamage)
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
        ItemBase previousItem = playerInventory.TryAddOrReturnPreviousItem(inPickItem, playerGun);
  
        return previousItem; //있던 슬롯에서 교체될것은 반환
    }

    public void SetNetworkNumber(uint inNumber)
    {
        myNetworkNumber.Value = inNumber;
    }

    public uint GetNetworkNumber()
    {
        return myNetworkNumber.Value;
    }

    [Rpc(SendTo.Everyone)]
    public void SetSpawnPositionRpc(Vector3 inPosition)
    {
        if (IsOwner)
        {
            transform.position = inPosition;
        }
    }


    public void UseItem(int inIndex)
    {

    }

    public void UseGranade()
    {
        //if () // 플레이어 인벤토리 4번 슬롯 갯수 확인
        {
            SpawnGranadeRpc();
        }
    }
    private void SpawnGranadeRpc()
    {
        Granade granade = Instantiate(granadePrefab, transform.position, Quaternion.LookRotation(lookDirection));
        granade.SetOwner(GetNetworkNumber());
        Vector3 start = transform.position + Vector3.up;
        granade.Launch(start, mouseWorldPosition);
    }
}
