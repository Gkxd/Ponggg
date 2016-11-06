using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MeleeAttack : NetworkBehaviour
{
    [Range(30, 120)]
    public float angle;

    [Range(1, 10)]
    public float range;

    [Range(0, 3)]
    public float strength;

    public AnimationCurve angleSpeed;
    
    public Transform target { get; set; }
    public bool clockWise { get; set; }

    private float startTime;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Material material;

    private Vector3 startRay;
    private Vector3 endRay;
    private Vector3 startTangent;
    private Vector3 endTangent;

    private Stack<Vector2> uvList;
    private List<Vector3> vertexList;
    private List<int> triangleList;

    private bool hit;

    void Start()
    {
        startTime = Time.time;

        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        material = GetComponent<Renderer>().material;

        uvList = new Stack<Vector2>();
        vertexList = new List<Vector3>();
        triangleList = new List<int>();

        if (clockWise)
        {
            startRay = Quaternion.Euler(0, 0, 0.5f * angle) * Vector3.right;
            endRay = Quaternion.Euler(0, 0, -0.5f * angle) * Vector3.right;
        }
        else
        {
            startRay = Quaternion.Euler(0, 0, -0.5f * angle) * Vector3.right;
            endRay = Quaternion.Euler(0, 0, 0.5f * angle) * Vector3.right;
        }

        Vector3 cross = Vector3.Cross(startRay, endRay);
        startTangent = Vector3.Cross(cross, startRay);
        endTangent = Vector3.Cross(cross, endRay);
    }

    void Update()
    {
        float progress = (Time.time - startTime) * 3;
        if (progress < 1)
        {
            Vector3 currentRay = Vector3.Slerp(startRay, endRay, angleSpeed.Evaluate(progress));
            Vector3 currentTangent = Vector3.Slerp(startTangent, endTangent, angleSpeed.Evaluate(progress));

            Vector3 currentWorldRay = transform.TransformDirection(currentRay);
            Vector3 currentWorldTangent = transform.TransformDirection(currentTangent);

            if (GameState.Ball != null)
            {
                Vector3 ballRay = GameState.Ball.transform.position - target.position;
                if (!hit && Vector3.Angle(currentWorldRay, ballRay) < 15 && ballRay.sqrMagnitude <= range * range)
                {
                    GameState.Ball.direction = currentWorldTangent;
                    GameState.Ball.speed += strength;
                    hit = true;
                }
            }

            Vector3 basePoint = transform.InverseTransformPoint(target.position);
            Vector3 endPoint = basePoint + currentRay * range;

            vertexList.Add(basePoint);
            vertexList.Add(endPoint);

            uvList.Push(new Vector3(1 - progress, 0));
            uvList.Push(new Vector3(0.5f - progress, 0));

            int n = vertexList.Count;
            if (n > 2)
            {
                triangleList.Add(n - 4); // prev base
                triangleList.Add(n - 3); // prev end
                triangleList.Add(n - 1); // this end

                triangleList.Add(n - 1); // this end
                triangleList.Add(n - 2); // this base
                triangleList.Add(n - 4); // prev base
            }

            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            mesh.uv = uvList.ToArray();
        }
        else
        {
            progress -= 1;

            Color c = material.color;
            c.a = Mathf.Lerp(1, 0, progress);
            material.color = c;
            if (progress > 1)
            {
                Destroy(gameObject);
            }
        }
    }
}
