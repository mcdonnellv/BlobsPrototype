using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiSimCanMove : Conditional {

	public override TaskStatus OnUpdate() {
		if (CombatManager.combatManager.CanBlobsMoveForward())
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}
}
