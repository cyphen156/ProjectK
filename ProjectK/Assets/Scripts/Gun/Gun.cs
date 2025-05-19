using System;
using UnityEngine;



public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform fireTransform;

    public static event Action<Vector3> OnFire;
    private bool isReloading;
    private float defaultReloadTime; //기본 탄창 채우는 시간
    private float equiptReloadTime; //스텟 적용
    private float restReloadTime; //채우기 까지 남은 시간

    private int defaultBulletCount; //기본 총알 수
    private int equiptBulletCount; //스텟 적용
    private int restBulletCount; //남은 총알 수

    private int defaultRps; //스텟 적용
    private int equiptRps; //1초당 총알 발사 갯수
    
    private bool isRating; //연사속도대기
    private float rateTime;
    private float restRateTime;

    private float defaultFocusRegion; //탄 밀집도 : 클수록 퍼진다.
    private float equiptFocusRegion;

    public static event Action<int> OnChageAmmoUI;


    private void Awake()
    {
        defaultReloadTime = 2f;
        restReloadTime = 0f;

        defaultRps = 15;
        restRateTime = 0f;

        defaultBulletCount = 30;
        restBulletCount = defaultBulletCount;

        defaultFocusRegion = 1f; //조준 반경
  
        isRating = false;
        isReloading = false;
        ResetEquiptValue();
    }

    private void Start()
    {
        FindTransform();

        OnChageAmmoUI?.Invoke(restBulletCount);
    }
    
    private void FindTransform()
    {
        //총알 생성될 위치 
        fireTransform = transform.Find("fireTransform");
    }

    public void Fire(Vector3 inDirection)
    {
       // Debug.Log("총 발사 요청");
        if (isReloading == true || isRating == true )
        {
            return;
        }
        if(HaveBullet() == false)
        {
            return;
        }
        Bullet bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.LookRotation(inDirection));
        bullet.SetDirection(inDirection);

        if(OnFire != null)
        {
            OnFire.Invoke(transform.position);
        }

        isRating = true;
        restRateTime = rateTime;
        restBulletCount -= 1;

        OnChageAmmoUI?.Invoke(restBulletCount);
    }

    public void Reload()
    {
        if(isReloading == true || IsFullBullet() == true)
        {
            //장전 중이거나 풀탄창이면 리로드 안함
            return;
        }
        isReloading = true;
        restReloadTime = equiptReloadTime;
    }

    public void EquiptItems(ItemBase[] inEquiptItems)
    {
        ResetEquiptValue();
        //착용 중인 아이템 적용
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
        if(inEquiptItem == null || inEquiptItem.itemType == ItemMainType.None)
        {
            return;
        }

        ItemMainType mainType = inEquiptItem.itemType;
        if(mainType != ItemMainType.AttachMent)
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
        if(restBulletCount <= 0)
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
        if(isReloading == false)
        {
            return;
        }
        restReloadTime -= Time.deltaTime;
        if(restReloadTime <= 0)
        {
            isReloading = false;
            restBulletCount = equiptBulletCount;

            OnChageAmmoUI?.Invoke(restBulletCount);

            DoneRateTime();
        }
    }

    private void CountRateTime()
    {
        //장전중이거나, 연사대기가 아니면 종료
        if(isReloading == true || isRating == false)
        {
            return;
        }
        restRateTime -= Time.deltaTime;
        if(restRateTime <= 0)
        {
            DoneRateTime();
        }
    }

    private void DoneRateTime()
    {
        isRating = false;
    }

    private void CalRateTime()
    {
        rateTime = 1f /equiptRps; //연사속도
    }

  
}
