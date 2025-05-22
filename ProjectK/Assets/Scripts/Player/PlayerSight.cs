using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerSight : MonoBehaviour
{
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float angle;
        public float distance;

        public ViewCastInfo(bool inHit, Vector3 inPoint, float inAngle, float inDistance)
        {
            hit = inHit;
            point = inPoint;
            angle = inAngle;
            distance = inDistance;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 inPointA, Vector3 inPointB)
        {
            pointA = inPointA;
            pointB = inPointB;
        }
    }

    // Field of View
    [Header("Radious")]
    [SerializeField] private float baseViewRadius;
    [SerializeField] private float activeViewRadius;
    [SerializeField] private float crosshairRadius;

    [Header("Angle")]
    [SerializeField] private float baseViewAngle;
    [SerializeField] private float activeViewAngle;
    [SerializeField] private float crosshairAngle;

    [Header("Distance")]
    [SerializeField] private float viewDistance;

    [SerializeField] private float meshResolution;
    [SerializeField] private int edgeResolveIterations;
    [SerializeField] private float edgeDistanceThreshold;

    private Mesh viewMesh;
    [SerializeField] private MeshFilter meshFilter;

    private LayerMask targetMask;
    private LayerMask obstacleMask;
    private List<Transform> visibleTargets;

    #region Unity Methods   
    private void Awake()
    {
        baseViewRadius = 10f;
        activeViewRadius = 2f * baseViewRadius;
        baseViewAngle = 45f;
        activeViewAngle = baseViewAngle;
        viewDistance = activeViewRadius;

        meshResolution = 3f;
        edgeResolveIterations = 4;
        edgeDistanceThreshold = 0.5f;

        visibleTargets = new List<Transform>();
    }
    private void OnEnable()
    {
        PlayerController.OnCrosshairSizeChanged += UpdateCrosshairRadius;
    }

    private void OnDisable()
    {
        PlayerController.OnCrosshairSizeChanged -= UpdateCrosshairRadius;
    }

    private void Start()
    {
        viewMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = viewMesh;
        targetMask = LayerMask.GetMask("Player");
        obstacleMask = ~targetMask;
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }
    #endregion

    private ViewCastInfo ViewCast(float inGlobalAngle)
    {
        Vector3 direction = DirectionFromAngle(inGlobalAngle, true);
        Vector3 rayPoint = transform.position;
        rayPoint += 2 * Vector3.up;
        RaycastHit hit;
        if (Physics.Raycast(rayPoint, direction, out hit, activeViewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, inGlobalAngle);
        }
        else
        {
            return new ViewCastInfo(false, rayPoint + direction * activeViewRadius, activeViewRadius, inGlobalAngle);
        }
    }
    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVislbleTargets();
        }
    }

    // 방향 벡터 얻어오기
    public Vector3 DirectionFromAngle(float inAngle, bool isGlobal)
    {
        if (!isGlobal)
        {
            inAngle += transform.eulerAngles.y;
        }
        float rad = inAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
    }

    private void FindVislbleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, activeViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; ++i)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < activeViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.Clamp(Mathf.RoundToInt(meshResolution * activeViewAngle), 6, 500);
        float angleStep = activeViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; ++i)
        {
            float baseAngle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
            //float angle = baseAngle - activeViewAngle / 2 + angleStep * i;
            float angle = transform.eulerAngles.y - activeViewAngle / 2 + angleStep * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * 0.01f);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; ++i)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public Vector3 GetRandomSpreadDirection()
    {
        float forwardAngle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        float spreadAngle = Random.Range(-crosshairAngle, crosshairAngle);

        float finalAngle = forwardAngle + spreadAngle;
        return DirectionFromAngle(finalAngle, true).normalized;
    }

    private void UpdateCrosshairRadius(float inCrosshairRadius)
    {
        crosshairRadius = inCrosshairRadius;
        float spreadAngleRad = Mathf.Atan(crosshairRadius / viewDistance);
        crosshairAngle = spreadAngleRad * Mathf.Rad2Deg / 10;
    }
}