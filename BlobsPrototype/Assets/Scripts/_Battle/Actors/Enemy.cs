using UnityEngine;
using System.Collections;

public class Enemy : NPC {

	// Use this for initialization
	public override void Start () {
		faction = Faction.enemy;
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}
}
