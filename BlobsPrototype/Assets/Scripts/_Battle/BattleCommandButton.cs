using UnityEngine;
using System.Collections;

public class BattleCommandButton : UIButton {

	public UIProgressBar progressBar;
	public BattleCommand command;
	private float timeToFill = 1f;
	private bool pressed = false;

	// Use this for initialization
	void Start () {
		progressBar.value = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(progressBar.value > 0)
			progressBar.value -= Time.deltaTime * (1 / timeToFill);

		//for continuous button holding
		if(pressed) {
			Pressed();
		}
	}

	public void OnValuechange() {
	}

	public void Pressed() {
		transform.parent.BroadcastMessage("ButtonPressed", gameObject);
		CombatManager.combatManager.inputCommand = command;
	}

	void ButtonPressed(GameObject other) {
		isEnabled = (gameObject != other);
	}

	void BattleCommandExecuted(object[] objs) {
		BattleCommand cmd = (BattleCommand)objs[0];
		float t =  (float)objs[1];
		if(command == cmd) {
			isEnabled = true;
			progressBar.alpha = .5f;
			progressBar.value = 1f;
			timeToFill = t;
		}
	}

	void BattleCommandCompleted(object[] objs) {
		BattleCommand cmd = (BattleCommand)objs[0];
		isEnabled = true;
		if(command == cmd)
			progressBar.alpha = 0f;
	}

	protected override void OnPress (bool isPressed) {
		pressed = isPressed;
		base.OnPress(isPressed);
	}
}
