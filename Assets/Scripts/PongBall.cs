using UnityEngine;
using System.Collections;

public class PongBall : MonoBehaviour
{

    public float speed;

    private new Rigidbody rigidbody;

    private Vector3 direction;

    private bool hitLeft, hitTop, hitRight, hitBottom;

    void Start()
    {
        direction = Random.insideUnitCircle.normalized;
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rigidbody.velocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        Reflect(collision.contacts[0].normal);
    }

    public void Reflect(Vector3 normal)
    {
        direction = Vector3.Reflect(direction, normal);
    }
}
