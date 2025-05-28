using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlayerSight : NetworkBehaviour
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
    private Vector3 sightOffset;

    #region Unity Methods   
    private void Awake()
    {
        baseViewRadius = 8f;
        activeViewRadius = 5f * baseViewRadius;
        baseViewAngle = 45f;
        activeViewAngle = baseViewAngle;
        viewDistance = activeViewRadius;

        meshResolution = 6f;
        edgeResolveIterations = 10;
        edgeDistanceThreshold = 0.5f;
        sightOffset = new Vector3(0f, 2f, 0f);
        visibleTargets = new List<Transform>();
    }
    private void OnEnable()
    {
        if (IsOwner)
        {
            PlayerController.OnCrosshairSizeChanged += UpdateCrosshairRadius;
        }
        
    }

    private void OnDisable()
    {
        if (IsOwner)
        {
            PlayerController.OnCrosshairSizeChanged -= UpdateCrosshairRadius;
        }
    }

    private void Start()
    {
        viewMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = viewMesh;
        targetMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Wall", "Deploy", "Ground");
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    private void LateUpdate()
    {
        if (GetComponent<NetworkBehaviour>().IsOwner)
        {
            DrawFieldOfView();
            DrawSelfCircleSight();
        }
    }
    #endregion

    private ViewCastInfo ViewCast(float inGlobalAngle)
    {
        Vector3 direction = DirectionFromAngle(inGlobalAngle, true);
        Vector3 rayPoint = transform.position + sightOffset;
        RaycastHit hit;

        float maxDist = activeViewRadius;
        float maxAllowedDist = maxDist * 1.05f;

        if (Physics.Raycast(rayPoint, direction, out hit, maxDist, obstacleMask))
        {
            float clampedDist = Mathf.Clamp(hit.distance, 0.1f, maxAllowedDist);
            Vector3 point = rayPoint + direction * clampedDist;
            return new ViewCastInfo(true, point, inGlobalAngle, clampedDist);
        }
        else
        {
            return new ViewCastInfo(false, rayPoint + direction * activeViewRadius, inGlobalAngle, activeViewRadius);
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
        return new Vector3(Mathf.Sin(inAngle * Mathf.Deg2Rad), 0, Mathf.Cos(inAngle * Mathf.Deg2Rad));
    }

    private void FindVislbleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position + sightOffset, activeViewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; ++i)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position + sightOffset - transform.position + sightOffset).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < activeViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position + sightOffset, target.position + sightOffset);

                if (!Physics.Raycast(transform.position + sightOffset, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    private void DrawSelfCircleSight(int segments = 36)
    {
        Vector3 origin = transform.position + sightOffset;

        Vector3[] vertices = new Vector3[segments + 1];
        int[] triangles = new int[segments * 3];

        vertices[0] = transform.InverseTransformPoint(origin); // 반드시 로컬 좌표로

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Vector3 worldPoint = origin + dir * baseViewRadius;
            vertices[i + 1] = transform.InverseTransformPoint(worldPoint);
        }

        for (int i = 0; i < segments; i++)
        {
            int tri = i * 3;
            triangles[tri] = 0;
            triangles[tri + 1] = i + 1;
            triangles[tri + 2] = (i + 2 > segments) ? 1 : i + 2;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Graphics.DrawMesh(mesh, transform.localToWorldMatrix, renderer.sharedMaterial, gameObject.layer);
    }
    private void DrawFieldOfView()
    {
        Vector3 origin = transform.position + sightOffset;
        int stepCount = Mathf.RoundToInt(activeViewAngle * meshResolution);
        float angleStep = activeViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        float baseAngle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;

        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; ++i)
        {
            float angle = baseAngle - activeViewAngle / 2f + angleStep * i;
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

        vertices[0] = transform.InverseTransformPoint(origin);
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            Vector3 dir = (viewPoints[i] - origin).normalized;
            Vector3 adjusted = viewPoints[i] + dir * 0.01f;
            vertices[i + 1] = transform.InverseTransformPoint(adjusted);
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