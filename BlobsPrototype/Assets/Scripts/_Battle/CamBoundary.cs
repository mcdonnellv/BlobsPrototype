using UnityEngine;
using System.Collections;

public class CamBoundary : MonoBehaviour {
	public float speed = 5f;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rb.velocity = new Vector2(speed,0f);
	}
}
