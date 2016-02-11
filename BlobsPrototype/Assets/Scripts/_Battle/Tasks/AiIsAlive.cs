using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Conditional task which determines if the target object is still alive")]
public class AiIsAlive : Conditional
{
	public SharedGameObject target;
	public SharedBool checkSelf;

	private ActorHealth health;
	private GameObject localTarget;


	public override void OnAwake() {
	}

	public override void OnStart() {
		if(checkSelf.Value)
			localTarget = gameObject;
		else
			localTarget = target.Value;
		if(localTarget == null)
			return;
		
		// cache the health component
		health = localTarget.GetComponent<ActorHealth>();
	}

	// OnUpdate will return success if the object is still alive and failure if it not
	public override TaskStatus OnUpdate() {
		if (health != null && health.IsAlive())
			return TaskStatus.Success;
		
		return TaskStatus.Failure;
	}
}

