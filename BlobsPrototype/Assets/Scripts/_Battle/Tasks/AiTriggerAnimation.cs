using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiTriggerAnimation : Action {

	public SharedString animationName;
	public SharedString triggerName;
	public SharedBool waitOnAnimation;
	private float startTime;
	private float animationDelay;
	private Animator animator;

	public override void OnAwake() {
		animator = GetComponent<Animator>();
	}

	public override void OnStart() {
		if (animator == null) {
			Debug.Log("Animator component not found");
			return;
		}

		startTime = Time.time;
		animationDelay = 0f;

		AnimationClip clip = null;
		RuntimeAnimatorController ac = animator.runtimeAnimatorController;
		for(int i = 0; i<ac.animationClips.Length; i++)                 //For all animations{
			if(ac.animationClips[i].name == animationName.Value)        //If it has the same name as your clip
				clip = ac.animationClips[i];

		if(clip == null) {
			Debug.Log(animationName.Value + " animation not found");
			return;
		}

		// remember the length of this animation
		animationDelay = clip.length;

		// begin the animation
		animator.SetTrigger(triggerName.Value);
	}


	public override TaskStatus OnUpdate () {
		if(animator == null || animationDelay <= 0)
			return TaskStatus.Failure;

		// succeed when the animation is over
		if(startTime + animationDelay < Time.time)
			return TaskStatus.Success;

		if(waitOnAnimation.Value == false)
			return TaskStatus.Success;

		return TaskStatus.Running;
	}

}