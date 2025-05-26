using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Bullet : NetworkBehaviour, ISpawnable
{
    private float defaultLifeTime;
    private float lifeTime;
    private float speed;
    private float damage;
    private Vector3 direction;
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>();
    private uint ownerNetworkId;

    private void Awake()
    {
        defaultLifeTime = 5.0f;
        lifeTime = defaultLifeTime;
        speed = 14.0f;
        damage = 30f;
    }


    protected override void InternalOnNetworkPostSpawn()
    {
        //Debug.Log("스폰 되었다.2");
        //OnNetworkSpawn - 스폰될떄
        //Internal(지금함수) - 스폰되고 나서 
        if (IsServer == false)
        {
            //OnNetworkSpawn에서 비활성화 해버리면 NetWorkObejct가 Null이 뜸
            gameObject.SetActive(isActive.Value);
        }
    }

    public void SetBulletInfo(Vector3 inSpawnPosition, Vector3 inDirection)
    {
        //총에서 만들어서 호출
        ControlActive(true); //총알 쐈다
        //transform.position = inSpawnPosition;
        var netTransform = GetComponent<NetworkTransform>();
        //위치를 스폰위치로 순간이동 시켜서 동기화
        netTransform.Teleport(inSpawnPosition, Quaternion.identity, transform.localScale);
        direction = inDirection;
        lifeTime = defaultLifeTime;
    }

    #region 총알 행동
    private void Update()
    {
        if (IsHost == false)
        {
            return;
        }

        Move();
        CountLifeTime();
    }

    public void SetOwner(uint inOwnerId)
    {
        ownerNetworkId = inOwnerId;
    }

    public uint GetOwner()
    {
        return ownerNetworkId;
    }

    private void Move()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void CountLifeTime()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            //Destroy(gameObject);
            BulletOff();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsHost)
        {
            // 데미지를 줄 수 있다면
            if (other.TryGetComponent<ITakeDamage>(out var target))
            {
                bool isSelfOwnedSpawnedObject = false;

                if (other.TryGetComponent<ISpawnable>(out var spawnObj))
                {
                    if (spawnObj.GetOwner() == ownerNetworkId)
                    {
                        isSelfOwnedSpawnedObject = true;
                    }
                }

                if (!isSelfOwnedSpawnedObject)
                {
                    target.TakeDamage(damage);
                }
            }

            GetComponent<NetworkObject>().Despawn();
        }
    }
    #endregion


    #region 총알 활성화 동기화
    private void BulletOff()
    {
        if (IsHost == false)
        {
           return;
        }
        ControlActive(false);
        BulletPool.Instance.Recycle(this);
    }

    public void ControlActive(bool inActive)
    {
        if (IsHost == false)
        {
            return;
        }
        isActive.Value = inActive;
        ActiveRpc(inActive);
    }

    [Rpc(SendTo.Everyone)]
    private void ActiveRpc(bool inActive)
    {
        gameObject.SetActive(inActive);
    }
    #endregion
}
