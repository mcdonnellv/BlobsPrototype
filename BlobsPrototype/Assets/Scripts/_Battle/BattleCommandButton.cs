using UnityEngine;
using System.Collections;

public class BattleCommandButton : UIButton {

	public UIProgressBar progressBar;
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

	public void Pressed() {
		transform.parent.BroadcastMessage("ButtonPressed", gameObject); //all buttons go on unified cooldown
		CombatManager.combatManager.inputCommand = command;
	}

	void ButtonPressed(GameObject other) {
		isEnabled = false;
		progressBar.value = 1f;
		progressBar.alpha = .5f;
		timeToFill = Mathf.Infinity;
	}

	void BattleCommandExecuted(object[] objs) {
		BattleCommand cmd = (BattleCommand)objs[0];
		float t =  (float)objs[1];
		timeToFill = t;
		Invoke("EnableButton", t - .2f); //enable early so we can queue a command with no downtime
	}

	void EnableButton() {
		isEnabled = true;
	}
}
