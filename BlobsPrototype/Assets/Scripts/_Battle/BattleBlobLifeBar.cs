using UnityEngine;
using System.Collections;

public class BattleBlobLifeBar : BattleLifeBar {
	// add code for portrait

	void Start() {
		moveWithActor = false;
	}

	protected override void HostDead() {
	}
}
