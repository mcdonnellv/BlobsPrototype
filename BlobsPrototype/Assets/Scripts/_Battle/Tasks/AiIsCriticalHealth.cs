using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiIsCriticalHealth : Conditional {

	public SharedFloat criticalHealth;

	private ActorHealth health;

	public override void OnStart() {
		// cache the health component
		health = GetComponent<ActorHealth>();
	}

	public override TaskStatus OnUpdate () {
		if(health == null)
			return TaskStatus.Failure;

		float healthPercentage = health.health / health.startHealth;
		if(healthPercentage > criticalHealth.Value)
			return TaskStatus.Failure;

		return TaskStatus.Success;
	}
}
