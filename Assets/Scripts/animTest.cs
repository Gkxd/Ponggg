using UnityEngine;
using System.Collections;

public class animTest : MonoBehaviour {

	Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		animator.SetBool ("run1", Input.GetKey (KeyCode.C));
		animator.SetBool ("run2", Input.GetKey (KeyCode.V));
		animator.SetBool ("run3", Input.GetKey (KeyCode.B));
	}
}
