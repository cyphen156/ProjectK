using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Netcode;
public class BulletPool : NetworkBehaviour
{
    public static BulletPool Instance;
    public int MakeCount = 50;
    [SerializeField] private Bullet bulletPrefab;
    private Queue<Bullet> pool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += MakePoolObject;
    }

    private void MakePoolObject()
    {
        Debug.Log("풀만들기 시작");
        if(IsServer == false)
        {
            return;
        }
        Debug.Log("난 서버");
        pool = new() ;
        for (int i = 0; i < MakeCount; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab, transform);
            bullet.GetComponent<NetworkObject>().Spawn();
            pool.Enqueue(bullet);
        }

    }

    public void Recycle(Bullet inBullet)
    {
        
        pool.Enqueue(inBullet);
        Debug.Log("재활용 " + pool.Count);
    }

    public Bullet GetBullet()
    {
        Debug.Log(pool.Count + "에서 빼기");
        return pool.Dequeue();
    }
}
