using UnityEngine;
using System.Collections;


public class Trait {

	public static bool IsPersistentTrait(TraitCondition tc) {
		return (!Trait.IsPreCombatTrait(tc));
	}


	public static bool IsPreCombatTrait(TraitCondition tc) {
		switch(tc) {
		case TraitCondition.Alone:
		case TraitCondition.ForEachOtherBlob:
			return true;
		}
		return false;
	}


	public static bool IsPersistentTraitConditionMet(TraitCondition tc, Blob blob) {
		switch(tc) {
		case TraitCondition.None: return true;
		case TraitCondition.ColorBlack: return (blob.element == Element.Black);
		case TraitCondition.ColorBlue: return (blob.element == Element.Blue);
		case TraitCondition.ColorWhite: return (blob.element == Element.White);
		case TraitCondition.ColorGreen: return (blob.element == Element.Green);
		case TraitCondition.ColorRed: return (blob.element == Element.Red);
		}
		return false;
	}


	public static bool IsCombatTraitConditionMet(TraitCondition tc) {
		switch(tc) {
		case TraitCondition.Alone: return (CombatManager.combatManager.GetBlobCount() == 1);
		case TraitCondition.ForEachOtherBlob: return (CombatManager.combatManager.GetBlobCount() > 1);
		}
		return false;
	}


	public static void ProcessPeristentTrait(TraitType t, int val, AbilityModifier m, CombatStats c) {
		CombatStatType statType = CombatStatType.Attack;
		switch(t) {
		case TraitType.AttackMod: statType = CombatStatType.Attack; break;
		case TraitType.ArmorMod: statType = CombatStatType.Armor; break;
		case TraitType.HealthMod: statType = CombatStatType.Health; break;
		case TraitType.StaminaMod: statType = CombatStatType.Stamina; break;
		case TraitType.SpeedMod: statType = CombatStatType.Speed; break;
		}
		
		Stat stat = c.StatFromType(statType);
		stat.ModGeneModdedValue(m, val);
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

		Stat stat = c.StatFromType(statType);
		stat.ModCombatValue(m, val);
	}
}
