using UnityEngine;
using System;
using System.Collections;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class Attack : Action {

		public SharedString attackTrigger;
		private float startTime;
		private float attackDelay;
		private Animator animator;

		public override void OnAwake() {
			animator = GetComponent<Animator>();
		}

		public override void OnStart() {
			// the target may be null if it has been destoryed. In that case set the health to null and return
			if (animator == null) 
				return;
	
			startTime = Time.time;
			attackDelay = 0f;

			RuntimeAnimatorController ac = animator.runtimeAnimatorController;
			for(int i = 0; i<ac.animationClips.Length; i++)                 //For all animations{
				if(ac.animationClips[i].name == attackTrigger.Value)        //If it has the same name as your clip
					attackDelay = ac.animationClips[i].length;
			
			// begin the attack
			animator.SetTrigger(attackTrigger.Value);
		}


		public override TaskStatus OnUpdate () {
			if (animator == null || attackDelay <= 0) 
				return TaskStatus.Failure;

			// succeed when the animation is over
			if (startTime + attackDelay < Time.time)
				return TaskStatus.Success;

			return TaskStatus.Running;
		}

	}
}

