using UnityEngine;
using System.Collections;

public class BulletWiggle : MonoBehaviour {
	
	[Tooltip("The maximum magnitude of the wiggle")]
	public float wiggleRange;

	[Tooltip ("The frequency of the wiggle")]
	public float wiggleFrequency;

	int timer;
	float mag, ang, xchangemax, ychangemax;
	float xchange, ychange;
	new Vector3 originalVelocity;

	void Start () {

		originalVelocity = GetComponent<Rigidbody> ().velocity;

		timer = 0;

		mag = Mathf.Sqrt (Mathf.Pow(originalVelocity.x,2) + Mathf.Pow(originalVelocity.y,2));
		ang = Mathf.Acos (originalVelocity.x / mag);

		xchangemax = wiggleRange * Mathf.Cos (Mathf.PI / 2 + ang);
		ychangemax = wiggleRange * Mathf.Sin (Mathf.PI / 2 + ang);

	}
	
	void Update () {
		xchange = xchangemax * Mathf.Sin (timer * wiggleFrequency);
		ychange = ychangemax * Mathf.Sin (timer * wiggleFrequency);

		GetComponent<Rigidbody> ().velocity = new Vector3 (originalVelocity.x + xchange, originalVelocity.y + ychange, 0);
		timer = (timer + 1) % 360;
	}
}
