using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public enum GunState
{
    None,
    Attack,
    Reload
}
public class Gun : NetworkBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform fireTransform;

    public Animator animator;

    public static event Action<Vector3> OnFire;

    public GunState CurrentGunState { get; private set; }
    private bool isReloading;
    private float defaultReloadTime; //기본 탄창 채우는 시간
    private float equiptReloadTime; //스텟 적용
    private float restReloadTime; //채우기 까지 남은 시간
   

    private int defaultBulletCount; //기본 총알 수
    private int equiptBulletCount; //스텟 적용
    private int restBulletCount; //남은 총알 수
    private int defaultRps; //스텟 적용
    private int equiptRps; //1초당 총알 발사 갯수

    public bool canShoot = true; // 총 발사 가능 여부 (구르기 시 총기 발사 X 구현위해 만듬)

    private bool isRating; //연사속도대기
    private float rateTime;
    private float restRateTime;

    private float defaultFocusRegion; //탄 밀집도 : 클수록 퍼진다.
    public float equiptFocusRegion;

    public static event Action<int> OnChageAmmoUI;
    private bool isStateLock;

    private void Awake()
    {
        defaultReloadTime = 2f;
        restReloadTime = 0f;

        defaultRps = 4;
        restRateTime = 0f;

        defaultBulletCount = 30;
        restBulletCount = defaultBulletCount;

        defaultFocusRegion = 1f; //조준 반경
        isRating = false;
        isReloading = false;
        isStateLock = false;
        CurrentGunState = GunState.None;
        ResetEquiptValue();
    }

    private void Start()
    {
        FindTransform();
        OnChangeAmmo();
    }

    private void FindTransform()
    {
        fireTransform = transform.Find("fireTransform");
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 inDirection)
    {
        Bullet bullet = BulletPool.Instance.GetBullet();
        bullet.SetBulletInfo(fireTransform.position, inDirection);
    }

    public void Fire(Vector3 inDirection)
    {
        if (isReloading == true || isRating == true)
        {
            return;
        }
        if (HaveBullet() == false)
        {
            return;
        }
        //보내고 - 서버에서 한번더 확인해서 -> 생성
        //서버에게 총알만들기
#if MULTI
        SpawnBulletServerRpc(inDirection);
#else
        Bullet bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.LookRotation(inDirection));
        bullet.SetDirection(inDirection);
#endif
        if (OnFire != null)
        {
            OnFire.Invoke(transform.position);
        }

        isRating = true;
        restRateTime = rateTime;
        restBulletCount -= 1;
        OnChangeAmmo();
    }

    public void Reload()
    {
        if (isReloading == true || IsFullBullet() == true)
        {
            return;
        }
        isReloading = true;
        restReloadTime = equiptReloadTime;
    }

    public void EquiptItems(ItemBase[] inEquiptItems)
    {
        ResetEquiptValue();
        for (int i = 0; i < inEquiptItems.Length; i++)
        {
            AttachEquiptment(inEquiptItems[i]);
        }
    }

    private void ResetEquiptValue()
    {
        //장비 적용 값 default로 초기화
        equiptReloadTime = defaultReloadTime; //장전시간
        equiptRps = defaultRps; //초당 발사 갯수
        CalRateTime();
        equiptBulletCount = defaultBulletCount; //탄창 용량
        equiptFocusRegion = defaultFocusRegion;
    }

    private void AttachEquiptment(ItemBase inEquiptItem)
    {
        if (inEquiptItem == null || inEquiptItem.itemType == ItemMainType.None)
        {
            return;
        }

        ItemMainType mainType = inEquiptItem.itemType;
        if (mainType != ItemMainType.AttachMent)
        {
            Debug.LogError("장착물이 아닙니다.");
            return;
        }

        Stat targetStat = inEquiptItem.stat;
        int power = inEquiptItem.power;
        switch (targetStat)
        {
            case Stat.Focus:
                equiptFocusRegion = defaultFocusRegion - power;
                break;
            case Stat.AmmoSize:
                equiptBulletCount = defaultBulletCount + power;
                break;
            case Stat.ReloadTime:
                equiptReloadTime = defaultReloadTime - power;
                break;
            case Stat.Rps:
                equiptRps = defaultRps + power;
                CalRateTime();
                break;
            default:
                Debug.LogWarning("정의 되지 않은 스텟");
                break;
        }
    }

    private void Update()
    {
        CountReloadTime();
        CountRateTime();
    }

    private bool HaveBullet()
    {
        if (restBulletCount <= 0)
        {
            return false;
        }
        return true;
    }

    private bool IsFullBullet()
    {
        return equiptBulletCount == restBulletCount;
    }

    private void CountReloadTime()
    {
        if (isReloading == false)
        {
            return;
        }
        restReloadTime -= Time.deltaTime;
        if (restReloadTime <= 0)
        {
            isReloading = false;
            restBulletCount = equiptBulletCount;
            OnChangeAmmo();
            DoneRateTime();
        }
    }

    private void CountRateTime()
    {
        if (isReloading == true || isRating == false)
        {
            return;
        }
        restRateTime -= Time.deltaTime;
        if (restRateTime <= 0)
        {
            DoneRateTime();
        }
    }

    private void DoneRateTime()
    {
        isRating = false;
    }

    public void ChangeGunState(GunState inGunState)
    {
        if (CurrentGunState != inGunState)
        {
            Logger.Warning($"gunstateChanged :{CurrentGunState} -> {inGunState}");
            CurrentGunState = inGunState;
            switch (inGunState)
            {
                case GunState.None:
                    break;
                case GunState.Attack:
                    SetGunAnimatorTrigger(inGunState);
                    break;
                case GunState.Reload:
                    if (isStateLock)
                    {
                        return;
                    }
                    Reload();
                    SetGunAnimatorTrigger(inGunState);
                    StartCoroutine(ChangeGunStateCoroutine(CurrentGunState, 2f));
                    break;
                default:
                    Logger.Log("FatalErreo :: Tried State Change is Not Allowed");
                    break;
            }
        }
    }

    public IEnumerator ChangeGunStateCoroutine(GunState inState, float inDelay)
    {
        isStateLock = true;
        yield return new WaitForSeconds(inDelay);
        isStateLock = false;
    }
    public void SetGunAnimatorTrigger(GunState inState)
    {
        animator.SetTrigger(inState.ToString());
    }

    public void SetGunAnimatorBool(GunState inState, bool inBoolState)
    {
        animator.SetBool(inState.ToString(), inBoolState);
    }
    private void CalRateTime()
    {
        rateTime = 1f /equiptRps; //연사속도
    }

    private void OnChangeAmmo()
    {
        if (IsOwner)
        {
            OnChageAmmoUI?.Invoke(restBulletCount);
        }
        
    }
}
