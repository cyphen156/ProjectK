using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float defaultLifeTime;
    private float lifeTime;
    private float speed;
    private float damage;
    private Vector3 direction;
    private NetworkVariable<bool> isActive = new NetworkVariable<bool>(false);

    private void Awake()
    {
        gameObject.SetActive(isActive.Value);
        defaultLifeTime = 5.0f;
        lifeTime = defaultLifeTime;
        speed = 14.0f;
        damage = 30f;



    }
    public override void OnNetworkSpawn()
    {
        isActive.OnValueChanged += OnActiveValueChange;
    }

    public void SetDirection(Vector3 inSpawnPosition, Vector3 inDirection)
    {
        //총에서 만들어서 호출
        transform.position = inSpawnPosition;
        direction = inDirection;
        lifeTime = defaultLifeTime;
        ActiveRpc(true);
    }

    [Rpc(SendTo.Everyone)]
    private void ActiveRpc(bool inActive)
    {
        gameObject.SetActive(inActive);
    }

    private void Update()
    {
        if(IsHost == false)
        {
            Debug.Log("호스트가 아니다");
        
        }

        if (IsServer == false)
        {
            Debug.Log("서버가 아니다");
    
        }

        if(IsClient == false)
        {
            Debug.Log("클라아님");
        }

        Move();
        CountLifeTime();
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
            InActive();
        }
    }

    private void OnActiveValueChange(bool inPreActive, bool inActive)
    {
        Debug.Log("바뀐다");
        gameObject.SetActive(inActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsHost)
        {
            ITakeDamage takeDamageObj = other.GetComponent<ITakeDamage>();
            //체력을 동기화하면 다른 클라이언트의 체력도 동기화가 된다. 
            //원하는건 그 타겟을 넘기고 싶은거 - 넷 오브젝트가 있을 것
            if (takeDamageObj != null)
            {
                takeDamageObj.TakeDamage(damage);
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    public void InActive()
    {
        if(IsHost == false)
        {
            Debug.Log("비활성화는 서버만보여주기");
            return;
        }
        ActiveRpc(false);
        BulletPool.Instance.Recycle(this);
    }

}
