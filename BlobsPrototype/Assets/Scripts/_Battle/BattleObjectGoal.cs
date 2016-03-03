using UnityEngine;
using System.Collections;

public class BattleObjectGoal : MonoBehaviour {

	public void OnTriggerEnter (Collider other) {
		if(other.tag == "Blob")
			CombatManager.combatManager.ResetLevel();
	}
}
