using UnityEngine;
using System.Collections;

public class TwoAttacks : MonoBehaviour {
	
	public GameObject attack1;
	public GameObject attack2;
	public float verticalSeparation;

	void Start () {
		Vector3 pos1 = new Vector3 (transform.position.x, transform.position.y + verticalSeparation, transform.position.z);
		Vector3 pos2 = new Vector3 (transform.position.x, transform.position.y - verticalSeparation, transform.position.z);

		GameObject mov1 = (GameObject)Instantiate (attack1, pos1, Quaternion.FromToRotation(Vector3.right,Vector3.right));
		GameObject mov2 = (GameObject)Instantiate (attack2, pos2, Quaternion.FromToRotation(Vector3.right,Vector3.right));
	}

	void Update () {
		
	}
}
