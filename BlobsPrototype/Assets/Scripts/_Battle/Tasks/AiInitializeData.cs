using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AiInitializeData : Composite {

	public SharedBool initialized;
	public SharedFloat health;
	public SharedFloat attack;
	public SharedFloat speed;
	public SharedFloat damageMitigation;
	public SharedFloat stamina;
	public SharedFloat walkSpeed;
	public SharedFloat runSpeed;
	public SharedFloat staggerLimit;
	public SharedFloat criticalHealth;
	public SharedFloat enrageDuration;
	public SharedFloat enrageAttack;
	public SharedFloat enrageSpeed;
	public SharedFloat enrageConditionTimer;
	public SharedFloat perception;

	public override void OnStart() {
		if(initialized.Value == false) {
			initialized.Value = true;

			Enemy actor = GetComponent<Enemy>();
			if(actor == null) {
				Debug.LogError("No Actor component to initialize data from");
				return;
			}
			BaseMonster m = actor.monsterData;

			health.Value = m.health;
			attack.Value = m.attack;
			speed.Value = m.speed;
			damageMitigation.Value = m.damageMitigation;
			stamina.Value = m.stamina;
			walkSpeed.Value = m.walkSpeed;
			runSpeed.Value = m.runSpeed;
			staggerLimit.Value = m.staggerLimit;
			criticalHealth.Value = m.criticalHealth;
			enrageDuration.Value = m.enrageDuration;
			enrageAttack.Value = m.enrageAttack;
			enrageSpeed.Value = m.enrageSpeed;
			enrageConditionTimer.Value = m.enrageConditionTimer;
			perception.Value = m.perception;
		}
	}
}
