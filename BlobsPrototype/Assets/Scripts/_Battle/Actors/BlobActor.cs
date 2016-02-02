using UnityEngine;
using System.Collections;

public class BlobActor : Actor {
	// Use this for initialization
	void Start () {
		faction = Faction.blob;
		
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}
}
