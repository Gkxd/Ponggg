using UnityEngine;
using System.Collections;

public class PongBall : MonoBehaviour
{

    public float speed { get; set; }
    public Vector3 direction { get; set; }

    private new Rigidbody rigidbody;

    void Start()
    {
        speed = 5;
        direction = Random.insideUnitCircle.normalized;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
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
