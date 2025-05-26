using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Granade : NetworkBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    private Rigidbody rb;

    private float damage;
    private float explosionRadius;
    private float explosionDelay;

    private LayerMask obstacleMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damage = 60f;
        explosionRadius = 5f;
        explosionDelay = 5f;
        obstacleMask = LayerMask.GetMask("Player", "Deploy");
        explosionEffectPrefab = Resources.Load<GameObject>("Prefabs/Effect/ExplosionEffect");
    }

    public void Launch(Vector3 start, Vector3 target, float launchAngle = 60f)
    {
        transform.position = start;

        Vector3 dir = target - start;
        float h = dir.y;
        dir.y = 0;
        float distance = dir.magnitude;
        float radianAngle = launchAngle * Mathf.Deg2Rad;
        dir.y = distance * Mathf.Tan(radianAngle);
        distance += h / Mathf.Tan(radianAngle);

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * radianAngle));
        Vector3 velocityVec = velocity * dir.normalized;

        rb.linearVelocity = velocityVec;

        // 5초 뒤에는 무조건 폭발함
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        if (IsHost)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, obstacleMask);
        foreach (Collider hit in hitColliders)
        {
            ITakeDamage Target = hit.GetComponent<ITakeDamage>();
            if (Target != null)
            {
                Target.TakeDamage(damage);
            }
        }
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        GetComponent<NetworkObject>().Despawn();
    }
}
