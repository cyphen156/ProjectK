using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletPool : NetworkBehaviour
{
    public static BulletPool Instance;
    private int makeCount;
    [SerializeField] private Bullet bulletPrefab;
    private Queue<Bullet> pool;

    private void Awake()
    {
        makeCount = 30;
        Instance = this;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += MakePoolObject;
    }

    private void MakePoolObject()
    {
       // Debug.Log("풀만들기 시작");
        if(IsServer == false)
        {
            return;
        }
        pool = new() ;
        MakeBullet();

    }

    private void MakeBullet()
    {
        for (int i = 0; i < makeCount; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab, transform);
            bullet.GetComponent<NetworkObject>().Spawn();
            bullet.ControlActive(false);
            pool.Enqueue(bullet);
        }
    }

    public void Recycle(Bullet inBullet)
    {
        pool.Enqueue(inBullet);
       // Debug.Log("재활용 " + pool.Count);
    }

    public Bullet GetBullet()
    {
      //  Debug.Log(pool.Count + "에서 빼기");
        if(pool.Count == 1)
        {
            MakeBullet();
        }
        return pool.Dequeue();
    }
}
