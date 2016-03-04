using UnityEngine;
using System.Collections;

public class BattleObjectObstacle : MonoBehaviour {
	protected ActorHealth health;

	public void Start () {
		health = GetComponent<ActorHealth>();
		if(health != null)
			health.onDeath += Death;
	}

	private void Death() {
		Destroy(gameObject);
	}
}
