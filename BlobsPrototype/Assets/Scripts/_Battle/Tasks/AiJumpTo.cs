using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class AiJumpTo : Action {
	public SharedVector3 targetPosition;

	private float speed;
	private Actor actor;
	private Vector3 startPosition;
	private float startTime;
	private float height;
	private float totalDistance;
	// Use this for initialization
	public override void OnAwake() {
		actor = GetComponent<Actor>();
	}

	public override void OnStart() {
		startPosition = actor.transform.position;
		startTime = Time.time;
		totalDistance = Vector3.Distance(startPosition, targetPosition.Value);
			

		height = Mathf.Min(totalDistance * .5f, 8f);
		speed = 5f;
	}
	
	// Update is called once per frame
	public override TaskStatus OnUpdate() {
		float currentDuration = Time.time - startTime;
		float journeyFraction = currentDuration / totalDistance * speed;

		if(Vector3.Distance(actor.transform.position, targetPosition.Value)  < .1f)
			return TaskStatus.Success;
		
		Vector3 curPos = Vector3.Lerp(startPosition, targetPosition.Value, journeyFraction);
		curPos.y += height * Mathf.Sin(journeyFraction * Mathf.PI);
		actor.transform.position = curPos;
		return TaskStatus.Running;
	}
}
