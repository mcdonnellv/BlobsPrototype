using UnityEngine;
using System.Collections;

[System.Serializable]
public class CombatStats {
	public int attack;
	public int armor;
	public int health;
	public int stamina;
	public Element element;

	public static int defaultAttack = 100;
	public static int defaultArmor = 100;
	public static int defaultHealth = 100;
	public static int defaultStamina = 100;
	public static Element defaultElement = Element.None;

	public CombatStats() {
		SetDefaultValues();
	}

	public void SetDefaultValues() {
		attack = defaultAttack;
		armor = defaultArmor;
		health = defaultHealth;
		stamina = defaultStamina;
		element = defaultElement;
	}

	public void CalculateOtherStats(TraitType t, float v) {
		int value = (int)v;
		switch (t) {
		case TraitType.SetElement: element = (Element)v; break;
		}
	}

	public void CalculateAddedStats(TraitType t, float v) {
		int value = (int)v;
		switch (t) {
		case TraitType.AttackMod: attack += value; break;
		case TraitType.ArmorMod: armor += value; break;
		case TraitType.HealthMod: health += value; break;
		case TraitType.StaminaMod: stamina += value; break;
		}
	}
	
	public void CalculatePercentStats(TraitType t, float v) {
		switch (t) {
		case TraitType.AttackMod: attack = (int)(attack * v); break;
		case TraitType.ArmorMod: armor = (int)(armor * v); break;
		case TraitType.HealthMod: health = (int)(health * v); break;
		case TraitType.StaminaMod: stamina = (int)(stamina * v); break;
		}
	}
}
