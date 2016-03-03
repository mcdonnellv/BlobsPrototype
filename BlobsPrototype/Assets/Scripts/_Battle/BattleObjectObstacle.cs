using UnityEngine;
using System.Collections;

public class BattleObjectObstacle : MonoBehaviour {
	protected ActorHealth health;

	public void Start () {
		health = GetComponent<ActorHealth>();
		if(health != null)
			health.onDeath += Death;
	}

	public void OnTriggerEnter (Collider other) {
		if(other.tag == "Blob")
			CombatManager.combatManager.RevertBlobAnchorPosition();
	}

	private void Death() {
		Destroy(gameObject);
	}
}
