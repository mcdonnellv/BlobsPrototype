﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Combatant {
	public Monster monster;
	public Blob blob;
	public int id = 0;
	public Combatant target;
	public CombatStats combatStats;
	public string name;
	public DateTime turnTime = DateTime.MaxValue;
	public bool isBlob { get { return (blob != null); } }
	public bool isMonster { get { return (monster != null); } }


	public void SetInitialRandomCombatSpeed() {
		combatStats.speed.ResetCombatValue();
		int variance = Mathf.FloorToInt(combatStats.speed.combatValue * .2f);
		int modAmount = UnityEngine.Random.Range(-variance, variance + 1);
		combatStats.speed.ModCombatValue(AbilityModifier.Added, modAmount);
	}


	public void CalculatePreCombatStats() {
		if(isMonster)
			return;

		foreach(Gene g in blob.genes) {
			if(g.active && Trait.IsPreCombatTrait(g.traitCondition) && Trait.IsCombatTraitConditionMet(g.traitCondition)) {
				Trait.ProcessCombatTrait(g.traitType, g.value, g.modifier, combatStats);
				HudManager.hudManager.combatMenu.PushMessage(name + "'s " + g.itemName + " gene activates");
			}
		}
	}
}