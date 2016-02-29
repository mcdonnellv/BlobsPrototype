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
		private Actor actor;

		public override void OnAwake() {
			actor = gameObject.GetComponent<Actor>();
		}

		public override TaskStatus OnUpdate()
		{
			if(base.Target() == Vector3.zero || HasArrived()) {
				if(AiManager.IsLookingAt(actor, (Vector2)base.Target()) == false)
					AiManager.LookAtTarget(actor, (Vector2)base.Target());
				return TaskStatus.Success;
			}

			// check if behind me
			AiManager.AiMoveToDestination(actor, (Vector2)base.Target(), moveForce.Value, speed.Value, lookAtTarget.Value);
			return TaskStatus.Running;
		}

		bool HasArrived() {
			bool retval =  Mathf.Abs(transform.position.x - base.Target().x ) <= arriveDistance.Value;
			return retval;
		}

		// Draw the seeing radius
		public override void OnDrawGizmos() {
#if UNITY_EDITOR
			if (Owner == null)
				return;
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.forward, arriveDistance.Value);
			UnityEditor.Handles.color = oldColor;
#endif
		}
	}


}
