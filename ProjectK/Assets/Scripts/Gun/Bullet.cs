using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float defaultLifeTime;
    private float lifeTime;
    private float speed;
    private float damage;
    private Vector3 direction;

    private void Awake()
    {
        //변수 초기값 
        defaultLifeTime = 5.0f;
        lifeTime = defaultLifeTime;
        speed = 14.0f;
        damage = 30f;
    }

    public void SetDirection(Vector3 inDirection)
    {
        //총에서 만들어서 호출
        direction = inDirection;
    }

    private void Update()
    {
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
        if(lifeTime <= 0)
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
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

}
