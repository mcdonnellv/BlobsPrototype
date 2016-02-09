using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsCriticalHealth : Conditional {

	private ActorHealth actorHealth;

	public override void OnAwake() {
		actorHealth = gameObject.GetComponent<ActorHealth>();
	}

	public override TaskStatus OnUpdate () {
		if(actorHealth == null)
			return TaskStatus.Failure;

		if(actorHealth.IsCriticalHealth())
			return TaskStatus.Success;

		return TaskStatus.Failure;
	}
}
