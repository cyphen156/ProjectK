using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform fireTransform;
  
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

    public void Fire()
    {
        Debug.Log("총 발사");
        Vector3 direction = fireTransform.position - playerTransform.position;
        GameObject bullet = Instantiate(bulletPrefab, fireTransform);
    }

}
