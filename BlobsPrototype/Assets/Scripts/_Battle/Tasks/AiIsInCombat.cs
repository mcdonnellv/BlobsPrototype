using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Conditional task which determines if the target object is in combat")]
public class AiIsInCombat : Conditional
{
	public SharedGameObject aggroTarget;
	private GameObject localTarget;
	private ActorHealth health;


	public override void OnAwake() {
	}

	public override void OnStart() {
		if(aggroTarget.Value != null)
			health = aggroTarget.Value.GetComponent<ActorHealth>();
	}

	// OnUpdate will return success if the object is still alive and failure if it not
	public override TaskStatus OnUpdate() {
		if(health != null && health.IsAlive())
			return TaskStatus.Success;
		
		return TaskStatus.Failure;
	}
}

