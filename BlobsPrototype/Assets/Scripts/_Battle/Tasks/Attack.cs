using UnityEngine;
using System;
using System.Collections;

namespace BehaviorDesigner.Runtime.Tasks
{
	public class Attack : Action {

		public SharedGameObject target;
		public SharedFloat range;
		public SharedFloat damage;

		private ActorHealth health;
		private float startTime;
		private float hitboxSpawnDelay;
		private float attackDelay;
		private bool hitBoxSpawned;


		public override void OnStart() {
			// the target may be null if it has been destoryed. In that case set the health to null and return
			if (target.Value == null) {
				health = null;
				return;
			}

			// cache the health component
			health = target.Value.GetComponent<ActorHealth>();

			attackDelay = 0f;
			Animator animator = GetComponent<Animator>();
			AnimationClip attackClip = null;
			if (animator != null) {
				RuntimeAnimatorController ac = animator.runtimeAnimatorController;
				for(int i = 0; i<ac.animationClips.Length; i++)                 //For all animations{
					if(ac.animationClips[i].name == "AttackAnim")        //If it has the same name as your clip
						attackClip = ac.animationClips[i];

				if(attackClip != null) 
					attackDelay = attackClip.length;
					
				// begin the attack
				animator.SetTrigger("Attack");
			}

			startTime = Time.time;
			hitboxSpawnDelay = attackDelay * .8f;
			hitBoxSpawned = false;
		}


		public override TaskStatus OnUpdate () {
			if (health == null || health.Amount <= 0 || attackDelay <= 0) 
				return TaskStatus.Failure;

			// approximate the time within the animation to do a hit check
			if (!hitBoxSpawned && startTime + hitboxSpawnDelay < Time.time) 
				if(CheckForHit()) 
					health.takeDamage(damage.Value);

			// succeed when the animation is over
			if (startTime + attackDelay < Time.time) 
				return TaskStatus.Success;

			return TaskStatus.Running;
		}


		bool CheckForHit() {
			hitBoxSpawned = true;
			Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0);
			Collider[] hits = Physics.OverlapSphere(pos, range.Value);
			for (int i = 0; i < hits.Length; ++i) 
				if(hits[i].transform.gameObject == target.Value)
					return true;
			return false;
		}

		// Draw the seeing radius
		public override void OnDrawGizmos()
		{
			#if UNITY_EDITOR
			if (Owner == null || range == null) {
				return;
			}
			var oldColor = UnityEditor.Handles.color;
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.forward, range.Value);
			UnityEditor.Handles.color = oldColor;
			#endif
		}
	}
}

