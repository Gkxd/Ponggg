using UnityEngine;
using System.Collections;

public class MeleeAttack : MonoBehaviour
{
    [Range(30, 120)]
    public float angle;

    [Range(1, 3)]
    public float range;

    private float currentAngle;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private Vector3 lastPosition;

    void Start()
    {
        currentAngle = -0.5f * angle;
        meshFilter = GetComponent<MeshFilter>();

        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void FixedUpdate()
    {
        currentAngle = Mathf.Lerp(0.5f * angle, currentAngle, 50);
    }

    void Update()
    {

    }
}
