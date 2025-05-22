using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    private float defaultLifeTime;
    private float lifeTime;
    private float speed;
    private float damage;
    private Vector3 direction;
    private NetworkVariable<bool> isActive;

    private void Awake()
    {
        isActive = new NetworkVariable<bool>();
        isActive.Value = false;
        gameObject.SetActive(isActive.Value);
        isActive.OnValueChanged += OnActiveValueChange;
        defaultLifeTime = 5.0f;
        lifeTime = defaultLifeTime;
        speed = 14.0f;
        damage = 30f;



    }

    public void SetDirection(Vector3 inDirection)
    {
        //총에서 만들어서 호출
        direction = inDirection;
        lifeTime = defaultLifeTime;
        isActive.Value = true;
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
        gameObject.SetActive(inActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        ITakeDamage takeDamageObj = other.GetComponent<ITakeDamage>();
        if (takeDamageObj != null)
        {
            takeDamageObj.TakeDamage(damage);
            InActive();
        }
    }

    public void InActive()
    {
        isActive.Value = false;
        BulletPool.Instance.Recycle(this);
    }
}
