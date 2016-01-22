using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stat {
	public CombatStatType type;
	int _defaultValue;
	int _birthValue;
	int _geneModdedValue;
	int _combatValue;

	public int defaultValue { get { return _defaultValue; } }
	public int birthValue { get { return _birthValue; } }
	public int geneModdedValue { get { return _geneModdedValue; } }
	public int combatValue { get { return _combatValue; } }
	public static int defaultStartingValue = 100;

	public Stat(CombatStatType t, Stat s) {
		type = t;
		_defaultValue = s.defaultValue;
		_birthValue = s.birthValue;
		_geneModdedValue = s.geneModdedValue;
		_combatValue = s.combatValue;
	}
	
	public Stat(CombatStatType t, int v) {
		type = t;
		_defaultValue = v;
		_birthValue = v;
		_geneModdedValue = v;
		_combatValue = v;
	}

	public void ResetCombatValue() { _combatValue = _geneModdedValue; }
	public void ResetGeneModdedValue() { _geneModdedValue = _birthValue; }
	public void ModBirthValue(float v) { _birthValue = _defaultValue + (int)v; }
	public void ModGeneModdedValue(float v) { _geneModdedValue = _birthValue + (int)v; }
	public void ModGeneModdedValue(AbilityModifier m, float v) { _geneModdedValue = (m == AbilityModifier.Added) ? _birthValue + (int)v : Mathf.FloorToInt(_birthValue * v); }
	public void ModCombatValue(AbilityModifier m, float v) { _combatValue = (m == AbilityModifier.Added) ? _combatValue + (int)v :  Mathf.FloorToInt(_combatValue * v); }
}
