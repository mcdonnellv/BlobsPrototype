using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

	// Update is called once per frame
	void Update () 
	{
		if (target)
		{ 
			Camera camera = GetComponent<Camera>();
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			destination =  new Vector3(destination.x,0,destination.z);
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}

	}

//	public float interpVelocity;
//	public float minDistance;
//	public float followDistance;
//	public GameObject target;
//	public Vector3 offset;
//	Vector3 targetPos;
//	// Use this for initialization
//	void Start () {
//		targetPos = transform.position;
//	}
//
//	// Update is called once per frame
//	void FixedUpdate () {
//		if (target)
//		{
//			Vector3 posNoZ = transform.position;
//			posNoZ.z = target.transform.position.z;
//
//			Vector3 targetDirection = (target.transform.position - posNoZ);
//			targetDirection = new Vector3(targetDirection.x,0,targetDirection.z);
//			interpVelocity = targetDirection.magnitude * 5f;
//
//			targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime); 
//
//			transform.position = Vector3.Lerp( transform.position, targetPos + offset, 0.25f);
//		}
//	}
}