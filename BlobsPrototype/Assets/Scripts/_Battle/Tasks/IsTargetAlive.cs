using UnityEngine;


namespace BehaviorDesigner.Runtime.Tasks
{
	public class IsTargetAlive : Conditional {

		public SharedGameObject target;

		public override TaskStatus OnUpdate () {
			if(target.Value == null) 
				return TaskStatus.Failure;

			Actor targetActor = target.Value.gameObject.GetComponent<Actor>();

			if (targetActor == null)
				return TaskStatus.Failure;

			if(targetActor.health <= 0)
				return TaskStatus.Failure;

			return TaskStatus.Success;
		}
	}
}

