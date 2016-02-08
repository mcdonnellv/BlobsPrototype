using UnityEngine;
using System;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class IsAttackCooldownDone : Conditional {

		public SharedString attackTime;

		public override TaskStatus OnUpdate () {

			if(attackTime == null ||  string.IsNullOrEmpty(attackTime.Value))
				return TaskStatus.Success;
			
			TimeSpan cooldown = new TimeSpan(0,0,1);
			DateTime time = DateTime.Parse(attackTime.Value);
			time = time + cooldown;
			if(time < DateTime.Now)
				return TaskStatus.Success;
			
			return TaskStatus.Failure;
		}
	}

}
