using UnityEngine;
using System.Collections;

public class BattleBlobLifeBar : MonoBehaviour {
	public ActorHealth health;
	public UIProgressBar lifeBar;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(health != null && lifeBar != null && health.startHealth != 0)
			lifeBar.value = health.health / health.startHealth;
	}
}
