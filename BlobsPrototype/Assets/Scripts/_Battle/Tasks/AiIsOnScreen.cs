using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiIsOnScreen : Conditional {
	public SharedGameObject targetGameObject;
	private GameObject prevGameObject;
	private GameObject currentGameObject;
	public override void OnStart() {
		currentGameObject = GetDefaultGameObject(targetGameObject.Value);
		if (currentGameObject != prevGameObject) {
			prevGameObject = currentGameObject;
		}
	}

	public override TaskStatus OnUpdate() {
		if(AiManager.IsObjectOnScreen(currentGameObject))
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}
}
