using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public enum Faction {
	blob,
	enemy,
};

public class Combatant : MonoBehaviour {
	public int id = 0;
	public CombatStats combatStats;
	public DateTime turnTime = DateTime.MaxValue;
	public Faction faction;
	public ICombatantController controller;

	public Combatant target;
	public float perception = 10f;

	public Vector3 targetMovePos;

	public bool IsAlive() { return gameObject.GetComponent<CombatantHealth>().health > 0; }
	public void MoveBackward(float moveSpeed) { MoveForward(-moveSpeed); }

	public void MoveForward(float moveSpeed) {
		moveSpeed *= (faction == Faction.enemy) ? -1 : 1;
		Rigidbody2D r = GetComponent<Rigidbody2D>();
		r.velocity = new Vector2(transform.localScale.x * moveSpeed, r.velocity.y);
	}

	void Update() {
		if(controller.moving) {
			bool movingRight = (GetComponent<Rigidbody2D>().velocity.x > 0);
			if(movingRight && transform.localPosition.x > targetMovePos.x)
				transform.localPosition = targetMovePos;
			else if(!movingRight && transform.localPosition.x < targetMovePos.x)
				transform.localPosition = targetMovePos;
			if(transform.localPosition == targetMovePos)
				controller.DestinationReached();
		}
	}




//	public void SetInitialRandomCombatSpeed() {
//		combatStats.speed.ResetCombatValue();
//		int variance = Mathf.FloorToInt(combatStats.speed.combatValue * .2f);
//		int modAmount = UnityEngine.Random.Range(-variance, variance + 1);
//		combatStats.speed.combatValue += modAmount;
//	}
//
//
//	public void CalculatePreCombatStats() {
//		if(isMonster)
//			return;
//
//		foreach(Gene g in blob.genes) {
//			if(Trait.IsPreCombatTrait(g.traitCondition) && Trait.IsCombatTraitConditionMet(g.traitCondition)) {
//				Trait.ProcessCombatTrait(g.traitType, g.value, g.modifier, combatStats);
//				HudManager.hudManager.combatMenu.PushMessage(name + "'s " + g.itemName + " gene activates");
//			}
//		}
//	}
//
//	public void CalculatePreCombatStats(List<QuestBonus> combatBonus) {
//		if(combatBonus.Count == 0)
//			return;
//		foreach(QuestBonus qb in combatBonus) {
//			float val = 1f + qb.value;
//			switch(qb.type) {
//			case QuestBonusType.Armor:  combatStats.armor.combatValue = Mathf.FloorToInt(combatStats.armor.combatValue * val); break;
//			case QuestBonusType.Attack: combatStats.attack.combatValue = Mathf.FloorToInt(combatStats.attack.combatValue * val); break;
//			case QuestBonusType.Health: combatStats.health.combatValue = Mathf.FloorToInt(combatStats.health.combatValue * val); break;
//			case QuestBonusType.Speed:  combatStats.speed.combatValue = Mathf.FloorToInt(combatStats.speed.combatValue * val); break;
//			}
//		}
//	}
}