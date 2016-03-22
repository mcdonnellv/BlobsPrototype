using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiSimBattleCommand : Action {

	public BattleCommand command;

	public override void OnStart() {
		CombatManager.combatManager.inputCommand = command;
	}

	// OnUpdate will return success if the object is still alive and failure if it not
	public override TaskStatus OnUpdate() {
		return TaskStatus.Success;
	}
}
