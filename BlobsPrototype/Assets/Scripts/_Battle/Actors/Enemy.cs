using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;

public class Enemy : Actor {

	public int monsterId = 0;
	public BaseMonster monsterData;

	public void Awake () {
		monsterData = MonsterManager.monsterManager.GetBaseMonsterByID(monsterId);
	}

	// Use this for initialization
	public override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	// This function is called by the animation system on the frame of the attack 
	public override ActorAttack SpawnAttackBox() {
		ActorAttack aa = base.SpawnAttackBox();
		aa.validTargetTags.Add("Blob");
		aa.transform.position += new Vector3(0f, .5f, 0f) + (transform.right * 1);
		return aa;
	}


}
