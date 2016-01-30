using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// this is the brain of a combatant, things he does
public interface ICombatantController {
	void LookForTarget();
	void ChargeIn();
	void MeleeAttack();
	void ShootAttack();
	void MarchForward();
	void RecieveBuff();
	void Guard();
	void Dodge();
}
