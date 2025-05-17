using System;
using UnityEngine;



public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform fireTransform;

    public static event Action<Vector3> OnFire;
    private bool isReloading;
    private float defaultReloadTime; //기본 탄창 채우는 시간
    private float restReloadTime; //채우기 까지 남은 시간

    private int defaultBulletCount; //기본 총알 수
    private int restBulletCount; //남은 총알 수

    private int defaultRps; //1초당 총알 발사 갯수

    private bool isRating; //연사속도대기
    private float defaultRateTime;
    private float restRateTime;

    private float focusRegion; //탄 밀집도 : 클수록 퍼진다.

    private void Awake()
    {
        defaultReloadTime = 2f; //장전시간
        restReloadTime = 0f;

        defaultRps = 15; //초당 발사 갯수
        CalRateTime();
        restRateTime = 0f;

        defaultBulletCount = 15; //탄창 용량
        restBulletCount = defaultBulletCount;

        focusRegion = 1f; //조준 반경

        isRating = false;
        isReloading = false;
    }

    private void Start()
    {
        FindTransform();
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
        //  Debug.Log("총 발사");
        Debug.Log(inDirection);
        Bullet bullet = Instantiate(bulletPrefab, fireTransform.position, Quaternion.identity);
        bullet.SetDirection(inDirection);

        if(OnFire != null)
        {
            OnFire.Invoke(transform.position);
        }

        isRating = true;
        restRateTime = defaultRateTime;
        restBulletCount -= 1;
    }

    public void Reload()
    {
        if(isReloading == true || IsFullBullet() == true)
        {
            //장전 중이거나 풀탄창이면 리로드 안함
            return;
        }
        isReloading = true;
        restReloadTime = defaultReloadTime;
    }

    public void AttachEquiptment(ItemBase inEquiptItem)
    {
        if(inEquiptItem == null)
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
                focusRegion -= power;
                break;
            case Stat.AmmoSize:
                defaultBulletCount += power;
                break;
            case Stat.ReloadTime:
                defaultReloadTime -= power;
                break;
            case Stat.Rps:
                defaultRateTime += power;
                CalRateTime();
                break;
        }
    }

    public void DetachEquiptment(ItemBase inEquiptItem)
    {
        if(inEquiptItem == null)
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
                focusRegion += power;
                break;
            case Stat.AmmoSize:
                defaultBulletCount -= power;
                break;
            case Stat.ReloadTime:
                defaultReloadTime += power;
                break;
            case Stat.Rps:
                defaultRateTime -= power;
                CalRateTime();
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
        return defaultBulletCount == restBulletCount;
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
            restBulletCount = defaultBulletCount;
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
        defaultRateTime = defaultRps / 60f; //연사속도
    }
}
