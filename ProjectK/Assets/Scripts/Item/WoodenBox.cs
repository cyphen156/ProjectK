using Unity.Netcode;
using UnityEngine;

public class WoodenBox : NetworkBehaviour, ITakeDamage, ISpawnable
{
    private float hp;
    private float lifeTime;
    private uint ownerNetworkId;

    #region Unity Methods

    private void Awake()
    {
        hp = 300f;
        lifeTime = 20f;
    }

    #endregion

    private void Update()
    {
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
    private void CountLifeTime()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }

    public void TakeDamage(float inBulletDamage)
    {
        if (!IsHost)
        {
            return;
        }

        hp -= inBulletDamage;

        if (hp <= 0f)
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<NetworkObject>().Despawn();
        }
        // ««∞› ¿Ã∆Â∆Æ 
    }
}

