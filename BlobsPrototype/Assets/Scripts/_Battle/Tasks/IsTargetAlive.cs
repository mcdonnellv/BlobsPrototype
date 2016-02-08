using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Conditional task which determines if the target object is still alive")]
	public class IsTargetAlive : Conditional
	{
		[Tooltip("The target object that we are interested in to determine if it is still alive")]
		public SharedGameObject target;
		private ActorHealth health;
		
		public override void OnStart() {
			// the target may be null if it has been destoryed. In that case set the health to null and return
			if (target.Value == null) {
				health = null;
				return;
			}
			// cache the health component
			health = target.Value.GetComponent<ActorHealth>();
		}
		
		// OnUpdate will return success if the object is still alive and failure if it not
		public override TaskStatus OnUpdate() {
			if (health != null && health.Amount > 0) {
				return TaskStatus.Success;
			}
			return TaskStatus.Failure;
		}
	}
}

