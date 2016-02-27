using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

[Serializable]
public class ActorData {
	public float health = 1000f;
	public float attack = 100f;
	public float speed = 100f;
	public float damageMitigation = 0.5f;
	public float stamina = 100f;
	public float walkSpeed = 1f;
	public float runSpeed = 2f;
	public float flinchLimit = 100f;
	public float criticalHealth = 0.2f;
	public float enrageDuration = 30f;
	public float enrageAttack = 1.3f;
	public float enrageSpeed = 1.2f;
	public float enrageConditionTimer = 60f;
	public float perception = 10f;
	public float staminaRecovery = 5f; // pts per sec 
	public float exhaustRecoveryPoint = 0.5f; // exits exhausted state if stam/totalstam > this number
	public string opposingFaction = "Blob";
	public float jumpForce = 500f;
}
