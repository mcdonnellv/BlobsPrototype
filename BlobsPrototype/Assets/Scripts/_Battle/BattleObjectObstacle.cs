using UnityEngine;
using System.Collections;

public class BattleObjectObstacle : MonoBehaviour {

	public void OnTriggerEnter (Collider other) {
		if(other.tag == "Blob")
			CombatManager.combatManager.RevertBlobAnchorPosition();
	}
}
