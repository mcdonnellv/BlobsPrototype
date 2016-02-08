using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
	[TaskDescription("Move towards the specified position. The position can either be specified by a transform or position. If the transform " +
	                 "is used then the position will not be used.")]
	[TaskCategory("Movement")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/Movement/documentation.php?id=1")]
	[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
	public class MoveTowards2d : MoveTowards {

		public SharedFloat moveForce = 100;
		private Rigidbody2D rigidBody;

		public override void OnAwake() {
			rigidBody = gameObject.GetComponent<Rigidbody2D>();
		}

		public override TaskStatus OnUpdate()
		{
			Vector2 destTarget = (Vector2)base.Target();
			if(transform.position.x - destTarget.x <= arriveDistance.Value)
				return TaskStatus.Success;

			AiManager.MoveToDestination(transform, rigidBody, destTarget, moveForce.Value, speed.Value, lookAtTarget.Value);
			return TaskStatus.Running;
		}
	}
}
