using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stat {
	public CombatStatType type;
	public int defaultValue;
	public int birthValue;
	public int geneModdedValue;
	public int combatValue;
	public static int defaultStartingValue = 100;

	public Stat(CombatStatType t, Stat s) {
		type = t;
		defaultValue = s.defaultValue;
		birthValue = s.birthValue;
		geneModdedValue = s.geneModdedValue;
		combatValue = s.combatValue;
	}
	
	public Stat(CombatStatType t, int v) {
		type = t;
		defaultValue = v;
		birthValue = v;
		geneModdedValue = v;
		combatValue = v;
	}

	public void BirthDone() {
		geneModdedValue = birthValue;
		combatValue = birthValue;
	}

	public void ResetCombatValue() {
		combatValue = geneModdedValue;
	}
}
