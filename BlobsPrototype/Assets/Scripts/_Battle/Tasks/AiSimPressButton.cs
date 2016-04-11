using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiSimPressButton : Action {

	public UIButton button;

	// OnUpdate will return success if the object is still alive and failure if it not
	public override TaskStatus OnUpdate() {
		button.SendMessage("OnClick");
		return TaskStatus.Success;
	}
}
