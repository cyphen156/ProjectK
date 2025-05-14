using UnityEngine;



public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform fireTransform;

    private bool isReloading;
    private float defaultReloadTime; //기본 탄창 채우는 시간
    private float restReloadTime; //채우기 까지 남은 시간

    private int defaultBulletCount; //기본 총알 수
    private int restBulletCount; //남은 총알 수

    private bool isRating; //연사속도대기
    private float defaultRateTime;
    private float restRateTime;

    private void Awake()
    {
        defaultReloadTime = 2f; //장전시간
        restReloadTime = 0f;

        defaultRateTime = 0.2f; //연사속도
        restRateTime = 0f;

        defaultBulletCount = 15;
        restBulletCount = defaultBulletCount;

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

        //플레이어 위치
        Transform findTransform = transform;
        while (findTransform != null)
        {
            if (findTransform.name == "Player")
            {
                playerTransform = findTransform;
                break;
            }

            findTransform = findTransform.parent;
        }
        if (playerTransform == null)
        {
            Debug.LogError("플레이어 트랜스폼 찾지 못했음");
        }
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
}
