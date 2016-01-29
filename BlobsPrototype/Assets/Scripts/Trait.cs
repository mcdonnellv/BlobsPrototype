using UnityEngine;
using System.Collections;


public class Trait {

	public static bool IsPreCombatTrait(TraitCondition tc) {
		switch(tc) {
		case TraitCondition.Alone:
		case TraitCondition.ForEachOtherBlob:
			return true;
		}
		return false;
	}


	public static bool IsCombatTraitConditionMet(TraitCondition tc) {
		switch(tc) {
		case TraitCondition.Alone: return (CombatManager.combatManager.GetFactionCount(Faction.blob, false) == 1);
		case TraitCondition.ForEachOtherBlob: return (CombatManager.combatManager.GetFactionCount(Faction.blob, false) > 1);
		}
		return false;
	}


	public static void ProcessCombatTrait(TraitType t, int val, AbilityModifier m, CombatStats c) {
		CombatStatType statType = CombatStatType.Attack;
		switch(t) {
		case TraitType.AttackMod: statType = CombatStatType.Attack; break;
		case TraitType.ArmorMod: statType = CombatStatType.Armor; break;
		case TraitType.HealthMod: statType = CombatStatType.Health; break;
		case TraitType.StaminaMod: statType = CombatStatType.Stamina; break;
		case TraitType.SpeedMod: statType = CombatStatType.Speed; break;
		}

		Stat stat = c.allStats[(int)statType];
		stat.combatValue = (m == AbilityModifier.Added) ? stat.combatValue + val : stat.combatValue * val;
	}
}
