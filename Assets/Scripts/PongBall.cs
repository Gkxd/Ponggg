using UnityEngine;
using System.Collections;

public class PongBall : MonoBehaviour
{

    public float speed { get; set; }
    public Vector3 direction { get; set; }

    private new Rigidbody rigidbody;
    private float startTime;

    void Start()
    {
        speed = 5;
        direction = Random.insideUnitCircle.normalized;
        rigidbody = GetComponent<Rigidbody>();

        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > 3)
        {
            rigidbody.velocity = direction * speed;
        }
        else
        {

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (transform.position.x > 8.5f) // Player 0 score
        {
            GameState.Ball = null;
            Destroy(gameObject);
            GameState.SpawnBall();
            GameState.Player1.MissedPongBall();
        }
        else if (transform.position.x < -8.5f) // Player 1 score
        {
            GameState.Ball = null;
            Destroy(gameObject);
            GameState.SpawnBall();
            GameState.Player0.MissedPongBall();
        }
        else
        {
            Reflect(collision.contacts[0].normal);
        }
    }

    public void Reflect(Vector3 normal)
    {
        direction = Vector3.Reflect(direction, normal);
    }
}
