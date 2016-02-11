using UnityEngine;
using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

[Serializable]
public class BaseMonster : BaseThing {
	public string actorPrefabName = "Enemy";
	public BehaviorTree behaviorTree;
	public float health = 1000f;
	public float attack = 100f;
	public float speed = 100f;
	public float damageMitigation = 0.5f;
	public float stamina = 100f;
	public float walkSpeed = 1f;
	public float runSpeed = 2f;
	public float staggerLimit = 100f;
	public float criticalHealth = 0.2f;
	public float enrageDuration = 30f;
	public float enrageAttack = 1.3f;
	public float enrageSpeed = 1.2f;
	public float enrageConditionTimer = 60f;
	public float perception = 10f;
}
