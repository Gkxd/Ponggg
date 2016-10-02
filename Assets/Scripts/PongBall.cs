using UnityEngine;
using System.Collections;

public class PongBall : MonoBehaviour
{

    public float speed;

	public float angle;

    private new Rigidbody rigidbody;

    private Vector3 direction;

    private bool hitLeft, hitTop, hitRight, hitBottom;

	void SetDirection()
	{
		direction = new Vector3 (Mathf.Sin (angle), Mathf.Cos (angle));
		direction.Normalize ();
	}

    void Start()
    {
		angle = Random.value * 2 * Mathf.PI;
		SetDirection ();
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

	void Update(){
		if (Input.GetKeyDown ("up")) {
			angle += Mathf.PI / 32;
			SetDirection ();
		}
	}
}
