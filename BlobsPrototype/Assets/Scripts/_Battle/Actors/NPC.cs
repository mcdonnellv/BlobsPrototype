using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// an actor with ai
public abstract class NPC : Actor {
	List<Behavior> behaviors = new List<Behavior>();

	// Use this for initialization
	public override void Start () {
		behaviors.Add(new Patrol(this, AiManager.aiManager.patrolPoints));
	}
	
	// Update is called once per frame
	public override void Update () {
		foreach(Behavior b in behaviors) {
			b.Tick();
		}
		base.Update();
	}
}