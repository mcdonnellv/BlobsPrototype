using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiChooseAttack : Action {

	public SharedFloat meleeDistance;
	private Enemy enemy;
	private Animator animator;

	public override void OnAwake() {
		enemy = GetComponent<Enemy>();
		animator = GetComponent<Animator>();
	}

	public override void OnStart() {}

	public override TaskStatus OnUpdate() {
		int index = UnityEngine.Random.Range(0, enemy.attackPrefabs.Count);
		enemy.attackIndex = index;
		AnimatorOverrideController overrideController = new AnimatorOverrideController();
		animator.runtimeAnimatorController = overrideController;
		AnimationClip clip = overrideController["MeleeAttackAnim"];
		clip = enemy.attackPrefabs[index].animationClip;

		return TaskStatus.Success;
		
	}
}
