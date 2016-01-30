using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlobCombatController : MonoBehaviour, ICombatantController {

	public Combatant target;
	CombatManager combatManager { get { return CombatManager.combatManager; } }
	float actionDelay = 1f;

	void LookForTarget() {
		List<Combatant> enemies = combatManager.GetCombatantsByFaction(Faction.enemy,true);
		if(enemies.Count > 0)
			target = enemies[0];
		Invoke("Attack", actionDelay);
	}

	void Attack() {
		//for now do all melee
		Invoke("ChargeIn", 0);
	}

	void ChargeIn() {
		Combatant combatant = go.GetComponent<Combatant>();
		combatant.MoveForward(moveSpeed);
		combatant.targetMovePos = target.transform.localPosition();
	}

	void MeleeAttack();
	void ShootAttack();
	void MarchForward();
	void RecieveBuff();
	void Guard();
	void Dodge();

	void DestinationReached() {
		moving = false;
	};
}
