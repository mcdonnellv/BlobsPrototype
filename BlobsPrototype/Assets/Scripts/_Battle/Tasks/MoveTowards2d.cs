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
			if(base.Target() == Vector3.zero || HasArrived()) {
				return TaskStatus.Success;
			}

			AiManager.MoveToDestination(transform, rigidBody, (Vector2)base.Target(), moveForce.Value, speed.Value, lookAtTarget.Value, "Walk");
			return TaskStatus.Running;
		}

		bool HasArrived() {
			bool retval =  Mathf.Abs(transform.position.x - base.Target().x) <= arriveDistance.Value;
			return retval;
		}
	}
}
