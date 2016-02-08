using UnityEngine;
using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class Attack : Action {

		public SharedGameObject target;
		public SharedString attackTime;
		public float attackDamage = 1;

		public override TaskStatus OnUpdate () {
			if(target.Value == null) 
				return TaskStatus.Failure;

			Actor targetActor = target.Value.gameObject.GetComponent<Actor>();

			if (targetActor == null)
				return TaskStatus.Failure;

			if(targetActor.health <= 0)
				return TaskStatus.Failure;

			targetActor.health -= attackDamage;
			attackTime.Value = System.DateTime.Now.ToString();

			return TaskStatus.Success;
		}
	}
}

