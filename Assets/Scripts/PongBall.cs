using UnityEngine;
using System.Collections;

public class PongBall : MonoBehaviour
{

    public float speed;

    private new Rigidbody rigidbody;

    private Vector3 direction;

	public float angle;

    private bool hitLeft, hitTop, hitRight, hitBottom;

	void SetDirection()
	{
		direction = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle));
		direction.Normalize ();
	}

	float GetAngle()
	{
		float ang = Mathf.Atan (direction.y / direction.x);
		if (Mathf.Sign(direction.x) == -1){
			ang += Mathf.PI;
		}
		return ang;
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
			angle = GetAngle ();

			if (angle > Mathf.PI / 2 && angle < Mathf.PI * 3 / 2) {
				angle -= Mathf.PI / 16;
			} else {
				angle += Mathf.PI / 16;
			}

			SetDirection ();
		}
		if (Input.GetKeyDown ("down")) {
			angle = GetAngle ();

			if (angle > Mathf.PI / 2 && angle < Mathf.PI * 3 / 2) {
				angle += Mathf.PI / 16;
			} else {
				angle -= Mathf.PI / 16;
			}

			SetDirection ();
		}
	}
}
