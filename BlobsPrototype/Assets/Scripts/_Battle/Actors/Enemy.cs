using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	// This function is called by the animation system on the frame of the attack 
	public ActorAttack SpawnAttackBox(float size) {
		ActorAttack aa = base.SpawnAttackBox(size);
		aa.opposingFactionTags.Add("Blob");
		aa.transform.position += new Vector3(0f, .5f, 0f) + (transform.right * 1);
		return aa;
	}
}
