using UnityEngine;
using System.Collections;

[System.Serializable]
public class CombatStats : MonoBehaviour {
	public int attack;
	public int armor;
	public int health;
	public int stamina;

	public CombatStats() {
		SetValues(0);
	}

	public void SetValues(int v) {
		attack = v;
		armor = v;
		health = v;
		stamina = v;
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
