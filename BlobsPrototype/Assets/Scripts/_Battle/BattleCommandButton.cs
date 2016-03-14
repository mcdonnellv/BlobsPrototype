using UnityEngine;
using System.Collections;

public class BattleCommandButton : MonoBehaviour {

	public UIProgressBar progressBar;
	public UIButton button;
	public BattleCommand command;
	private float timeToFill = 1f;

	// Use this for initialization
	void Start () {
		progressBar.value = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(progressBar.value > 0)
			progressBar.value -= Time.deltaTime * (1 / timeToFill);
	}

	public void OnValuechange() {
		if(progressBar.value == 0f) {
			progressBar.alpha = 0f;
			transform.parent.BroadcastMessage("ButtonActionCompleted", gameObject);
		}
	}

	public void Pressed() {
		transform.parent.BroadcastMessage("ButtonPressed", gameObject);
		CombatManager.combatManager.inputCommand = command;
	}

	protected void ButtonPressed(GameObject other) {
		button.isEnabled = (gameObject != other);
	}

	protected void ButtonActionCompleted(GameObject other) {
		button.isEnabled = true;
	}

	public void BattleCommandExecuted(object[] objs) {
		BattleCommand cmd = (BattleCommand)objs[0];
		float t =  (float)objs[1];

		if(command != cmd) {
			button.isEnabled = false;
			return;
		}
		
		progressBar.alpha = .5f;
		progressBar.value = 1f;
		timeToFill = t;
	}
}
