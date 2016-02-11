using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiShouldAnimStall : Conditional {
	private Animator anim;
	private string currentAnimName;


	public override void OnAwake() {
		anim = GetComponent<Animator>();
	}
	
	public override void OnStart() {
		if(anim == null)
			return;
		AnimatorClipInfo[] ac = anim.GetCurrentAnimatorClipInfo(0);
		currentAnimName = ac[0].clip.name;
	}

	public override TaskStatus OnUpdate() {

		if (currentAnimName == "FlinchAnim" ||
		    currentAnimName == "DeathAnim" ||
		    currentAnimName == "DeadAnim")
			return TaskStatus.Success;

		return TaskStatus.Failure;
	}

}
